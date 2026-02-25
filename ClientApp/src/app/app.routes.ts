import { Routes } from '@angular/router';
import { CommentListComponent } from './components/comment-list/comment-list.component';
import { LoginComponent } from './components/login/login.component';

export const routes: Routes = [
  { 
    path: '', 
    component: CommentListComponent, 
    title: 'Главная | Комментарии' 
  },
  { 
    path: 'login', 
    component: LoginComponent, 
    title: 'Вход в систему' 
  },
  { 
    path: '**', 
    redirectTo: '' 
  }
];