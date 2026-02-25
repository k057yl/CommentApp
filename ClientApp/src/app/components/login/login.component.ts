import { Component, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <div class="login-box">
      <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
        <input formControlName="userName" placeholder="Логин">
        <input formControlName="password" type="password" placeholder="Пароль">
        <button type="submit" [disabled]="loginForm.invalid">Войти</button>
      </form>
    </div>
  `
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loginForm = this.fb.group({
    userName: ['', Validators.required],
    password: ['', Validators.required]
  });

  onSubmit() {
    this.auth.login(this.loginForm.value).subscribe({
      next: () => this.router.navigate(['/']),
      error: () => alert('Неверный логин или пароль')
    });
  }
}