import { HttpContextToken, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast-service';

// Set on a request whose caller shows 400/401 errors itself (register form, login),
// so the user doesn't get the same message twice.
export const SKIP_ERROR_TOAST = new HttpContextToken<boolean>(() => false);

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((error) => {
      if (error) {
        switch (error.status) {
          case 400:
            if (error.error.errors) {
              // ModelState validation errors -> flatten to a single array
              const modelStateErrors = [];
              for (const key in error.error.errors) {
                if (error.error.errors[key]) {
                  modelStateErrors.push(error.error.errors[key]);
                }
              }
              throw modelStateErrors.flat();
            } else if (!req.context.get(SKIP_ERROR_TOAST)) {
              toast.error(error.error, error.status);
            }
            break;
          case 401:
            if (!req.context.get(SKIP_ERROR_TOAST)) {
              toast.error('Unauthorised', error.status);
            }
            break;
          case 404:
            router.navigateByUrl('/not-found');
            break;
          case 500:
            const navigationExtras = { state: { error: error.error } };
            router.navigateByUrl('/server-error', navigationExtras);
            break;
          default:
            toast.error('Something unexpected went wrong');
            break;
        }
      }
      return throwError(() => error);
    })
  );
};
