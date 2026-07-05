import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs';
import { Member, Photo } from '../models/member';
import { PaginatedResult } from '../models/pagination';
import { UserParams } from '../models/user-params';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);

  baseUrl = environment.apiUrl;

  getMembers(userParams: UserParams) {
    let params = new HttpParams()
      .append('pageNumber', userParams.pageNumber)
      .append('pageSize', userParams.pageSize)
      .append('orderBy', userParams.orderBy)
      .append('direction', userParams.direction);

    if (userParams.search) {
      params = params.append('search', userParams.search);
    }

    return this.http
      .get<Member[]>(this.baseUrl + 'members', { observe: 'response', params })
      .pipe(
        map((response) => {
          const paginatedResult = new PaginatedResult<Member[]>();
          paginatedResult.items = response.body ?? [];
          const pagination = response.headers.get('Pagination');
          if (pagination) {
            paginatedResult.pagination = JSON.parse(pagination);
          }
          return paginatedResult;
        })
      );
  }

  getMember(id: string) {
    return this.http.get<Member>(this.baseUrl + 'members/' + id);
  }

  uploadPhoto(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + 'members/add-photo', formData);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'members/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'members/delete-photo/' + photoId);
  }
}
