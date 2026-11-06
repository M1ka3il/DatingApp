import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../../core/services/account-service.service';
import { ToastService } from '../../core/services/toast-service';

// Group-level validator: password and confirmPassword must be identical.
const passwordsMatch: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return password && confirm && password !== confirm ? { passwordMismatch: true } : null;
};

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private toast = inject(ToastService);
  private router = inject(Router);

  // Same rules as API/DTO/RegisterDTO.cs (+ client-only password confirmation).
  protected form = this.fb.nonNullable.group(
    {
      userName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: passwordsMatch }
  );

  protected submitting = signal(false);
  protected serverErrors = signal<string[]>([]);

  constructor() {
    // Already logged in (e.g. /register opened via URL): registering again would
    // replace the session and leave the old presence connection open.
    if (this.accountService.currentUser()) {
      this.router.navigateByUrl('/members');
    }
  }

  protected showError(name: 'userName' | 'email' | 'password' | 'confirmPassword'): boolean {
    const control = this.form.controls[name];
    return control.invalid && (control.touched || control.dirty);
  }

  protected showMismatch(): boolean {
    const confirm = this.form.controls.confirmPassword;
    return this.form.hasError('passwordMismatch') && (confirm.touched || confirm.dirty);
  }

  register() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { userName, email, password } = this.form.getRawValue();
    this.submitting.set(true);
    this.serverErrors.set([]);

    this.accountService.register({ userName, email, password }).subscribe({
      next: () => {
        this.toast.success('Account created – welcome!');
        this.router.navigateByUrl('/members');
      },
      error: (error) => {
        this.submitting.set(false);
        // The error interceptor rethrows ModelState errors as a string array;
        // plain 400s (e.g. "Username is taken") arrive as HttpErrorResponse.
        if (Array.isArray(error)) {
          this.serverErrors.set(error);
        } else if (typeof error?.error === 'string') {
          this.serverErrors.set([error.error]);
        } else {
          this.serverErrors.set(['Registration failed. Please try again.']);
        }
      },
    });
  }

  cancel() {
    this.router.navigateByUrl('/members');
  }
}
