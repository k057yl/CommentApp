import { Component, Input, OnInit, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommentService } from '../../services/comment.service';
import { AuthService } from '../../services/auth.service';

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

  private fb = inject(FormBuilder);
  private commentService = inject(CommentService);
  private auth = inject(AuthService);

  commentForm!: FormGroup;
  captchaCode: string = '';
  selectedImage: File | null = null;
  selectedTextFile: File | null = null;
  isPreview: boolean = false;
  imageName: string = '';
  textFileName: string = '';

  ngOnInit() {
    this.commentForm = this.fb.group({
      userName: [{ value: '', disabled: true }, [Validators.required]],
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      text: ['', [Validators.required]],
      captcha: ['', [Validators.required]]
    });

    this.loadUserData();
    this.refreshCaptcha();
  }

  private loadUserData() {
    const name = this.auth.getUserName();
    if (name) {
      this.commentForm.patchValue({
        userName: name,
        email: `${name}@test.com`
      });
    }
  }

  refreshCaptcha() {
    this.commentService.getCaptcha().subscribe(res => {
      this.captchaCode = res.code;
    });
  }

  onFileSelected(event: any, type: 'image' | 'text') {
    const file = event.target.files[0];
    if (!file) return;

    if (type === 'image') {
      this.selectedImage = file;
      this.imageName = file.name;
    } else {
      this.selectedTextFile = file;
      this.textFileName = file.name;
    }
  }

  removeFile(type: 'image' | 'text') {
    if (type === 'image') {
      this.selectedImage = null;
      this.imageName = '';
    } else {
      this.selectedTextFile = null;
      this.textFileName = '';
    }
  }

  get previewContent() {
    return this.commentForm.get('text')?.value || '';
  }

  togglePreview() {
    this.isPreview = !this.isPreview;
  }

  onSubmit() {
    if (this.commentForm.invalid) return;

    const formData = new FormData();
    const formValues = this.commentForm.getRawValue();

    formData.append('Text', formValues.text);
    formData.append('Captcha', formValues.captcha);
    
    Object.keys(formValues).forEach(key => {
      formData.append(key, formValues[key]);
    });

    if (this.parentId) {
      formData.append('ParentId', this.parentId.toString());
    }

    if (this.selectedImage) {
      formData.append('Image', this.selectedImage);
    }

    if (this.selectedTextFile) {
      formData.append('TextFile', this.selectedTextFile);
    }

    this.commentService.createComment(formData).subscribe({
      next: () => {
        this.commentForm.patchValue({ text: '', captcha: '' });
        this.selectedImage = null;
        this.selectedTextFile = null;
        this.imageName = '';
        this.textFileName = '';

        const fileInputs = document.querySelectorAll('input[type="file"]') as NodeListOf<HTMLInputElement>;
        fileInputs.forEach(input => input.value = '');

        this.commentCreated.emit();
        this.refreshCaptcha();
        this.isPreview = false;
      },
      error: (err) => alert(err.error?.message || 'Ошибка сохранения')
    });
  }
}