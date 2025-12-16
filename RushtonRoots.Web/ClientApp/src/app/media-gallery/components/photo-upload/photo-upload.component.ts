import { Component, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { UploadFile } from '../../models/media-gallery.model';

@Component({
  selector: 'app-photo-upload',
  standalone: false,
  templateUrl: './photo-upload.component.html',
  styleUrls: ['./photo-upload.component.scss']
})
export class PhotoUploadComponent {
  @Output() filesSelected = new EventEmitter<File[]>();
  @Output() uploadComplete = new EventEmitter<UploadFile[]>();
  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  uploadFiles: UploadFile[] = [];
  isDragging = false;
  maxFileSize = 10 * 1024 * 1024; // 10MB

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    const files = event.dataTransfer?.files;
    if (files) {
      this.handleFiles(Array.from(files));
    }
  }

  onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.handleFiles(Array.from(input.files));
    }
  }

  triggerFileInput(): void {
    this.fileInput?.nativeElement.click();
  }

  private handleFiles(files: File[]): void {
    const validFiles = files.filter(file => 
      file.type.startsWith('image/') || file.type.startsWith('video/')
    );

    validFiles.forEach(file => {
      if (file.size > this.maxFileSize) {
        return;
      }

      const reader = new FileReader();
      reader.onload = (e) => {
        this.uploadFiles.push({
          file,
          preview: e.target?.result as string,
          progress: 0,
          status: 'pending'
        });
      };
      reader.readAsDataURL(file);
    });

    this.filesSelected.emit(validFiles);
  }

  removeFile(index: number): void {
    this.uploadFiles.splice(index, 1);
  }

  startUpload(): void {
    // Simulate upload
    this.uploadFiles.forEach((uploadFile, index) => {
      uploadFile.status = 'uploading';
      const interval = setInterval(() => {
        uploadFile.progress += 10;
        if (uploadFile.progress >= 100) {
          uploadFile.status = 'success';
          clearInterval(interval);
        }
      }, 200);
    });
  }

  clearAll(): void {
    this.uploadFiles = [];
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }
}
