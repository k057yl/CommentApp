import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommentService } from '../../services/comment.service';

@Component({
  selector: 'app-comment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './comment-form.component.html',
  styleUrl: './comment-form.component.scss'
})
export class CommentFormComponent implements OnInit {
  @Input() parentId: number | null = null;
  @Output() commentCreated = new EventEmitter<void>();

  commentForm!: FormGroup;
  captchaCode: string = '';
  selectedImage: File | null = null;
  selectedTextFile: File | null = null;
  isPreview: boolean = false;

  constructor(private fb: FormBuilder, private commentService: CommentService) {}

  ngOnInit() {
    this.commentForm = this.fb.group({
      userName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      homePage: [''],
      text: ['', [Validators.required]],
      captcha: ['', [Validators.required]]
    });

    this.refreshCaptcha();
  }

  refreshCaptcha() {
    this.commentService.getCaptcha().subscribe(res => {
      this.captchaCode = res.code;
    });
  }

  onFileSelected(event: any, type: 'image' | 'text') {
    const file = event.target.files[0];
    if (type === 'image') this.selectedImage = file;
    else this.selectedTextFile = file;
  }

  togglePreview() {
    this.isPreview = !this.isPreview;
  }

  onSubmit() {
    if (this.commentForm.invalid) return;

    const formData = new FormData();
    Object.keys(this.commentForm.value).forEach(key => {
      formData.append(key, this.commentForm.value[key]);
    });

    if (this.parentId) formData.append('parentId', this.parentId.toString());
    if (this.selectedImage) formData.append('image', this.selectedImage);
    if (this.selectedTextFile) formData.append('textFile', this.selectedTextFile);

    this.commentService.createComment(formData).subscribe({
      next: () => {
        this.commentForm.reset();
        this.commentCreated.emit();
        this.refreshCaptcha();
        this.isPreview = false;
      },
      error: (err) => alert(err.error?.message || 'Ошибка сохранения')
    });
  }
}