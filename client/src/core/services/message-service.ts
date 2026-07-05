import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs';
import { Message } from '../models/message';
import { PaginatedResult } from '../models/pagination';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

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
