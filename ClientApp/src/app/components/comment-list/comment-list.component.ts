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
  templateUrl: './comment-list.component.html',
  styleUrl: './comment-list.component.scss'
})
export class CommentListComponent implements OnInit {
  public auth = inject(AuthService);
  private commentService = inject(CommentService);

  page = 1;
  pageSize = 25;
  sortBy = 'date';
  sortOrder: 'asc' | 'desc' = 'desc';
  totalCount = 0;
  
  comments: Comment[] = [];

  ngOnInit() {
    this.loadComments();
    this.commentService.onNewComment(() => {
      this.loadComments();
    });
  }

  changePage(newPage: number) {
    this.page = newPage;
    this.loadComments();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  changeSort(field: string) {
    if (this.sortBy === field) {
      this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = field;
      this.sortOrder = 'desc';
    }
    this.loadComments();
  }

  loadComments() {
    this.commentService.getComments(this.page, this.pageSize, this.sortBy, this.sortOrder)
      .subscribe({
        next: (res) => {
          this.comments = res.items || [];
          this.totalCount = res.totalCount || 0;
        },
        error: (err) => {
          console.error('Ошибка загрузки:', err);
          this.comments = []; 
        }
      });
  }
}