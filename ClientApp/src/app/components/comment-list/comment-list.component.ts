import { Component, OnInit } from '@angular/core';
import { CommentService } from '../../services/comment.service';
import { Comment } from '../../models/comment.model';
import { CommentItemComponent } from '../comment-item/comment-item.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-comment-list',
  standalone: true,
  imports: [CommonModule, CommentItemComponent],
  template: `
    <div class="container">
      <h2>Комментарии</h2>
      <app-comment-item 
        *ngFor="let comment of comments" 
        [comment]="comment">
      </app-comment-item>
    </div>
  `
})
export class CommentListComponent implements OnInit {
  comments: Comment[] = [];

  constructor(private commentService: CommentService) {}

  ngOnInit() {
    this.commentService.getComments().subscribe(res => {
      this.comments = res.items;
    });
  }
}