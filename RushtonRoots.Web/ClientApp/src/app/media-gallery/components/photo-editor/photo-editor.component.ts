import { Component, Input, Output, EventEmitter, OnInit, ViewChild, ElementRef } from '@angular/core';
import { MediaItem, PhotoFilter, PHOTO_FILTERS, PhotoEditState, CropData } from '../../models/media-gallery.model';

@Component({
  selector: 'app-photo-editor',
  standalone: false,
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.scss']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photo!: MediaItem;
  @Output() saveEdit = new EventEmitter<PhotoEditState>();
  @Output() cancel = new EventEmitter<void>();

  @ViewChild('canvas') canvas?: ElementRef<HTMLCanvasElement>;

  filters = PHOTO_FILTERS;
  selectedFilter: PhotoFilter = PHOTO_FILTERS[0];
  
  brightness = 100;
  contrast = 100;
  saturation = 100;
  rotation = 0;
  
  editState!: PhotoEditState;
  currentImageUrl: string = '';

  ngOnInit(): void {
    this.currentImageUrl = this.photo.url;
    this.editState = {
      originalUrl: this.photo.url,
      operations: [],
      currentUrl: this.photo.url,
      hasChanges: false
    };
  }

  applyFilter(filter: PhotoFilter): void {
    this.selectedFilter = filter;
    this.updatePreview();
  }

  adjustBrightness(value: number): void {
    this.brightness = value;
    this.updatePreview();
  }

  adjustContrast(value: number): void {
    this.contrast = value;
    this.updatePreview();
  }

  adjustSaturation(value: number): void {
    this.saturation = value;
    this.updatePreview();
  }

  rotate(degrees: number): void {
    this.rotation = (this.rotation + degrees) % 360;
    this.updatePreview();
  }

  private updatePreview(): void {
    this.editState.hasChanges = true;
  }

  getImageStyle(): any {
    let filters = this.selectedFilter.cssFilter;
    
    if (this.brightness !== 100) {
      filters += ` brightness(${this.brightness}%)`;
    }
    if (this.contrast !== 100) {
      filters += ` contrast(${this.contrast}%)`;
    }
    if (this.saturation !== 100) {
      filters += ` saturate(${this.saturation}%)`;
    }

    return {
      filter: filters,
      transform: `rotate(${this.rotation}deg)`
    };
  }

  resetAll(): void {
    this.selectedFilter = this.filters[0];
    this.brightness = 100;
    this.contrast = 100;
    this.saturation = 100;
    this.rotation = 0;
    this.editState.hasChanges = false;
  }

  save(): void {
    this.saveEdit.emit(this.editState);
  }

  onCancel(): void {
    if (!this.editState.hasChanges || confirm('Discard changes?')) {
      this.cancel.emit();
    }
  }
}
