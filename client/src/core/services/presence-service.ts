import { Injectable, inject, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { ToastService } from './toast-service';
import { User } from '../models/user';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  private toast = inject(ToastService);
  private hubConnection?: HubConnection;

  onlineUsers = signal<string[]>([]);

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.hubUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Error)
      .build();

    this.hubConnection.start().catch((error) => console.error(error));

    this.hubConnection.on('UserIsOnline', (username: string) => {
      this.onlineUsers.update((users) =>
        users.includes(username) ? users : [...users, username]
      );
    });

    this.hubConnection.on('UserIsOffline', (username: string) => {
      this.onlineUsers.update((users) => users.filter((u) => u !== username));
    });

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsers.set(usernames);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === 'Connected') {
      this.hubConnection.stop().catch((error) => console.error(error));
    }
  }
}
