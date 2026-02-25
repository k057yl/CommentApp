import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Comment } from '../../models/comment.model';
import { CommentFormComponent } from '../comment-form/comment-form.component';

@Component({
  selector: 'app-comment-item',
  standalone: true,
  imports: [CommonModule, CommentFormComponent],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss'
})
export class CommentItemComponent {
  @Input() comment!: Comment;
  @Output() commentCreated = new EventEmitter<void>();

  isReplying = false;

  toggleReply() {
    this.isReplying = !this.isReplying;
  }

  onReplyCreated() {
    this.isReplying = false;
    this.commentCreated.emit();
  }
}