import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime } from 'rxjs/operators';
import { 
  MediaItem, 
  Album, 
  MediaSearchFilters, 
  MediaSortOption, 
  MEDIA_SORT_OPTIONS, 
  GalleryViewMode,
  MediaActionEvent,
  MediaType
} from '../../models/media-gallery.model';

/**
 * MediaGalleryComponent - Main media gallery component
 * 
 * Features:
 * - Grid, list, and masonry view modes
 * - Infinite scroll for large collections
 * - Search and filter functionality
 * - Album organization
 * - Media actions (view, edit, delete, download, share)
 */
@Component({
  selector: 'app-media-gallery',
  standalone: false,
  templateUrl: './media-gallery.component.html',
  styleUrls: ['./media-gallery.component.scss']
})
export class MediaGalleryComponent implements OnInit, OnDestroy {
  @Input() mediaItems: MediaItem[] = [];
  @Input() albums: Album[] = [];
  @Input() canEdit = false;
  @Input() canDelete = false;
  @Input() personId?: number;
  @Output() mediaAction = new EventEmitter<MediaActionEvent>();
  @Output() uploadRequested = new EventEmitter<void>();
  @Output() loadMore = new EventEmitter<void>();

  private destroy$ = new Subject<void>();
  private searchSubject$ = new Subject<string>();

  // View state
  viewMode: GalleryViewMode = 'grid';
  selectedAlbumId?: number;
  filters: MediaSearchFilters = {};
  sortOption: MediaSortOption = MEDIA_SORT_OPTIONS[0];
  sortOptions = MEDIA_SORT_OPTIONS;
  
  // Infinite scroll
  isLoading = false;
  hasMore = true;
  pageSize = 24;
  currentPage = 1;

  // Filtered and sorted items
  displayedItems: MediaItem[] = [];
  
  // Search
  searchText = '';
  showFilters = false;

  // Selected items for batch operations
  selectedItems: Set<number> = new Set();
  selectionMode = false;

  // Expose Object for template use
  Object = Object;

  ngOnInit(): void {
    this.setupSearch();
    this.applyFiltersAndSort();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSearch(): void {
    this.searchSubject$
      .pipe(
        debounceTime(300),
        takeUntil(this.destroy$)
      )
      .subscribe(searchText => {
        this.filters.searchText = searchText;
        this.applyFiltersAndSort();
      });
  }

  onSearchChange(value: string): void {
    this.searchText = value;
    this.searchSubject$.next(value);
  }

  setViewMode(mode: GalleryViewMode): void {
    this.viewMode = mode;
  }

  setSortOption(option: MediaSortOption): void {
    this.sortOption = option;
    this.applyFiltersAndSort();
  }

  setAlbum(albumId?: number): void {
    this.selectedAlbumId = albumId;
    this.filters.albumId = albumId;
    this.applyFiltersAndSort();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  applyFilters(filters: MediaSearchFilters): void {
    this.filters = { ...this.filters, ...filters };
    this.applyFiltersAndSort();
  }

  clearFilters(): void {
    this.filters = {};
    this.searchText = '';
    this.selectedAlbumId = undefined;
    this.applyFiltersAndSort();
  }

  private applyFiltersAndSort(): void {
    let items = [...this.mediaItems];

    // Apply filters
    if (this.filters.searchText) {
      const search = this.filters.searchText.toLowerCase();
      items = items.filter(item => 
        item.title?.toLowerCase().includes(search) ||
        item.description?.toLowerCase().includes(search) ||
        item.fileName.toLowerCase().includes(search) ||
        item.tags.some(tag => tag.name.toLowerCase().includes(search))
      );
    }

    if (this.filters.mediaType) {
      items = items.filter(item => item.type === this.filters.mediaType);
    }

    if (this.filters.albumId) {
      items = items.filter(item => item.albumIds.includes(this.filters.albumId!));
    }

    if (this.filters.tags && this.filters.tags.length > 0) {
      items = items.filter(item => 
        item.tags.some(tag => this.filters.tags!.includes(tag.id))
      );
    }

    if (this.filters.personIds && this.filters.personIds.length > 0) {
      items = items.filter(item =>
        this.filters.personIds!.some(personId => item.personIds.includes(personId))
      );
    }

    if (this.filters.dateFrom) {
      const fromDate = new Date(this.filters.dateFrom);
      items = items.filter(item => new Date(item.uploadDate) >= fromDate);
    }

    if (this.filters.dateTo) {
      const toDate = new Date(this.filters.dateTo);
      items = items.filter(item => new Date(item.uploadDate) <= toDate);
    }

    if (this.filters.isFavorite !== undefined) {
      items = items.filter(item => item.isFavorite === this.filters.isFavorite);
    }

    // Apply sorting
    items.sort((a, b) => {
      const field = this.sortOption.field;
      const aVal = a[field];
      const bVal = b[field];

      if (aVal === undefined || bVal === undefined) return 0;

      let comparison = 0;
      if (typeof aVal === 'string' && typeof bVal === 'string') {
        comparison = aVal.localeCompare(bVal);
      } else if (aVal instanceof Date && bVal instanceof Date) {
        comparison = aVal.getTime() - bVal.getTime();
      } else if (typeof aVal === 'number' && typeof bVal === 'number') {
        comparison = aVal - bVal;
      }

      return this.sortOption.direction === 'asc' ? comparison : -comparison;
    });

    this.displayedItems = items;
    this.currentPage = 1;
  }

  onMediaClick(item: MediaItem): void {
    if (this.selectionMode) {
      this.toggleSelection(item.id);
    } else {
      this.mediaAction.emit({ action: 'view', mediaItem: item });
    }
  }

  onMediaAction(action: 'edit' | 'delete' | 'download' | 'share' | 'favorite', item: MediaItem): void {
    this.mediaAction.emit({ action, mediaItem: item });
  }

  onScroll(): void {
    if (!this.isLoading && this.hasMore) {
      this.loadMore.emit();
    }
  }

  toggleSelectionMode(): void {
    this.selectionMode = !this.selectionMode;
    if (!this.selectionMode) {
      this.selectedItems.clear();
    }
  }

  toggleSelection(itemId: number): void {
    if (this.selectedItems.has(itemId)) {
      this.selectedItems.delete(itemId);
    } else {
      this.selectedItems.add(itemId);
    }
  }

  selectAll(): void {
    this.displayedItems.forEach(item => this.selectedItems.add(item.id));
  }

  deselectAll(): void {
    this.selectedItems.clear();
  }

  isSelected(itemId: number): boolean {
    return this.selectedItems.has(itemId);
  }

  deleteSelected(): void {
    if (confirm(`Delete ${this.selectedItems.size} selected items?`)) {
      this.selectedItems.forEach(id => {
        const item = this.mediaItems.find(m => m.id === id);
        if (item) {
          this.onMediaAction('delete', item);
        }
      });
      this.selectedItems.clear();
      this.selectionMode = false;
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  formatDate(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }

  getMediaTypeIcon(type: MediaType): string {
    switch (type) {
      case MediaType.Photo: return 'photo';
      case MediaType.Video: return 'videocam';
      case MediaType.Audio: return 'audiotrack';
      case MediaType.Document: return 'description';
      default: return 'insert_drive_file';
    }
  }

  formatDuration(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = Math.floor(seconds % 60);

    if (hours > 0) {
      return `${hours}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    }
    return `${minutes}:${secs.toString().padStart(2, '0')}`;
  }
}
