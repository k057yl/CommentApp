import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CommentService } from '../../services/comment.service';
import { AuthService } from '../../services/auth.service';
import { Comment } from '../../models/comment.model';
import { CommentItemComponent } from '../comment-item/comment-item.component';
import { CommentFormComponent } from '../comment-form/comment-form.component';

@Component({
  selector: 'app-comment-list',
  standalone: true,
  imports: [CommonModule, CommentItemComponent, CommentFormComponent, RouterLink],
  template: `
    <div class="container">
      <h2>Обсуждение</h2>

      <div class="auth-section">
        @if (auth.isLoggedIn() | async) {
          <div class="form-card">
            <h3>Оставить комментарий</h3>
            <app-comment-form (commentCreated)="loadComments()"></app-comment-form>
          </div>
        } @else {
          <div class="login-alert">
            <p>Чтобы оставлять комментарии, пожалуйста, <a routerLink="/login">войдите в систему</a>.</p>
          </div>
        }
      </div>

      <hr />

      <div class="comments-list">
        @for (comment of comments; track comment.id) {
          <app-comment-item 
            [comment]="comment" 
            (commentCreated)="loadComments()">
          </app-comment-item>
        } @empty {
          <p>Комментариев пока нет. Будьте первым!</p>
        }
      </div>
    </div>
  `,
styles: [`
  .container { max-width: 900px; margin: 2rem auto; font-family: 'Inter', sans-serif; }
  h2 { font-weight: 800; color: #1a202c; margin-bottom: 1.5rem; }
  
  .auth-section { 
    background: #ffffff; 
    border-radius: 12px; 
    box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); 
    padding: 1.5rem;
    margin-bottom: 2rem;
  }

  .login-alert {
    background: #f7fafc;
    border: 2px dashed #e2e8f0;
    border-radius: 12px;
    padding: 2rem;
    text-align: center;
    color: #4a5568;
    a { color: #3182ce; font-weight: 600; text-decoration: none; &:hover { text-decoration: underline; } }
  }

  .comments-list { display: flex; flex-direction: column; gap: 1.5rem; }
`]
})
export class CommentListComponent implements OnInit {
  public auth = inject(AuthService);
  private commentService = inject(CommentService);
  
  comments: Comment[] = [];

  ngOnInit() {
    this.loadComments();
  }

  loadComments() {
    this.commentService.getComments().subscribe({
      next: (res) => {
        this.comments = res.items || res; 
        console.log('Комментарии загружены:', this.comments);
      },
      error: (err) => console.error('Ошибка загрузки:', err)
    });
  }
}