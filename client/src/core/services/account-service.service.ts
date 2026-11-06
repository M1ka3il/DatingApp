import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { User } from '../models/user';
import { PresenceService } from './presence-service';
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
    return this.http.post<User>(this.baseUrl + 'account/login', creds).pipe(
      tap((user) => this.setCurrentUser(user))
    );
  }

  register(creds: { userName: string; email: string; password: string }) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds).pipe(
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

  logout() {
    localStorage.removeItem(AccountService.StorageKey);
    this.presenceService.stopHubConnection();
    this.currentUser.set(null);
  }
}
