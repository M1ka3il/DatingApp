import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Member } from '../models/member';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);

  baseUrl = environment.apiUrl;

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'members');
  }

  getMember(id: string) {
    return this.http.get<Member>(this.baseUrl + 'members/' + id);
  }
}
