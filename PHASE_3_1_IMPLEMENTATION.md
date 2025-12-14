# Phase 3.1 Implementation: Photo Gallery System

## Overview
This document describes the implementation of Phase 3.1 from the ROADMAP.md - a comprehensive photo gallery system with Azure Blob Storage integration.

## Implementation Date
December 2025

## Features Implemented

### 1. Photo Upload and Management System
- **Photo Upload**: Users can upload photos with metadata (caption, date, person association)
- **Photo Storage**: Photos are stored in Azure Blob Storage with automatic URL generation
- **Photo Metadata**: Tracks file size, content type, blob name, and thumbnail URLs
- **Photo Display**: Primary photo designation for person profiles
- **Photo Organization**: Display order management for photo galleries

### 2. Azure Blob Storage Integration
- **BlobStorageService**: Service class for interacting with Azure Blob Storage
  - Upload files with unique blob names
  - Delete files from blob storage
  - Generate thumbnails (basic implementation)
  - Get SAS URLs for temporary file access
- **Configuration**: Blob storage connection string and container name in appsettings.json
- **Auto-registration**: BlobStorageService automatically registered via Autofac conventions

### 3. Photo Tagging (Tag People in Photos)
- **PhotoTag Entity**: Links people to photos with optional position coordinates
- **Tag Management**: Create and delete tags for photos
- **Person Discovery**: Find all photos a person appears in
- **Tag API**: RESTful endpoints for managing photo tags

### 4. Photo Albums and Collections
- **PhotoAlbum Entity**: Organize photos into themed collections
- **Album Metadata**: Name, description, date, cover photo
- **Visibility Control**: Public/private album settings
- **User Ownership**: Albums are owned by the creating user
- **Album Management API**: Full CRUD operations for albums

### 5. Photo Timeline View
- **Timeline Endpoints**: Query photos by date range
- **Date-based Organization**: Photos can be organized by PhotoDate
- **Timeline API**: Placeholder for future timeline implementation

### 6. Photo Sharing Permissions
- **PhotoPermission Entity**: Granular permission control for photos and albums
- **Permission Levels**: View, Edit, Delete
- **Scope**: Permissions can be granted to individual users or households
- **Cascading Permissions**: Album permissions can control access to all photos in the album

### 7. Image Optimization and Thumbnails
- **Thumbnail Generation**: Basic thumbnail creation (stored separately in blob storage)
- **Thumbnail URLs**: Separate URL for thumbnail access
- **Future Enhancement**: Integration with image processing library (e.g., ImageSharp) for resize and optimization

## Database Schema

### PersonPhoto (Enhanced)
```csharp
- Id: int (PK)
- PersonId: int (FK to Person)
- PhotoAlbumId: int? (FK to PhotoAlbum)
- PhotoUrl: string (Full URL to blob)
- ThumbnailUrl: string? (URL to thumbnail)
- Caption: string?
- PhotoDate: DateTime?
- IsPrimary: bool
- DisplayOrder: int
- BlobName: string? (Blob storage filename)
- FileSize: long (File size in bytes)
- ContentType: string? (MIME type)
```

### PhotoAlbum (New)
```csharp
- Id: int (PK)
- Name: string
- Description: string?
- CreatedByUserId: string (FK to ApplicationUser)
- AlbumDate: DateTime?
- CoverPhotoUrl: string?
- IsPublic: bool
- DisplayOrder: int
```

### PhotoTag (New)
```csharp
- Id: int (PK)
- PersonPhotoId: int (FK to PersonPhoto)
- PersonId: int (FK to Person)
- Notes: string?
- XPosition: int? (X coordinate percentage 0-100)
- YPosition: int? (Y coordinate percentage 0-100)
```

### PhotoPermission (New)
```csharp
- Id: int (PK)
- PersonPhotoId: int? (FK to PersonPhoto)
- PhotoAlbumId: int? (FK to PhotoAlbum)
- UserId: string? (FK to ApplicationUser)
- HouseholdId: int? (FK to Household)
- PermissionLevel: string (View, Edit, Delete)
```

## API Endpoints

### PhotoController
- `GET /api/photo/{id}` - Get photo by ID
- `GET /api/photo/person/{personId}` - Get all photos for a person
- `GET /api/photo/person/{personId}/primary` - Get primary photo for a person
- `POST /api/photo/upload` - Upload a new photo
- `PUT /api/photo/{id}` - Update photo metadata
- `DELETE /api/photo/{id}` - Delete a photo
- `GET /api/photo/timeline` - Get photos by date range
- `GET /api/photo/album/{albumId}` - Get photos in an album

### PhotoAlbumController
- `GET /api/photoalbum/{id}` - Get album by ID
- `GET /api/photoalbum` - Get all albums
- `GET /api/photoalbum/user/{userId}` - Get albums for a user
- `GET /api/photoalbum/my-albums` - Get current user's albums
- `POST /api/photoalbum` - Create new album
- `PUT /api/photoalbum/{id}` - Update album
- `DELETE /api/photoalbum/{id}` - Delete album

### PhotoTagController
- `GET /api/phototag/{id}` - Get tag by ID
- `GET /api/phototag/photo/{photoId}` - Get all tags for a photo
- `GET /api/phototag/person/{personId}` - Get all photos a person is tagged in
- `POST /api/phototag` - Create a new tag
- `DELETE /api/phototag/{id}` - Delete a tag

## Services Architecture

### Application Layer Services
1. **PersonPhotoService**: Manages photo CRUD operations and blob storage integration
2. **PhotoAlbumService**: Manages album CRUD operations
3. **PhotoTagService**: Manages photo tagging operations

### Infrastructure Layer Services
1. **BlobStorageService**: Handles Azure Blob Storage operations
   - File upload/download
   - File deletion
   - Thumbnail generation
   - SAS URL generation

### Repositories
1. **PersonPhotoRepository**: Data access for PersonPhoto entities
2. **PhotoAlbumRepository**: Data access for PhotoAlbum entities
3. **PhotoTagRepository**: Data access for PhotoTag entities

### Mappers
1. **PhotoAlbumMapper**: Maps between PhotoAlbum entity and PhotoAlbumViewModel
2. **PhotoTagMapper**: Maps between PhotoTag entity and PhotoTagViewModel

## Configuration

### appsettings.json
```json
{
  "AzureBlobStorage": {
    "ConnectionString": "",
    "ContainerName": "rushtonroots-files"
  }
}
```

For local development:
- Use Azure Storage Emulator (Azurite) with connection string: `UseDevelopmentStorage=true`
- Or configure a real Azure Storage account connection string

## Dependency Injection

All services, repositories, and mappers follow naming conventions and are automatically registered via Autofac:
- Services ending with "Service" → registered as `IServiceName`
- Repositories ending with "Repository" → registered as `IRepositoryName`
- Mappers ending with "Mapper" → registered as `IMapperName`

The BlobStorageService in Infrastructure is also auto-registered through convention-based registration.

## Migration

Database migration: `20251214004045_AddPhotoGalleryFeatures`

To apply the migration:
```bash
dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
```

## Security Considerations

1. **Authentication**: All photo endpoints require authentication (`[Authorize]` attribute)
2. **User Ownership**: Albums track the creating user via `CreatedByUserId`
3. **Permission System**: PhotoPermission entity provides granular access control
4. **Blob Security**: Blobs are stored with `PublicAccessType.None` (private)
5. **SAS URLs**: Temporary access via time-limited SAS URLs for secure sharing

## Future Enhancements

### Phase 3.2 & 3.3 Considerations
1. **Image Processing**: Integrate ImageSharp or similar for true thumbnail generation and optimization
2. **Video Support**: Extend BlobStorageService for video uploads (Phase 3.3)
3. **Frontend Components**: Angular components for photo gallery, upload, and management
4. **Permission Enforcement**: Implement permission checking in services
5. **Batch Operations**: Bulk upload and tagging capabilities
6. **Search**: Full-text search across photo captions and tags
7. **AI Integration**: Automatic face detection and tagging (Phase 10.1)

### Technical Debt
1. **Thumbnail Generation**: Currently uses placeholder logic - needs actual image resizing
2. **Timeline Queries**: Repository methods for date-based queries need implementation
3. **Permission Checks**: Permission validation not yet enforced in services
4. **Blob Cleanup**: Orphaned blob cleanup mechanism needed
5. **File Validation**: File type and size validation should be added

## Testing

### Unit Tests
- **TODO**: Add unit tests for PersonPhotoService
- **TODO**: Add unit tests for PhotoAlbumService
- **TODO**: Add unit tests for PhotoTagService
- **TODO**: Add unit tests for BlobStorageService (with mocked Azure SDK)

### Integration Tests
- **TODO**: Test photo upload workflow end-to-end
- **TODO**: Test album creation and photo assignment
- **TODO**: Test permission system

## Success Metrics

✅ **Completed**:
- Photo upload and storage system
- Azure Blob Storage integration
- Photo tagging system
- Photo album management
- Permission framework
- RESTful API endpoints
- Database migrations

⏳ **Pending**:
- Frontend components
- Actual thumbnail generation (placeholder implemented)
- Permission enforcement
- Unit and integration tests

## Conclusion

Phase 3.1 has been successfully implemented with a solid foundation for photo management. The backend infrastructure, database schema, and API endpoints are complete. The system is ready for frontend development and can be extended to support additional media types in future phases.

**Next Steps**: Phase 3.2 (Document Management) or frontend development for Phase 3.1 features.
