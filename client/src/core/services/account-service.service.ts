import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { tap } from 'rxjs';
import { User } from '../models/user';
import { PresenceService } from './presence-service';
import { SKIP_ERROR_TOAST } from '../interceptors/error-interceptor';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private presenceService = inject(PresenceService);
  private static readonly StorageKey = 'user';

  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);

  login(creds: { email: string; password: string }) {
    // The navbar shows its own "Failed to log in" toast.
    const context = new HttpContext().set(SKIP_ERROR_TOAST, true);
    return this.http.post<User>(this.baseUrl + 'account/login', creds, { context }).pipe(
      tap((user) => this.setCurrentUser(user))
    );
  }

  register(creds: { userName: string; email: string; password: string }) {
    // The register form displays server errors inline – no extra toast.
    const context = new HttpContext().set(SKIP_ERROR_TOAST, true);
    return this.http.post<User>(this.baseUrl + 'account/register', creds, { context }).pipe(
      tap((user) => this.setCurrentUser(user))
    );
  }

  setCurrentUser(user: User) {
    localStorage.setItem(AccountService.StorageKey, JSON.stringify(user));
    this.currentUser.set(user);
    this.presenceService.createHubConnection(user);
  }

  // Restore the persisted session on app start.
  loadCurrentUser() {
    const stored = localStorage.getItem(AccountService.StorageKey);
    if (stored) {
      const user = JSON.parse(stored) as User;
      this.currentUser.set(user);
      this.presenceService.createHubConnection(user);
    }
  }

  // Keep the navbar avatar in sync after the main photo changed.
  updateImageUrl(imageUrl: string | null) {
    const user = this.currentUser();
    if (!user) return;
    const updated = { ...user, imageUrl };
    localStorage.setItem(AccountService.StorageKey, JSON.stringify(updated));
    this.currentUser.set(updated);
  }

  logout() {
    localStorage.removeItem(AccountService.StorageKey);
    this.presenceService.stopHubConnection();
    this.currentUser.set(null);
  }
}
