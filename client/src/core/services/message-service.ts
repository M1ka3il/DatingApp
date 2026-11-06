import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { map } from 'rxjs';
import { Message } from '../models/message';
import { PaginatedResult } from '../models/pagination';
import { User } from '../models/user';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;

  private hubConnection?: HubConnection;
  // Live thread populated by the message hub.
  messageThread = signal<Message[]>([]);

  createHubConnection(user: User, otherUserId: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}message?user=${otherUserId}`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Error)
      .build();

    this.hubConnection.start().catch((error) => console.error(error));

    this.hubConnection.on('ReceiveMessageThread', (messages: Message[]) => {
      this.messageThread.set(messages);
    });

    this.hubConnection.on('NewMessage', (message: Message) => {
      this.messageThread.update((thread) => [...thread, message]);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === 'Connected') {
      this.hubConnection.stop().catch((error) => console.error(error));
    }
    this.messageThread.set([]);
  }

  async sendMessageViaHub(recipientId: string, content: string) {
    return this.hubConnection?.invoke('SendMessage', { recipientId, content });
  }

  getMessages(container: string, pageNumber: number, pageSize: number) {
    const params = new HttpParams()
      .append('container', container)
      .append('pageNumber', pageNumber)
      .append('pageSize', pageSize);

    return this.http
      .get<Message[]>(this.baseUrl + 'messages', { observe: 'response', params })
      .pipe(
        map((response) => {
          const result = new PaginatedResult<Message[]>();
          result.items = response.body ?? [];
          const pagination = response.headers.get('Pagination');
          if (pagination) result.pagination = JSON.parse(pagination);
          return result;
        })
      );
  }

  getMessageThread(userId: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + userId);
  }

  sendMessage(recipientId: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', { recipientId, content });
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
