import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment, PagedResponse } from '../models/comment.model';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private apiUrl = environment.apiUrl + '/comments'; 
  private hubUrl = environment.hubUrl;
  
  private hubConnection!: signalR.HubConnection;

  constructor(private http: HttpClient) { 
    this.initSignalR();
  }

  private initSignalR() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR: Connection started'))
      .catch(err => console.error('SignalR: Error while starting connection: ' + err));
  }

  public onNewComment(callback: () => void) {
    this.hubConnection.on('ReceiveComment', () => {
      callback();
    });
  }

  getComments(page: number = 1, pageSize: number = 25, sortBy: string = 'date', sortOrder: string = 'desc'): Observable<any> {
  const p = Number(page) || 1;
  const ps = Number(pageSize) || 25;
  const sb = String(sortBy || 'date');
  const so = String(sortOrder || 'desc');

  let params = new HttpParams()
    .set('page', p.toString())
    .set('pageSize', ps.toString())
    .set('sortBy', sortBy || 'date')
    .set('sortOrder', sortOrder || 'desc');
    //.set('_t', new Date().getTime().toString());

  return this.http.get<PagedResponse<Comment>>(this.apiUrl, { params });
}

  createComment(formData: FormData): Observable<Comment> {
    return this.http.post<Comment>(this.apiUrl, formData);
  }

  getCaptcha(): Observable<{ code: string }> {
    return this.http.get<{ code: string }>(`${this.apiUrl}/captcha`);
  }
}