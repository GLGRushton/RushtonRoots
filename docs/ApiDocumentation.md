# API Documentation - RushtonRoots

**Last Updated:** December 2025  
**Version:** 1.0

This document provides comprehensive API documentation for the RushtonRoots platform.

---

## Table of Contents

1. [Accessing API Documentation](#accessing-api-documentation)
2. [Authentication](#authentication)
3. [API Overview](#api-overview)
4. [Core API Controllers](#core-api-controllers)
5. [Common Response Codes](#common-response-codes)
6. [Error Handling](#error-handling)
7. [Rate Limiting](#rate-limiting)
8. [Versioning](#versioning)

---

## Accessing API Documentation

### Swagger UI (Development Only)

When running the application in **Development** mode, interactive API documentation is available via Swagger UI:

**URL:** `https://localhost:5001/api-docs`

**Features:**
- Interactive API exploration
- Test API endpoints directly from browser
- View request/response schemas
- Authentication support
- Example requests and responses

### OpenAPI Specification

The raw OpenAPI (Swagger) specification can be accessed at:

**URL:** `https://localhost:5001/swagger/v1/swagger.json`

This JSON file can be imported into:
- Postman
- Insomnia
- API testing tools
- Code generators

### Production Environments

For security reasons, Swagger UI is **disabled in production**. Use the OpenAPI specification file for integration or contact the development team for API access.

---

## Authentication

RushtonRoots uses **ASP.NET Core Identity** with **Cookie-based authentication** for web clients.

### Authentication Flow

1. **Login**
   ```
   POST /Account/Login
   Content-Type: application/x-www-form-urlencoded
   
   Email=user@example.com&Password=SecurePassword123
   ```

2. **Subsequent Requests**
   - Cookie is automatically sent with requests
   - No additional headers needed

### Authorization Levels

- **Public** - No authentication required
- **Authenticated** - Requires valid login
- **HouseholdAdmin** - Admin of at least one household
- **Admin** - System administrator

### Example: Authenticated Request

```http
GET /api/person/1
Cookie: .AspNetCore.Identity.Application=<cookie-value>
```

---

## API Overview

### Base URL

**Development:** `https://localhost:5001`  
**Production:** `https://your-domain.com`

### Content Type

All API endpoints accept and return JSON unless otherwise specified.

```http
Content-Type: application/json
Accept: application/json
```

### API Endpoints Summary

RushtonRoots exposes **31 API controllers** covering the following areas:

| Category | Controllers | Endpoints |
|----------|-------------|-----------|
| **Core Genealogy** | Person, Household, Partnership, ParentChild | 40+ |
| **Media & Documents** | Photo, PhotoAlbum, Document, Media | 25+ |
| **Collaboration** | Message, ChatRoom, Comment, Notification | 30+ |
| **Content** | Story, Recipe, Tradition, WikiPage | 35+ |
| **Events & Calendar** | FamilyEvent, LifeEvent, EventRsvp | 20+ |
| **Search & Discovery** | SearchApi, FamilyTree, Location | 15+ |
| **Gamification** | Contribution, ActivityFeed, Leaderboard | 12+ |
| **Total** | **31 Controllers** | **180+ Endpoints** |

---

## Core API Controllers

### Person API

**Base Route:** `/api/person`

#### Endpoints

```http
GET    /api/person                # List all persons
GET    /api/person/{id}           # Get person by ID
POST   /api/person                # Create new person
PUT    /api/person/{id}           # Update person
DELETE /api/person/{id}           # Delete person (soft delete)
GET    /api/person/search         # Search persons
```

#### Example: Create Person

**Request:**
```http
POST /api/person
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "birthDate": "1980-01-15",
  "householdId": 1,
  "photoFile": "<base64-encoded-image>"
}
```

**Response (201 Created):**
```json
{
  "id": 123,
  "firstName": "John",
  "lastName": "Doe",
  "fullName": "John Doe",
  "birthDate": "1980-01-15T00:00:00Z",
  "age": 44,
  "householdId": 1,
  "householdName": "Doe Family",
  "photoUrl": "https://storage.blob.core.windows.net/photos/123.jpg",
  "thumbnailUrl": "https://storage.blob.core.windows.net/thumbnails/small/123.jpg",
  "createdDateTime": "2025-12-21T18:00:00Z",
  "updatedDateTime": "2025-12-21T18:00:00Z"
}
```

---

### Household API

**Base Route:** `/api/household`

#### Endpoints

```http
GET    /api/household                        # List all households
GET    /api/household/{id}                   # Get household by ID
POST   /api/household                        # Create new household
PUT    /api/household/{id}                   # Update household
DELETE /api/household/{id}                   # Delete household
GET    /api/household/{id}/members           # Get household members
DELETE /api/household/{id}/member/{userId}   # Remove member
PUT    /api/household/{id}/member/{userId}/role  # Change member role
POST   /api/household/{id}/member/{userId}/resend-invite  # Resend invite
PUT    /api/household/{id}/settings          # Update settings
```

#### Example: Get Household Members

**Request:**
```http
GET /api/household/5/members
```

**Response (200 OK):**
```json
[
  {
    "personId": 10,
    "userId": "abc123",
    "fullName": "John Doe",
    "email": "john@example.com",
    "role": "ADMIN",
    "joinedDate": "2024-01-15T10:30:00Z",
    "photoUrl": "https://storage.blob.core.windows.net/photos/10.jpg"
  },
  {
    "personId": 11,
    "userId": "def456",
    "fullName": "Jane Doe",
    "email": "jane@example.com",
    "role": "EDITOR",
    "joinedDate": "2024-02-20T14:45:00Z",
    "photoUrl": "https://storage.blob.core.windows.net/photos/11.jpg"
  }
]
```

---

### ParentChild API

**Base Route:** `/api/parentchild`

#### Endpoints

```http
GET    /api/parentchild                   # List all parent-child relationships
GET    /api/parentchild/{id}              # Get relationship by ID
POST   /api/parentchild                   # Create new relationship
PUT    /api/parentchild/{id}              # Update relationship
DELETE /api/parentchild/{id}              # Delete relationship
GET    /api/parentchild/{id}/evidence     # Get evidence for relationship
GET    /api/parentchild/{id}/events       # Get related life events
GET    /api/parentchild/{id}/grandparents # Get grandparents
GET    /api/parentchild/{id}/siblings     # Get siblings
POST   /api/parentchild/{id}/verify       # Verify relationship
PUT    /api/parentchild/{id}/notes        # Update notes
```

#### Example: Verify Relationship

**Request:**
```http
POST /api/parentchild/25/verify
Authorization: Required (Admin or HouseholdAdmin)
```

**Response (200 OK):**
```json
{
  "id": 25,
  "parentPersonId": 10,
  "parentName": "John Doe",
  "childPersonId": 20,
  "childName": "Sarah Doe",
  "relationshipType": "BIOLOGICAL",
  "isVerified": true,
  "verifiedDate": "2025-12-21T18:00:00Z",
  "verifiedBy": "admin@example.com",
  "notes": "Birth certificate on file",
  "confidenceScore": 100
}
```

---

### Photo API

**Base Route:** `/api/photo`

#### Endpoints

```http
GET    /api/photo                 # List all photos
GET    /api/photo/{id}            # Get photo by ID
POST   /api/photo                 # Upload new photo
PUT    /api/photo/{id}            # Update photo metadata
DELETE /api/photo/{id}            # Delete photo
GET    /api/photo/{id}/tags       # Get photo tags (people in photo)
POST   /api/photo/{id}/tags       # Add person tag to photo
DELETE /api/photo/{id}/tags/{personId}  # Remove person tag
```

#### Example: Upload Photo

**Request:**
```http
POST /api/photo
Content-Type: multipart/form-data

------boundary
Content-Disposition: form-data; name="file"; filename="family-photo.jpg"
Content-Type: image/jpeg

<binary-image-data>
------boundary
Content-Disposition: form-data; name="title"

Family Reunion 2024
------boundary
Content-Disposition: form-data; name="description"

Annual family gathering at the park
------boundary
Content-Disposition: form-data; name="dateTaken"

2024-07-04
------boundary--
```

**Response (201 Created):**
```json
{
  "id": 500,
  "title": "Family Reunion 2024",
  "description": "Annual family gathering at the park",
  "dateTaken": "2024-07-04T00:00:00Z",
  "originalUrl": "https://storage.blob.core.windows.net/photos/500.jpg",
  "thumbnailSmallUrl": "https://storage.blob.core.windows.net/thumbnails/small/500.jpg",
  "thumbnailMediumUrl": "https://storage.blob.core.windows.net/thumbnails/medium/500.jpg",
  "uploadedBy": "John Doe",
  "uploadedDateTime": "2025-12-21T18:00:00Z",
  "tags": []
}
```

---

### Story API

**Base Route:** `/api/story`

#### Endpoints

```http
GET    /api/story                 # List all stories
GET    /api/story/{id}            # Get story by ID
POST   /api/story                 # Create new story
PUT    /api/story/{id}            # Update story
DELETE /api/story/{id}            # Delete story
GET    /api/story/category/{cat}  # Get stories by category
GET    /api/story/person/{id}     # Get stories about a person
```

---

### Tradition API

**Base Route:** `/api/tradition`

#### Endpoints

```http
GET    /api/tradition                 # List all traditions
GET    /api/tradition/{id}            # Get tradition by ID
POST   /api/tradition                 # Create new tradition
PUT    /api/tradition/{id}            # Update tradition
DELETE /api/tradition/{id}            # Delete tradition
GET    /api/tradition/category/{cat}  # Get traditions by category
```

---

## Common Response Codes

### Success Codes

| Code | Status | Description |
|------|--------|-------------|
| 200 | OK | Request successful |
| 201 | Created | Resource created successfully |
| 204 | No Content | Request successful, no content to return |

### Client Error Codes

| Code | Status | Description |
|------|--------|-------------|
| 400 | Bad Request | Invalid request format or validation error |
| 401 | Unauthorized | Authentication required |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource not found |
| 409 | Conflict | Resource conflict (e.g., duplicate) |
| 422 | Unprocessable Entity | Validation failed |

### Server Error Codes

| Code | Status | Description |
|------|--------|-------------|
| 500 | Internal Server Error | Unexpected server error |
| 503 | Service Unavailable | Service temporarily unavailable |

---

## Error Handling

### Error Response Format

All API errors return a consistent format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "FirstName": [
      "The FirstName field is required."
    ],
    "BirthDate": [
      "Birth date cannot be in the future."
    ]
  },
  "traceId": "00-abc123def456-789"
}
```

### Validation Errors (400 Bad Request)

```json
{
  "status": 400,
  "title": "Validation failed",
  "errors": {
    "Email": ["Invalid email format"],
    "Password": ["Password must be at least 8 characters"]
  }
}
```

### Not Found (404)

```json
{
  "status": 404,
  "title": "Resource not found",
  "detail": "Person with ID 999 was not found"
}
```

### Unauthorized (401)

```json
{
  "status": 401,
  "title": "Unauthorized",
  "detail": "Authentication required to access this resource"
}
```

### Forbidden (403)

```json
{
  "status": 403,
  "title": "Forbidden",
  "detail": "You do not have permission to access this resource"
}
```

---

## Rate Limiting

**Current Status:** Not implemented

**Planned Implementation:**
- 100 requests per minute per user
- 1000 requests per hour per user
- Headers: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`

---

## Versioning

**Current Version:** v1

**Versioning Strategy:** URL-based versioning

**Future Versions:**
- v1: Current API (default)
- v2: Planned for breaking changes (TBD)

**Example:**
```
/api/v1/person/123  (future)
/api/person/123     (current, defaults to v1)
```

---

## Best Practices

### Pagination

For list endpoints, use query parameters:

```http
GET /api/person?page=1&perPage=50
```

**Response includes pagination metadata:**
```json
{
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "perPage": 50,
    "totalPages": 10,
    "totalRecords": 500
  }
}
```

### Filtering

Use query parameters for filtering:

```http
GET /api/person?lastName=Doe&minAge=18&maxAge=65
```

### Sorting

Use `orderBy` and `direction` query parameters:

```http
GET /api/person?orderBy=lastName&direction=asc
```

### Searching

Use the dedicated search endpoint with query parameter:

```http
GET /api/person/search?q=john+doe
```

---

## Examples

### Complete CRUD Flow

**1. Create a Person**
```http
POST /api/person
{
  "firstName": "Alice",
  "lastName": "Smith",
  "birthDate": "1990-05-20",
  "householdId": 1
}
```

**2. Get the Person**
```http
GET /api/person/150
```

**3. Update the Person**
```http
PUT /api/person/150
{
  "firstName": "Alice",
  "lastName": "Johnson",  // Changed last name
  "birthDate": "1990-05-20",
  "householdId": 1
}
```

**4. Delete the Person**
```http
DELETE /api/person/150
```

---

## Testing with Postman

### Import OpenAPI Spec

1. Open Postman
2. Click **Import**
3. Enter URL: `https://localhost:5001/swagger/v1/swagger.json`
4. Click **Import**
5. All endpoints will be available in Postman

### Environment Variables

Create a Postman environment with:

```json
{
  "baseUrl": "https://localhost:5001",
  "personId": "123",
  "householdId": "5"
}
```

Use in requests: `{{baseUrl}}/api/person/{{personId}}`

---

## Support

### Documentation

- **Swagger UI:** https://localhost:5001/api-docs (Development)
- **Developer Onboarding:** See [DeveloperOnboarding.md](./DeveloperOnboarding.md)
- **Architecture Overview:** See README.md

### Issues

Report API bugs or request features:
- **GitHub Issues:** https://github.com/GLGRushton/RushtonRoots/issues

---

**Document Version:** 1.0  
**Last Updated:** December 2025  
**API Version:** v1
