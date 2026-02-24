import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Comment } from '../../models/comment.model';

@Component({
  selector: 'app-comment-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss'
})
export class CommentItemComponent {
  @Input({ required: true }) comment!: Comment;

  @Output() reply = new EventEmitter<Comment>();

  formatDate(date: string) {
    return new Date(date).toLocaleString();
  }

  onReply() {
    this.reply.emit(this.comment);
  }
}