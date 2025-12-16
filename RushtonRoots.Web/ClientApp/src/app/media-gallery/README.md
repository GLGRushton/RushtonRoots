# Media Gallery Module

Comprehensive media gallery functionality for RushtonRoots application, including photo management, video playback, album organization, and advanced photo editing capabilities.

## Overview

The Media Gallery Module provides a complete media management solution with the following features:

- **Media Gallery**: Browse photos and videos with multiple view modes (grid, list, masonry)
- **Photo Lightbox**: Full-screen photo viewer with swipe gestures and zoom
- **Photo Tagging**: Tag people in photos with drag-and-drop positioning
- **Album Manager**: Create and organize media into albums
- **Photo Upload**: Drag-and-drop file upload with progress tracking
- **Photo Editor**: Edit photos with filters, adjustments, and rotation
- **Video Player**: Custom HTML5 video player with controls

## Components

### 1. MediaGalleryComponent

Main gallery component for displaying and managing media items.

**Features:**
- Grid, list, and masonry view modes
- Real-time search with debouncing
- Advanced filtering (media type, dates, favorites, albums)
- Multiple sorting options
- Infinite scroll support
- Selection mode for batch operations
- Responsive mobile design

**Usage in TypeScript:**
```typescript
import { MediaItem, Album } from './models/media-gallery.model';

mediaItems: MediaItem[] = [...];
albums: Album[] = [...];

onMediaAction(event: MediaActionEvent) {
  console.log('Action:', event.action, 'Media:', event.mediaItem);
}
```

**Usage in Razor View:**
```html
<app-media-gallery
  media-items='@Json.Serialize(Model.MediaItems)'
  albums='@Json.Serialize(Model.Albums)'
  can-edit="true"
  can-delete="true">
</app-media-gallery>
```

### 2. PhotoLightboxComponent

Enhanced full-screen photo viewer with advanced navigation.

**Features:**
- Swipe gestures for navigation (touch-enabled)
- Keyboard navigation (arrows, ESC, +/-, 0, I)
- Zoom in/out with mouse wheel or buttons
- Pan support when zoomed
- Photo information panel with expandable metadata
- EXIF metadata display
- Auto-hiding controls

**Keyboard Shortcuts:**
- `←` / `→` - Previous/Next photo
- `ESC` - Close lightbox
- `+` / `-` - Zoom in/out
- `0` - Reset zoom
- `I` - Toggle metadata

**Usage:**
```html
<app-photo-lightbox
  [photos]="photos"
  [initialIndex]="0"
  [showInfo]="true"
  (close)="onCloseLightbox()"
  (photoChanged)="onPhotoChanged($event)">
</app-photo-lightbox>
```

### 3. PhotoTaggingComponent

Interactive interface for tagging people in photos.

**Features:**
- Click-to-tag interface on photos
- Drag-and-drop tag repositioning
- Person autocomplete search
- Tag list sidebar
- Visual tag markers
- Face position tracking (x, y coordinates)

**Usage:**
```html
<app-photo-tagging
  [photo]="selectedPhoto"
  [availablePeople]="people"
  (tagAdded)="onTagAdded($event)"
  (tagRemoved)="onTagRemoved($event)"
  (close)="onCloseTagging()">
</app-photo-tagging>
```

### 4. AlbumManagerComponent

Create and manage photo albums.

**Features:**
- Grid and list view modes
- Create and edit album forms
- Album privacy settings
- Cover photo selection
- Album metadata management
- Photo count badges

**Usage:**
```html
<app-album-manager
  [albums]="albums"
  [mediaItems]="mediaItems"
  [canEdit]="true"
  (albumCreated)="onAlbumCreated($event)"
  (albumUpdated)="onAlbumUpdated($event)"
  (albumDeleted)="onAlbumDeleted($event)">
</app-album-manager>
```

### 5. PhotoUploadComponent

Drag-and-drop file upload with progress tracking.

**Features:**
- Drag-and-drop upload area
- Click to browse files
- Multiple file selection
- File validation (type and size)
- Preview thumbnails
- Upload progress tracking
- Batch upload support

**Usage:**
```html
<app-photo-upload
  (filesSelected)="onFilesSelected($event)"
  (uploadComplete)="onUploadComplete($event)">
</app-photo-upload>
```

### 6. PhotoEditorComponent

Photo editing tools with filters and adjustments.

**Features:**
- 9 filter presets (None, Grayscale, Sepia, Vintage, Bright, High Contrast, Cool, Warm, Dramatic)
- Adjustments: Brightness (0-200%), Contrast (0-200%), Saturation (0-200%)
- Rotation controls (90° increments)
- Real-time preview
- Reset functionality

**Usage:**
```html
<app-photo-editor
  [photo]="selectedPhoto"
  (saveEdit)="onSaveEdit($event)"
  (cancel)="onCancelEdit()">
</app-photo-editor>
```

### 7. VideoPlayerComponent

Custom HTML5 video player with controls.

**Features:**
- Play/pause functionality
- Progress slider with seek
- Volume control with mute
- Fullscreen mode
- Time display
- Custom styling

**Usage:**
```html
<app-video-player
  [video]="selectedVideo">
</app-video-player>
```

## Models

### MediaItem
```typescript
interface MediaItem {
  id: number;
  type: MediaType; // 'photo' | 'video' | 'document' | 'audio'
  url: string;
  thumbnailUrl: string;
  title?: string;
  description?: string;
  uploadDate: Date | string;
  fileSize: number;
  fileName: string;
  tags: MediaTag[];
  albumIds: number[];
  personIds: number[];
  metadata?: MediaMetadata;
}
```

### Album
```typescript
interface Album {
  id: number;
  name: string;
  description?: string;
  coverPhotoUrl?: string;
  createdDate: Date | string;
  mediaCount: number;
  isPrivate: boolean;
  tags: string[];
  sharedWith: string[];
}
```

### MediaTag
```typescript
interface MediaTag {
  id: number;
  name: string;
  color?: string;
  personId?: number;
  personName?: string;
  x?: number; // Position on image (percentage)
  y?: number; // Position on image (percentage)
}
```

## Installation

The module is already configured in the Angular application. Required dependencies:

```bash
npm install hammerjs ngx-image-cropper cropperjs @types/cropperjs
```

## Usage Examples

### Basic Media Gallery

```typescript
// Component
export class MediaPageComponent {
  mediaItems: MediaItem[] = [
    {
      id: 1,
      type: MediaType.Photo,
      url: '/media/photo1.jpg',
      thumbnailUrl: '/media/photo1-thumb.jpg',
      title: 'Family Reunion 2024',
      uploadDate: new Date(),
      fileSize: 2048000,
      fileName: 'reunion.jpg',
      tags: [],
      albumIds: [1],
      personIds: [101, 102],
      viewCount: 15,
      isFavorite: true
    }
  ];

  onMediaAction(event: MediaActionEvent) {
    switch (event.action) {
      case 'view':
        this.openLightbox(event.mediaItem);
        break;
      case 'edit':
        this.openEditor(event.mediaItem);
        break;
      case 'delete':
        this.deleteMedia(event.mediaItem);
        break;
    }
  }
}
```

### Photo Tagging Workflow

```typescript
// 1. Open photo tagging dialog
openPhotoTagging(photo: MediaItem) {
  this.selectedPhoto = photo;
  this.showTaggingDialog = true;
}

// 2. Handle tag added
onTagAdded(tag: MediaTag) {
  // Save tag to backend
  this.mediaService.addTag(this.selectedPhoto.id, tag).subscribe();
}

// 3. Handle tag removed
onTagRemoved(tagId: number) {
  // Remove tag from backend
  this.mediaService.removeTag(tagId).subscribe();
}
```

### Album Management

```typescript
// Create new album
onAlbumCreated(formData: AlbumFormData) {
  this.albumService.create(formData).subscribe(album => {
    this.albums.push(album);
  });
}

// Update album
onAlbumUpdated(event: { id: number; data: AlbumFormData }) {
  this.albumService.update(event.id, event.data).subscribe();
}
```

## Styling

All components use Material Design components and follow the RushtonRoots theme with green color scheme (#2e7d32).

Components are fully responsive with mobile-first design approach.

## Angular Elements Registration

All components are registered as Angular Elements for use in Razor views:

- `<app-media-gallery>`
- `<app-photo-lightbox>`
- `<app-photo-tagging>`
- `<app-album-manager>`
- `<app-photo-upload>`
- `<app-photo-editor>`
- `<app-video-player>`

## Browser Support

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)

Touch gestures work on all touch-enabled devices.

## Performance Considerations

- **Infinite Scroll**: Loads 24 items at a time to maintain performance
- **Debouncing**: Search input debounced to 300ms
- **Lazy Loading**: Images loaded on demand
- **Thumbnail URLs**: Separate thumbnail URLs for grid views

## Security

- File type validation on upload
- File size limits (10MB default)
- XSS protection through Angular sanitization
- Privacy controls for albums

## Future Enhancements

- Server-side infinite scroll pagination
- Image optimization and thumbnail generation
- Cloud storage integration
- Advanced photo editing (crop area selection)
- Face detection for auto-tagging
- Bulk operations (move to album, delete multiple)
- Social sharing integration

## License

Part of the RushtonRoots application.
