/**
 * Media Gallery Models
 * TypeScript interfaces and types for media gallery functionality
 */

/**
 * Media type enumeration
 */
export enum MediaType {
  Photo = 'photo',
  Video = 'video',
  Document = 'document',
  Audio = 'audio'
}

/**
 * Media item representing a photo, video, or other media
 */
export interface MediaItem {
  id: number;
  type: MediaType;
  url: string;
  thumbnailUrl: string;
  title?: string;
  description?: string;
  uploadDate: Date | string;
  fileSize: number;
  fileName: string;
  mimeType: string;
  width?: number;
  height?: number;
  duration?: number; // For videos/audio in seconds
  tags: MediaTag[];
  albumIds: number[];
  personIds: number[];
  uploadedBy: string;
  uploadedByName: string;
  viewCount: number;
  isFavorite: boolean;
  metadata?: MediaMetadata;
}

/**
 * Media tag for organizing and searching media
 */
export interface MediaTag {
  id: number;
  name: string;
  color?: string;
  personId?: number; // If tag represents a person
  personName?: string;
  x?: number; // Position on image (percentage)
  y?: number; // Position on image (percentage)
}

/**
 * Album for organizing media items
 */
export interface Album {
  id: number;
  name: string;
  description?: string;
  coverPhotoUrl?: string;
  createdDate: Date | string;
  updatedDate: Date | string;
  mediaCount: number;
  isPrivate: boolean;
  createdBy: string;
  createdByName: string;
  tags: string[];
  sharedWith: string[];
}

/**
 * Media metadata (EXIF data, etc.)
 */
export interface MediaMetadata {
  camera?: string;
  lens?: string;
  iso?: number;
  aperture?: string;
  shutterSpeed?: string;
  focalLength?: string;
  dateTaken?: Date | string;
  location?: MediaLocation;
  copyright?: string;
  colorSpace?: string;
  orientation?: number;
}

/**
 * Media location (GPS coordinates)
 */
export interface MediaLocation {
  latitude: number;
  longitude: number;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
}

/**
 * Photo editing operations
 */
export interface PhotoEditOperation {
  type: 'crop' | 'rotate' | 'filter' | 'brightness' | 'contrast' | 'saturation';
  value: any;
}

/**
 * Photo edit state
 */
export interface PhotoEditState {
  originalUrl: string;
  operations: PhotoEditOperation[];
  currentUrl: string;
  hasChanges: boolean;
}

/**
 * Photo filter preset
 */
export interface PhotoFilter {
  id: string;
  name: string;
  description: string;
  thumbnailUrl?: string;
  cssFilter: string; // CSS filter string
  previewImage?: string;
}

/**
 * Crop data for image cropping
 */
export interface CropData {
  x: number;
  y: number;
  width: number;
  height: number;
  rotate?: number;
  scaleX?: number;
  scaleY?: number;
}

/**
 * Media search filters
 */
export interface MediaSearchFilters {
  searchText?: string;
  mediaType?: MediaType;
  albumId?: number;
  tags?: number[];
  personIds?: number[];
  dateFrom?: Date | string;
  dateTo?: Date | string;
  isFavorite?: boolean;
}

/**
 * Media sort options
 */
export interface MediaSortOption {
  label: string;
  value: string;
  field: keyof MediaItem;
  direction: 'asc' | 'desc';
}

/**
 * Predefined media sort options
 */
export const MEDIA_SORT_OPTIONS: MediaSortOption[] = [
  { label: 'Newest First', value: 'date-desc', field: 'uploadDate', direction: 'desc' },
  { label: 'Oldest First', value: 'date-asc', field: 'uploadDate', direction: 'asc' },
  { label: 'Title A-Z', value: 'title-asc', field: 'title', direction: 'asc' },
  { label: 'Title Z-A', value: 'title-desc', field: 'title', direction: 'desc' },
  { label: 'Most Viewed', value: 'views-desc', field: 'viewCount', direction: 'desc' },
  { label: 'File Size', value: 'size-desc', field: 'fileSize', direction: 'desc' }
];

/**
 * Photo filter presets
 */
export const PHOTO_FILTERS: PhotoFilter[] = [
  {
    id: 'none',
    name: 'None',
    description: 'Original photo',
    cssFilter: 'none'
  },
  {
    id: 'grayscale',
    name: 'Grayscale',
    description: 'Black and white',
    cssFilter: 'grayscale(100%)'
  },
  {
    id: 'sepia',
    name: 'Sepia',
    description: 'Vintage sepia tone',
    cssFilter: 'sepia(100%)'
  },
  {
    id: 'vintage',
    name: 'Vintage',
    description: 'Warm vintage look',
    cssFilter: 'sepia(50%) contrast(110%) brightness(105%)'
  },
  {
    id: 'bright',
    name: 'Bright',
    description: 'Increased brightness',
    cssFilter: 'brightness(120%) contrast(105%)'
  },
  {
    id: 'contrast',
    name: 'High Contrast',
    description: 'Enhanced contrast',
    cssFilter: 'contrast(130%)'
  },
  {
    id: 'cool',
    name: 'Cool',
    description: 'Cool blue tone',
    cssFilter: 'saturate(120%) hue-rotate(-10deg)'
  },
  {
    id: 'warm',
    name: 'Warm',
    description: 'Warm orange tone',
    cssFilter: 'saturate(130%) hue-rotate(10deg)'
  },
  {
    id: 'dramatic',
    name: 'Dramatic',
    description: 'Dark and moody',
    cssFilter: 'contrast(140%) brightness(95%) saturate(110%)'
  }
];

/**
 * Upload file info
 */
export interface UploadFile {
  file: File;
  preview: string;
  progress: number;
  status: 'pending' | 'uploading' | 'success' | 'error';
  error?: string;
  uploadedMediaId?: number;
}

/**
 * Album creation data
 */
export interface AlbumFormData {
  name: string;
  description?: string;
  isPrivate: boolean;
  coverPhotoId?: number;
  tags: string[];
  sharedWith: string[];
}

/**
 * Media gallery view mode
 */
export type GalleryViewMode = 'grid' | 'list' | 'masonry';

/**
 * Media action event
 */
export interface MediaActionEvent {
  action: 'view' | 'edit' | 'delete' | 'download' | 'share' | 'favorite';
  mediaItem: MediaItem;
}

/**
 * Album action event
 */
export interface AlbumActionEvent {
  action: 'view' | 'edit' | 'delete' | 'share';
  album: Album;
}
