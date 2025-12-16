import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { Album, AlbumFormData, MediaItem, AlbumActionEvent } from '../../models/media-gallery.model';

/**
 * AlbumManagerComponent - Create and manage photo albums
 * 
 * Features:
 * - Create new albums
 * - Edit album details
 * - Add/remove media from albums
 * - Set album cover photo
 * - Manage album privacy
 * - Share albums
 */
@Component({
  selector: 'app-album-manager',
  standalone: false,
  templateUrl: './album-manager.component.html',
  styleUrls: ['./album-manager.component.scss']
})
export class AlbumManagerComponent implements OnInit {
  @Input() albums: Album[] = [];
  @Input() mediaItems: MediaItem[] = [];
  @Input() canEdit = false;
  @Output() albumCreated = new EventEmitter<AlbumFormData>();
  @Output() albumUpdated = new EventEmitter<{ id: number; data: AlbumFormData }>();
  @Output() albumDeleted = new EventEmitter<number>();
  @Output() albumAction = new EventEmitter<AlbumActionEvent>();

  albumForm!: FormGroup;
  isCreatingAlbum = false;
  editingAlbumId?: number;
  selectedAlbum?: Album;
  viewMode: 'grid' | 'list' = 'grid';

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
    this.albumForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.maxLength(500)],
      isPrivate: [false],
      coverPhotoId: [null],
      tags: this.fb.array([]),
      sharedWith: this.fb.array([])
    });
  }

  get tagsArray(): FormArray {
    return this.albumForm.get('tags') as FormArray;
  }

  get sharedWithArray(): FormArray {
    return this.albumForm.get('sharedWith') as FormArray;
  }

  startCreating(): void {
    this.isCreatingAlbum = true;
    this.editingAlbumId = undefined;
    this.albumForm.reset({ isPrivate: false });
  }

  startEditing(album: Album): void {
    this.editingAlbumId = album.id;
    this.isCreatingAlbum = false;
    this.albumForm.patchValue({
      name: album.name,
      description: album.description,
      isPrivate: album.isPrivate
    });
  }

  cancelEditing(): void {
    this.isCreatingAlbum = false;
    this.editingAlbumId = undefined;
    this.albumForm.reset();
  }

  saveAlbum(): void {
    if (this.albumForm.invalid) return;

    const formData: AlbumFormData = this.albumForm.value;

    if (this.editingAlbumId) {
      this.albumUpdated.emit({ id: this.editingAlbumId, data: formData });
    } else {
      this.albumCreated.emit(formData);
    }

    this.cancelEditing();
  }

  deleteAlbum(album: Album): void {
    if (confirm(`Delete album "${album.name}"?`)) {
      this.albumDeleted.emit(album.id);
    }
  }

  viewAlbum(album: Album): void {
    this.selectedAlbum = album;
    this.albumAction.emit({ action: 'view', album });
  }

  shareAlbum(album: Album): void {
    this.albumAction.emit({ action: 'share', album });
  }

  formatDate(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }
}
