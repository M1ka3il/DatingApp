import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);

  baseUrl = environment.apiUrl;

  login(creds: any){
    return this.http.post(this.baseUrl + 'account/login', creds);
  }
}