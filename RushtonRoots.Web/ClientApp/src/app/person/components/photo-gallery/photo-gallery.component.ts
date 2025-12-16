import { Component, Input, Output, EventEmitter, OnInit, ViewChild, ElementRef } from '@angular/core';
import { PersonPhoto } from '../../models/person-details.model';

/**
 * PhotoGalleryComponent - Displays and manages person photos
 * 
 * Features:
 * - Grid layout for photos
 * - Photo upload functionality
 * - Primary photo selection
 * - Photo preview/lightbox
 * - Photo deletion
 */
@Component({
  selector: 'app-photo-gallery',
  standalone: false,
  templateUrl: './photo-gallery.component.html',
  styleUrls: ['./photo-gallery.component.scss']
})
export class PhotoGalleryComponent implements OnInit {
  @Input() personId!: number;
  @Input() photos: PersonPhoto[] = [];
  @Input() canEdit = false;
  @Output() photoUploaded = new EventEmitter<File>();
  @Output() photoDeleted = new EventEmitter<number>();
  @Output() photoPrimaryChanged = new EventEmitter<number>();
  @Output() photoClicked = new EventEmitter<PersonPhoto>();

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  selectedFile: File | null = null;
  selectedPhotoIndex: number | null = null;
  isLightboxOpen = false;

  ngOnInit(): void {
    // Sort photos to show primary first
    this.sortPhotos();
  }

  ngOnChanges(): void {
    this.sortPhotos();
  }

  private sortPhotos(): void {
    this.photos.sort((a, b) => {
      if (a.isPrimary && !b.isPrimary) return -1;
      if (!a.isPrimary && b.isPrimary) return 1;
      return new Date(b.uploadDate).getTime() - new Date(a.uploadDate).getTime();
    });
  }

  triggerFileInput(): void {
    this.fileInput?.nativeElement.click();
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file && file.type.startsWith('image/')) {
      this.selectedFile = file;
      this.photoUploaded.emit(file);
      // Reset the input
      event.target.value = '';
    }
  }

  onDeletePhoto(photoId: number): void {
    if (confirm('Are you sure you want to delete this photo?')) {
      this.photoDeleted.emit(photoId);
    }
  }

  onSetPrimaryPhoto(photoId: number): void {
    this.photoPrimaryChanged.emit(photoId);
  }

  onPhotoClick(photo: PersonPhoto, index: number): void {
    this.selectedPhotoIndex = index;
    this.isLightboxOpen = true;
    this.photoClicked.emit(photo);
  }

  closeLightbox(): void {
    this.isLightboxOpen = false;
    this.selectedPhotoIndex = null;
  }

  previousPhoto(): void {
    if (this.selectedPhotoIndex !== null && this.selectedPhotoIndex > 0) {
      this.selectedPhotoIndex--;
    }
  }

  nextPhoto(): void {
    if (this.selectedPhotoIndex !== null && this.selectedPhotoIndex < this.photos.length - 1) {
      this.selectedPhotoIndex++;
    }
  }

  getCurrentPhoto(): PersonPhoto | null {
    if (this.selectedPhotoIndex !== null && this.photos[this.selectedPhotoIndex]) {
      return this.photos[this.selectedPhotoIndex];
    }
    return null;
  }

  canNavigatePrevious(): boolean {
    return this.selectedPhotoIndex !== null && this.selectedPhotoIndex > 0;
  }

  canNavigateNext(): boolean {
    return this.selectedPhotoIndex !== null && this.selectedPhotoIndex < this.photos.length - 1;
  }

  handleImageError(event: any): void {
    event.target.src = '/images/placeholder-photo.png';
  }

  formatDate(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  }
}
