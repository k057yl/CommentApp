import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private http: HttpClient) { }

  login(data: any) {
    return this.http.post('/login', data).pipe(
      tap(() => {
        localStorage.setItem('isLoggedIn', 'true');
      })
    );
  }

  register(data: any) {
    return this.http.post('/register', data);
  }
}