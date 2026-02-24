import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment, PagedResponse } from '../models/comment.model';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private apiUrl = '/api/comments';

  constructor(private http: HttpClient) { }

  getComments(page: number = 1, pageSize: number = 25): Observable<PagedResponse<Comment>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<PagedResponse<Comment>>(this.apiUrl, { params });
  }

  createComment(formData: FormData): Observable<Comment> {
    return this.http.post<Comment>(this.apiUrl, formData);
  }

  getCaptcha(): Observable<{ code: string }> {
    return this.http.get<{ code: string }>(`${this.apiUrl}/captcha`);
  }
}