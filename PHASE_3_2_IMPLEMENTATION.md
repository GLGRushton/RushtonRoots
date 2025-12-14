# Phase 3.2 Implementation: Document Management System

## Overview
This document describes the implementation of Phase 3.2 from the ROADMAP.md - a comprehensive document management system with categorization, version control, and person associations.

## Implementation Date
December 2025

## Features Implemented

### 1. Document Upload System
- **Document Upload**: Users can upload documents (PDFs, Word docs, etc.) with metadata
- **Document Storage**: Documents are stored in Azure Blob Storage (same infrastructure as photos)
- **Document Metadata**: Tracks title, description, category, file size, content type, document date
- **Document Display**: Display order management for document listings
- **User Ownership**: Documents track the uploading user

### 2. Document Categorization
- **Category System**: Documents can be categorized (birth certificates, wills, deeds, etc.)
- **Category Filtering**: Query documents by category
- **Flexible Categories**: Category is a string field allowing custom categories
- **Future Enhancement**: Could be converted to enum or lookup table for standardization

### 3. Document-to-Person Associations
- **DocumentPerson Entity**: Many-to-many relationship between documents and people
- **Multiple Associations**: A document can be associated with multiple people (e.g., marriage certificate)
- **Notes Field**: Optional notes to describe the association context
- **Bidirectional Navigation**: Find all documents for a person, or all people for a document

### 4. Document Search and Filtering
- **Title Search**: Search documents by title (contains)
- **Category Filter**: Filter by document category
- **Person Filter**: Find all documents associated with a specific person
- **Date Range Filter**: Filter by document date (from/to dates)
- **User Filter**: Filter by uploading user
- **Combined Filters**: All filters can be combined in a single search request

### 5. Document Preview Capability
- **SAS URL Generation**: Generate time-limited URLs for document preview
- **Secure Access**: Preview URLs expire after 1 hour by default
- **Blob Integration**: Uses existing BlobStorageService for secure access
- **Download Support**: URLs can be used for both preview and download

### 6. Version Control for Documents
- **DocumentVersion Entity**: Tracks all versions of a document
- **Version Numbers**: Sequential version numbering (1, 2, 3, etc.)
- **Change Notes**: Optional notes describing what changed in each version
- **Version History**: Full history of all document versions
- **Current Version**: Main document always points to the latest version
- **Version Retrieval**: Can retrieve any previous version by version ID

## Database Schema

### Document
```csharp
- Id: int (PK)
- Title: string (required)
- Description: string?
- DocumentUrl: string (Full URL to blob)
- ThumbnailUrl: string? (Future: thumbnail for preview)
- Category: string (required)
- BlobName: string? (Blob storage filename)
- FileSize: long (File size in bytes)
- ContentType: string? (MIME type)
- DocumentDate: DateTime? (Date relevant to the document)
- UploadedByUserId: string (FK to ApplicationUser)
- IsPublic: bool
- DisplayOrder: int
```

### DocumentVersion
```csharp
- Id: int (PK)
- DocumentId: int (FK to Document)
- DocumentUrl: string (Full URL to version blob)
- BlobName: string? (Blob storage filename for version)
- FileSize: long
- ContentType: string?
- VersionNumber: int (Sequential version number)
- ChangeNotes: string? (Description of changes)
- UploadedByUserId: string (FK to ApplicationUser)
- Unique Index: (DocumentId, VersionNumber)
```

### DocumentPerson
```csharp
- Id: int (PK)
- DocumentId: int (FK to Document)
- PersonId: int (FK to Person)
- Notes: string? (Context about the association)
- Unique Index: (DocumentId, PersonId)
```

### DocumentPermission
```csharp
- Id: int (PK)
- DocumentId: int (FK to Document)
- UserId: string? (FK to ApplicationUser)
- HouseholdId: int? (FK to Household)
- PermissionLevel: string (View, Edit, Delete)
```

## API Endpoints

### DocumentController
- `GET /api/document/{id}` - Get document by ID
- `GET /api/document` - Get all documents
- `GET /api/document/user/{userId}` - Get documents for a user
- `GET /api/document/my-documents` - Get current user's documents
- `GET /api/document/category/{category}` - Get documents by category
- `GET /api/document/person/{personId}` - Get documents associated with a person
- `POST /api/document/search` - Search documents with filters
- `POST /api/document/upload` - Upload a new document (multipart/form-data)
- `PUT /api/document/{id}` - Update document metadata
- `DELETE /api/document/{id}` - Delete a document and all versions
- `POST /api/document/{id}/version` - Upload a new version of a document
- `GET /api/document/{id}/versions` - Get all versions of a document
- `GET /api/document/{id}/preview` - Get time-limited preview URL

## Services Architecture

### Application Layer Services
1. **DocumentService**: Manages document CRUD operations, version control, and blob storage integration
   - Upload new documents with initial version
   - Update document metadata
   - Delete documents (including blob cleanup)
   - Upload new versions
   - Retrieve version history
   - Generate preview URLs
   - Search and filter documents

### Infrastructure Layer Services
1. **BlobStorageService**: Reused from Phase 3.1 for document storage
   - File upload/download
   - File deletion
   - SAS URL generation
   - No changes required - already supports all file types

### Repositories
1. **DocumentRepository**: Data access for all document-related entities
   - Document CRUD operations
   - Search and filtering
   - Version management
   - Person associations

### Mappers
1. **DocumentMapper**: Maps between entities and view models
   - Document entity ↔ DocumentViewModel
   - DocumentVersion entity ↔ DocumentVersionViewModel
   - CreateDocumentRequest → Document entity
   - UpdateDocumentRequest → Document entity updates

## Configuration

### appsettings.json
Uses the same Azure Blob Storage configuration as Phase 3.1:

```json
{
  "AzureBlobStorage": {
    "ConnectionString": "",
    "ContainerName": "rushtonroots-files"
  }
}
```

Documents are stored in the same container as photos but can be distinguished by:
- ContentType (e.g., application/pdf vs image/jpeg)
- File extension in blob name
- Separate database tables (Documents vs PersonPhotos)

## Dependency Injection

All services, repositories, and mappers follow naming conventions and are automatically registered via Autofac:
- `DocumentService` → registered as `IDocumentService`
- `DocumentRepository` → registered as `IDocumentRepository`
- `DocumentMapper` → registered as `IDocumentMapper`

No manual registration required in `AutofacModule.cs`.

## Migration

Database migration: `20251214100343_AddDocumentManagement`

Creates the following tables:
- Documents
- DocumentVersions
- DocumentPeople
- DocumentPermissions

To apply the migration:
```bash
dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
```

Or simply run the application (migrations run automatically on startup).

## Security Considerations

1. **Authentication**: All document endpoints require authentication (`[Authorize]` attribute)
2. **User Ownership**: Documents track the uploading user via `UploadedByUserId`
3. **Permission System**: DocumentPermission entity provides granular access control (framework in place)
4. **Blob Security**: Blobs are stored with `PublicAccessType.None` (private)
5. **SAS URLs**: Time-limited access via SAS URLs for secure preview/download
6. **Version Security**: All versions are tracked and associated with the uploading user
7. **Deletion Cleanup**: Deleting a document also removes all blobs (main file + all versions)

## Supported Document Types

The system supports any file type, but common document types include:
- **PDFs**: application/pdf
- **Word Documents**: application/vnd.openxmlformats-officedocument.wordprocessingml.document
- **Excel Spreadsheets**: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
- **Text Files**: text/plain
- **Images**: image/jpeg, image/png (can also be stored as documents, not just photos)

## Common Document Categories

Suggested categories for family documents:
- Birth Certificate
- Death Certificate
- Marriage Certificate
- Divorce Decree
- Will / Testament
- Deed / Property Document
- Military Records
- Census Records
- Immigration Papers
- Naturalization Papers
- Passport
- Diploma / Education
- Employment Records
- Medical Records
- Legal Documents
- Other

## Usage Examples

### Upload a New Document
```
POST /api/document/upload
Content-Type: multipart/form-data

Form Data:
- file: [PDF file]
- Title: "John Smith Birth Certificate"
- Description: "Original birth certificate from 1950"
- Category: "Birth Certificate"
- DocumentDate: 1950-03-15
- IsPublic: false
- AssociatedPeople: [123]
```

### Search Documents
```
POST /api/document/search
Content-Type: application/json

{
  "Category": "Birth Certificate",
  "FromDate": "1900-01-01",
  "ToDate": "2000-12-31",
  "PersonId": 123
}
```

### Upload a New Version
```
POST /api/document/456/version
Content-Type: multipart/form-data

Form Data:
- file: [Updated PDF file]
- changeNotes: "Added missing signatures"
```

### Get Preview URL
```
GET /api/document/456/preview

Response:
{
  "url": "https://account.blob.core.windows.net/container/blob?sastoken..."
}
```

## Future Enhancements

### Phase 3.3 & Beyond
1. **Thumbnail Generation**: Generate preview thumbnails for PDF documents
2. **OCR Integration**: Extract text from scanned documents for full-text search
3. **Document Templates**: Pre-defined templates for common document types
4. **Batch Upload**: Upload multiple documents at once
5. **Permission Enforcement**: Implement permission checking in services
6. **Document Annotations**: Add ability to annotate/comment on documents
7. **Digital Signatures**: Verify digital signatures on documents
8. **Document Scanning**: Mobile app integration for scanning documents
9. **AI Classification**: Automatically categorize documents using AI
10. **Document Linking**: Link related documents together

### Technical Debt
1. **Permission Checks**: Permission validation not yet enforced in services
2. **Thumbnail Generation**: Placeholder field exists but not implemented
3. **File Validation**: Should add file type and size validation
4. **Category Standardization**: Consider using enum or lookup table for categories
5. **Blob Cleanup**: Could implement orphaned blob cleanup mechanism
6. **Unit Tests**: Need comprehensive unit tests for all components

## Testing

### Manual Testing Checklist
- [x] Build solution successfully
- [ ] Run application and verify migration applied
- [ ] Upload a PDF document
- [ ] Upload a Word document
- [ ] Associate document with a person
- [ ] Search for documents by category
- [ ] Upload a new version of a document
- [ ] View version history
- [ ] Get preview URL and verify access
- [ ] Delete a document and verify blob cleanup

### Unit Tests
- **TODO**: Add unit tests for DocumentService
- **TODO**: Add unit tests for DocumentRepository
- **TODO**: Add unit tests for DocumentMapper
- **TODO**: Add unit tests for version control logic
- **TODO**: Add unit tests for search functionality

### Integration Tests
- **TODO**: Test document upload workflow end-to-end
- **TODO**: Test version control workflow
- **TODO**: Test document-person associations
- **TODO**: Test permission system
- **TODO**: Test blob cleanup on deletion

## Success Metrics

✅ **Completed**:
- Document upload and storage system
- Document categorization
- Document-to-person associations
- Search and filtering capabilities
- Version control system
- Preview URL generation
- RESTful API endpoints
- Database migrations
- Azure Blob Storage integration
- Auto-registration via Autofac

⏳ **Pending**:
- Frontend components (Angular UI)
- Thumbnail generation for documents
- Permission enforcement
- OCR/text extraction
- Unit and integration tests

## Comparison with Phase 3.1 (Photos)

### Similarities
- Both use Azure Blob Storage
- Both have permission framework (DocumentPermission vs PhotoPermission)
- Both track user ownership
- Both have categorization (PhotoAlbum vs Document Category)
- Both support metadata and display ordering
- Both use the same service/repository/mapper patterns

### Differences
- Documents have **version control** (photos do not)
- Documents have **category field** (photos use albums)
- Documents have **person associations** (photos have tags with positions)
- Documents focus on **file management** (photos focus on visual display)
- Documents support **any file type** (photos are images only)
- Documents have **preview URLs** (photos have direct display)

## Conclusion

Phase 3.2 has been successfully implemented with a comprehensive document management system. The backend infrastructure, database schema, API endpoints, and version control are complete. The system follows the same architectural patterns as Phase 3.1 and integrates seamlessly with the existing infrastructure.

**Key Achievements**:
- Full CRUD operations for documents
- Version control with change tracking
- Flexible categorization system
- Person associations for context
- Powerful search and filtering
- Secure preview/download via SAS URLs
- Clean architecture with separation of concerns
- Convention-based dependency injection

**Next Steps**: 
- Phase 3.3 (Video & Audio) or frontend development for Phase 3.2 features
- Add comprehensive unit and integration tests
- Implement permission enforcement
- Consider OCR/text extraction for searchability
