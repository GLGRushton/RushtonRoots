import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, HostListener, ViewChild, ElementRef } from '@angular/core';
import { MediaItem, MediaTag } from '../../models/media-gallery.model';

/**
 * PhotoLightboxComponent - Enhanced photo lightbox with swipe gestures
 * 
 * Features:
 * - Full-screen photo viewing
 * - Swipe gestures for navigation
 * - Keyboard navigation (arrow keys, ESC)
 * - Zoom and pan
 * - Photo information display
 * - Tag display and navigation
 */
@Component({
  selector: 'app-photo-lightbox',
  standalone: false,
  templateUrl: './photo-lightbox.component.html',
  styleUrls: ['./photo-lightbox.component.scss']
})
export class PhotoLightboxComponent implements OnInit, OnDestroy {
  @Input() photos: MediaItem[] = [];
  @Input() initialIndex = 0;
  @Input() showInfo = true;
  @Output() close = new EventEmitter<void>();
  @Output() photoChanged = new EventEmitter<MediaItem>();
  @Output() tagClicked = new EventEmitter<MediaTag>();
  @Output() deleteRequested = new EventEmitter<MediaItem>();
  @Output() editRequested = new EventEmitter<MediaItem>();

  @ViewChild('imageContainer') imageContainer?: ElementRef<HTMLDivElement>;

  currentIndex = 0;
  currentPhoto?: MediaItem;
  showControls = true;
  showMetadata = false;
  
  // Zoom
  zoomLevel = 1;
  minZoom = 1;
  maxZoom = 5;
  panX = 0;
  panY = 0;
  
  // Touch/Swipe
  private touchStartX = 0;
  private touchStartY = 0;
  private touchEndX = 0;
  private touchEndY = 0;
  private isDragging = false;
  private dragStartX = 0;
  private dragStartY = 0;

  // Auto-hide controls timer
  private controlsTimer?: any;

  ngOnInit(): void {
    this.currentIndex = this.initialIndex;
    this.updateCurrentPhoto();
    this.resetControlsTimer();
  }

  ngOnDestroy(): void {
    this.clearControlsTimer();
  }

  private updateCurrentPhoto(): void {
    if (this.currentIndex >= 0 && this.currentIndex < this.photos.length) {
      this.currentPhoto = this.photos[this.currentIndex];
      this.photoChanged.emit(this.currentPhoto);
      this.resetZoom();
    }
  }

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent): void {
    switch (event.key) {
      case 'Escape':
        this.onClose();
        break;
      case 'ArrowLeft':
        this.previous();
        event.preventDefault();
        break;
      case 'ArrowRight':
        this.next();
        event.preventDefault();
        break;
      case '+':
      case '=':
        this.zoomIn();
        event.preventDefault();
        break;
      case '-':
      case '_':
        this.zoomOut();
        event.preventDefault();
        break;
      case '0':
        this.resetZoom();
        event.preventDefault();
        break;
      case 'i':
      case 'I':
        this.toggleMetadata();
        event.preventDefault();
        break;
    }
  }

  @HostListener('touchstart', ['$event'])
  onTouchStart(event: TouchEvent): void {
    if (event.touches.length === 1) {
      this.touchStartX = event.touches[0].clientX;
      this.touchStartY = event.touches[0].clientY;
    }
    this.resetControlsTimer();
  }

  @HostListener('touchmove', ['$event'])
  onTouchMove(event: TouchEvent): void {
    if (event.touches.length === 1) {
      this.touchEndX = event.touches[0].clientX;
      this.touchEndY = event.touches[0].clientY;
    }
  }

  @HostListener('touchend', ['$event'])
  onTouchEnd(): void {
    this.handleSwipe();
  }

  @HostListener('mousemove')
  onMouseMove(): void {
    this.showControls = true;
    this.resetControlsTimer();
  }

  private handleSwipe(): void {
    const deltaX = this.touchEndX - this.touchStartX;
    const deltaY = this.touchEndY - this.touchStartY;
    const minSwipeDistance = 50;

    if (Math.abs(deltaX) > Math.abs(deltaY) && Math.abs(deltaX) > minSwipeDistance) {
      if (deltaX > 0) {
        this.previous();
      } else {
        this.next();
      }
    } else if (Math.abs(deltaY) > minSwipeDistance && deltaY < 0) {
      // Swipe up - could show info
      this.toggleMetadata();
    }
  }

  onMouseDown(event: MouseEvent): void {
    if (this.zoomLevel > 1) {
      this.isDragging = true;
      this.dragStartX = event.clientX - this.panX;
      this.dragStartY = event.clientY - this.panY;
      event.preventDefault();
    }
  }

  @HostListener('document:mousemove', ['$event'])
  onMouseMoveWhileDragging(event: MouseEvent): void {
    if (this.isDragging && this.zoomLevel > 1) {
      this.panX = event.clientX - this.dragStartX;
      this.panY = event.clientY - this.dragStartY;
    }
  }

  @HostListener('document:mouseup')
  onMouseUp(): void {
    this.isDragging = false;
  }

  @HostListener('wheel', ['$event'])
  onWheel(event: WheelEvent): void {
    event.preventDefault();
    if (event.deltaY < 0) {
      this.zoomIn();
    } else {
      this.zoomOut();
    }
  }

  next(): void {
    if (this.canGoNext()) {
      this.currentIndex++;
      this.updateCurrentPhoto();
    }
  }

  previous(): void {
    if (this.canGoPrevious()) {
      this.currentIndex--;
      this.updateCurrentPhoto();
    }
  }

  canGoNext(): boolean {
    return this.currentIndex < this.photos.length - 1;
  }

  canGoPrevious(): boolean {
    return this.currentIndex > 0;
  }

  zoomIn(): void {
    if (this.zoomLevel < this.maxZoom) {
      this.zoomLevel = Math.min(this.zoomLevel + 0.5, this.maxZoom);
    }
  }

  zoomOut(): void {
    if (this.zoomLevel > this.minZoom) {
      this.zoomLevel = Math.max(this.zoomLevel - 0.5, this.minZoom);
      if (this.zoomLevel === this.minZoom) {
        this.panX = 0;
        this.panY = 0;
      }
    }
  }

  resetZoom(): void {
    this.zoomLevel = 1;
    this.panX = 0;
    this.panY = 0;
  }

  toggleMetadata(): void {
    this.showMetadata = !this.showMetadata;
  }

  onClose(): void {
    this.close.emit();
  }

  onDelete(): void {
    if (this.currentPhoto && confirm('Are you sure you want to delete this photo?')) {
      this.deleteRequested.emit(this.currentPhoto);
    }
  }

  onEdit(): void {
    if (this.currentPhoto) {
      this.editRequested.emit(this.currentPhoto);
    }
  }

  onTagClick(tag: MediaTag, event: Event): void {
    event.stopPropagation();
    this.tagClicked.emit(tag);
  }

  getImageTransform(): string {
    return `scale(${this.zoomLevel}) translate(${this.panX / this.zoomLevel}px, ${this.panY / this.zoomLevel}px)`;
  }

  private resetControlsTimer(): void {
    this.clearControlsTimer();
    this.showControls = true;
    this.controlsTimer = setTimeout(() => {
      this.showControls = false;
    }, 3000);
  }

  private clearControlsTimer(): void {
    if (this.controlsTimer) {
      clearTimeout(this.controlsTimer);
      this.controlsTimer = undefined;
    }
  }

  formatDate(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  getDimensions(): string {
    if (this.currentPhoto?.width && this.currentPhoto?.height) {
      return `${this.currentPhoto.width} × ${this.currentPhoto.height}`;
    }
    return '';
  }
}
