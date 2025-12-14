# Phase 4.3 Implementation: Contribution & Validation System

## Overview
This document describes the implementation of Phase 4.3 from the ROADMAP.md - a comprehensive contribution and validation system that enables family members to suggest edits with approval workflows, citation requirements, conflict resolution, activity tracking, and gamification.

## Implementation Date
December 2025

## Features Implemented

### 1. Contribution Workflow (Suggest Edits)
- **Submit Contributions**: Users can suggest edits to any entity (Person, Partnership, LifeEvent, etc.)
- **Field-Level Changes**: Track specific field changes with old and new values
- **Reason Tracking**: Contributors must provide a reason for their suggested changes
- **Status Management**: Contributions move through Pending → Approved/Rejected/RequestMoreInfo workflow
- **Citation Support**: Optional citation linking for contributions
- **Citation Requirements**: Automatic detection of fields requiring citations (birth dates, death dates, places)
- **Contribution History**: Full history of all suggestions per entity and per user

### 2. Approval Process for Changes
- **Role-Based Approvals**: Only Admin and HouseholdAdmin users can approve/reject contributions
- **Review Workflow**: Reviewers can approve, reject, or request more information
- **Review Notes**: Reviewers can add notes explaining their decision
- **Approval History**: Multiple approval records per contribution
- **Final Decision Tracking**: Track which approval was the final decision
- **Automatic Notifications**: Contributors notified when their contributions are reviewed
- **Contributor Feedback**: Rejected contributions include reviewer notes for improvement

### 3. Fact-Checking and Citation Requirements
- **Citation Linking**: Link citations to specific facts (FactCitation entity)
- **Confidence Levels**: Rate citation confidence (Low, Medium, High, Proven)
- **Field-Level Citations**: Citations tracked per entity type, entity ID, and field name
- **Citation Notes**: Additional context about how citation supports the fact
- **Automatic Detection**: System automatically flags fields that require citations
- **Citation Validation**: Contributions for critical fields require citations before approval

**Citation-Required Fields**:
- BirthDate, BirthPlace
- DeathDate, DeathPlace
- MarriageDate, MarriagePlace
- BurialPlace

### 4. Conflict Resolution for Disputed Information
- **Conflict Detection**: System tracks conflicting data from different sources
- **Conflict Types**: DataMismatch, DuplicateEntry, SourceConflict
- **Conflict Tracking**: Store both current value and conflicting value
- **Resolution Workflow**: Admin users can resolve conflicts
- **Resolution Options**: AcceptCurrent, AcceptNew, AcceptBoth, Custom
- **Citation Selection**: Choose which citation is more authoritative
- **Resolution Notes**: Document why a particular resolution was chosen
- **Conflict Status**: Open, UnderReview, Resolved, Dismissed
- **Activity Tracking**: Conflict resolutions tracked in activity feed

### 5. Activity Feed Showing Recent Contributions
- **Activity Types**: ContributionSubmitted, ContributionApproved, PersonAdded, PhotoUploaded, etc.
- **User Attribution**: Track which user performed each activity
- **Entity Linking**: Link activities to specific entities (Person, Media, etc.)
- **Activity Description**: Human-readable description of what happened
- **Points Tracking**: Points earned for each activity (for gamification)
- **Public/Private Control**: Activities can be public or private
- **Recent Activities**: View most recent activities across all users
- **User Activities**: Filter activities by specific user
- **Public Feed**: Public activity feed for family-wide visibility
- **Activity Pagination**: Support for loading activities in chunks

### 6. Contribution Leaderboard/Gamification
- **Point System**: Users earn points for various contributions
- **Rank Progression**: Novice → Contributor → Researcher → Historian → Expert
- **Contribution Tracking**: Track count of submitted, approved, and rejected contributions
- **Activity Metrics**: Track people added, photos uploaded, stories written, etc.
- **Leaderboard**: Top contributors ranked by total points
- **User Score Dashboard**: Individual users can view their contribution statistics
- **Last Activity**: Track when user last contributed
- **Automatic Rank Updates**: Ranks automatically update based on point thresholds

**Point Values**:
- Contribution Submitted: 5 points
- Contribution Approved: 10 points (bonus)
- Citation Added: 8 points
- Conflict Resolved: 15 points
- Person Added: 20 points
- Photo Uploaded: 3 points
- Story Written: 25 points

**Rank Thresholds**:
- Novice: 0-49 points
- Contributor: 50-199 points
- Researcher: 200-499 points
- Historian: 500-999 points
- Expert: 1000+ points

## Database Schema

### Contribution
```csharp
- Id: int (PK)
- EntityType: string (required, max 100) - Type of entity (Person, Partnership, etc.)
- EntityId: int (required) - ID of entity being modified
- FieldName: string (required, max 100) - Which field is being changed
- OldValue: string? (max 2000) - Previous value
- NewValue: string (required, max 2000) - Proposed new value
- Reason: string (required, max 1000) - Why this change is suggested
- ContributorUserId: string (required, FK to ApplicationUser)
- Status: string (required, max 50, default "Pending") - Pending, Approved, Rejected, Conflicted, RequestMoreInfo
- ReviewerUserId: string? (FK to ApplicationUser)
- ReviewedAt: DateTime?
- ReviewNotes: string? (max 1000)
- CitationId: int? (FK to Citation)
- RequiresCitation: bool
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Indexes: (EntityType, EntityId), Status, ContributorUserId, CreatedDateTime
```

### ContributionApproval
```csharp
- Id: int (PK)
- ContributionId: int (required, FK to Contribution)
- ApproverUserId: string (required, FK to ApplicationUser)
- Decision: string (required, max 50) - Approved, Rejected, RequestMoreInfo
- Notes: string? (max 1000)
- DecisionDate: DateTime (required)
- IsFinalDecision: bool
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Indexes: ContributionId, ApproverUserId, DecisionDate
```

### FactCitation
```csharp
- Id: int (PK)
- EntityType: string (required, max 100)
- EntityId: int (required)
- FieldName: string (required, max 100)
- CitationId: int (required, FK to Citation)
- ConfidenceLevel: string (required, max 50, default "Medium") - Low, Medium, High, Proven
- Notes: string? (max 1000)
- AddedByUserId: string (required, FK to ApplicationUser)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Indexes: (EntityType, EntityId, FieldName), CitationId
```

### ConflictResolution
```csharp
- Id: int (PK)
- EntityType: string (required, max 100)
- EntityId: int (required)
- FieldName: string (required, max 100)
- ContributionId: int? (FK to Contribution)
- ConflictType: string (required, max 50) - DataMismatch, DuplicateEntry, SourceConflict
- CurrentValue: string? (max 2000)
- ConflictingValue: string? (max 2000)
- Status: string (required, max 50, default "Open") - Open, UnderReview, Resolved, Dismissed
- Resolution: string? (max 50) - AcceptCurrent, AcceptNew, AcceptBoth, Custom
- ResolutionNotes: string? (max 2000)
- ResolvedByUserId: string? (FK to ApplicationUser)
- ResolvedAt: DateTime?
- AcceptedCitationId: int? (FK to Citation)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Indexes: (EntityType, EntityId), Status, ContributionId
```

### ActivityFeedItem
```csharp
- Id: int (PK)
- UserId: string (required, FK to ApplicationUser)
- ActivityType: string (required, max 100) - ContributionSubmitted, ContributionApproved, PersonAdded, etc.
- EntityType: string? (max 100)
- EntityId: int?
- Description: string (required, max 500)
- ActionUrl: string? (max 500)
- Points: int (default 0)
- IsPublic: bool (default true)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Indexes: UserId, ActivityType, CreatedDateTime, IsPublic
```

### ContributionScore
```csharp
- Id: int (PK)
- UserId: string (required, FK to ApplicationUser, unique)
- TotalPoints: int (default 0)
- ContributionsSubmitted: int (default 0)
- ContributionsApproved: int (default 0)
- ContributionsRejected: int (default 0)
- CitationsAdded: int (default 0)
- ConflictsResolved: int (default 0)
- PeopleAdded: int (default 0)
- PhotosUploaded: int (default 0)
- StoriesWritten: int (default 0)
- LastActivityDate: DateTime?
- CurrentRank: string (required, max 50, default "Novice")
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Indexes: UserId (unique), TotalPoints, LastActivityDate
```

## API Endpoints

### Contribution Endpoints
- `GET /api/Contribution/{id}` - Get contribution by ID
- `GET /api/Contribution/status/{status}` - Get contributions by status
- `GET /api/Contribution/pending` - Get all pending contributions
- `GET /api/Contribution/my-contributions` - Get current user's contributions
- `POST /api/Contribution` - Create new contribution
- `POST /api/Contribution/review` - Review a contribution (Admin/HouseholdAdmin only)
- `POST /api/Contribution/apply/{id}` - Apply an approved contribution (Admin/HouseholdAdmin only)

### Activity Feed Endpoints
- `GET /api/ActivityFeed/recent?count=50` - Get recent activities
- `GET /api/ActivityFeed/public?count=50` - Get public activities
- `GET /api/ActivityFeed/user/{userId}?count=50` - Get user's activities
- `GET /api/ActivityFeed/my-activities?count=50` - Get current user's activities

### Leaderboard Endpoints
- `GET /api/Leaderboard?count=10` - Get top contributors
- `GET /api/Leaderboard/user/{userId}` - Get user's score
- `GET /api/Leaderboard/my-score` - Get current user's score

### Conflict Resolution Endpoints
- `GET /api/ConflictResolution/{id}` - Get conflict by ID
- `GET /api/ConflictResolution/open` - Get open conflicts (Admin/HouseholdAdmin only)
- `POST /api/ConflictResolution/resolve` - Resolve a conflict (Admin/HouseholdAdmin only)

## Services

### IContributionService
- `GetByIdAsync(int id)` - Get contribution by ID
- `GetByStatusAsync(string status)` - Get contributions by status
- `GetByContributorAsync(string userId)` - Get user's contributions
- `GetPendingContributionsAsync()` - Get all pending contributions
- `CreateAsync(CreateContributionRequest, userId)` - Submit new contribution
- `ReviewAsync(ReviewContributionRequest, reviewerUserId)` - Review contribution
- `ApplyContributionAsync(int contributionId)` - Apply approved contribution

### IActivityFeedService
- `GetRecentActivitiesAsync(int count)` - Get recent activities
- `GetUserActivitiesAsync(string userId, int count)` - Get user activities
- `GetPublicActivitiesAsync(int count)` - Get public activities
- `RecordActivityAsync(userId, activityType, entityType, entityId, description, points)` - Record new activity

### IContributionScoreService
- `GetByUserIdAsync(string userId)` - Get user's score
- `GetLeaderboardAsync(int count)` - Get top contributors
- `IncrementContributionSubmittedAsync(string userId)` - Award points for submission
- `IncrementContributionApprovedAsync(string userId)` - Award points for approval
- `IncrementContributionRejectedAsync(string userId)` - Track rejection
- `IncrementCitationAddedAsync(string userId)` - Award points for citation
- `IncrementConflictResolvedAsync(string userId)` - Award points for conflict resolution
- `IncrementPersonAddedAsync(string userId)` - Award points for adding person
- `IncrementPhotoUploadedAsync(string userId)` - Award points for photo upload
- `IncrementStoryWrittenAsync(string userId)` - Award points for story

### IConflictResolutionService
- `GetByIdAsync(int id)` - Get conflict by ID
- `GetOpenConflictsAsync()` - Get all open conflicts
- `ResolveConflictAsync(ResolveConflictRequest, resolverUserId)` - Resolve conflict

## Repositories

All repositories follow the standard pattern with interfaces and implementations:
- **ContributionRepository**: CRUD operations for contributions with status filtering
- **ActivityFeedRepository**: Query recent, user-specific, and public activities
- **ContributionScoreRepository**: Manage user scores and leaderboard
- **ConflictResolutionRepository**: Manage conflicts with status filtering

## Automatic Behaviors

### When Contribution is Submitted
1. Determine if citation is required based on field name
2. Create contribution with "Pending" status
3. Record activity in feed with 5 points
4. Increment user's ContributionsSubmitted count
5. Award 5 points to contributor

### When Contribution is Approved
1. Update contribution status to "Approved"
2. Create ContributionApproval record
3. Record activity in feed with 10 bonus points
4. Increment user's ContributionsApproved count
5. Award 10 additional points (15 total)
6. Update user rank if threshold crossed
7. Send notification to contributor

### When Contribution is Rejected
1. Update contribution status to "Rejected"
2. Create ContributionApproval record with notes
3. Increment user's ContributionsRejected count
4. No points awarded for rejected contributions
5. Send notification to contributor with feedback

### Rank Updates
Ranks are automatically recalculated whenever points are awarded:
- Checks current total points
- Assigns appropriate rank based on thresholds
- Persists rank change to database

## Testing

### Unit Tests Implemented
- **ContributionServiceTests**: 5 tests covering creation, review, and retrieval
- **ContributionScoreServiceTests**: 6 tests covering score management and rank progression
- **ActivityFeedServiceTests**: 4 tests covering activity recording and retrieval

All tests use FakeItEasy for mocking and follow AAA (Arrange-Act-Assert) pattern.

**Test Results**: 15/15 tests passing ✅

## Security Considerations

### Authorization
- **Public Access**: Anyone can submit contributions
- **Review Access**: Only Admin and HouseholdAdmin can approve/reject contributions
- **Conflict Resolution**: Only Admin and HouseholdAdmin can resolve conflicts
- **Activity Feed**: Public activities visible to all; private activities restricted to user

### Data Validation
- All required fields enforced at database level
- String length limits prevent overflow attacks
- Status values restricted to predefined set
- User IDs validated through ASP.NET Identity

### Audit Trail
- All contributions tracked with timestamps
- Approval history preserved
- Activity feed provides complete audit log
- Conflict resolutions documented with notes

## Integration with Existing Features

### Citations (Phase 2.2)
- Contributions can reference existing citations
- FactCitation links specific facts to citations
- Citation confidence levels support fact verification

### Notifications (Phase 4.1)
- Automatic notifications when contributions are reviewed
- Notification preferences respected for contribution updates

### Comments (Phase 4.2)
- Contributors can discuss changes through comments
- Comments on entities can reference contributions

## Future Enhancements

While Phase 4.3 is complete, potential future improvements could include:

1. **Automatic Conflict Detection**: System automatically detects when new contributions conflict with existing data
2. **Bulk Approval**: Approve multiple contributions at once
3. **Contribution Templates**: Pre-defined templates for common edit types
4. **AI-Assisted Review**: Machine learning to suggest likely correct values
5. **Contribution Analytics**: Dashboard showing contribution trends over time
6. **Badge System**: Special badges for milestone achievements
7. **Contribution Challenges**: Monthly challenges to encourage participation
8. **Expert Review**: Assign contributions to specific expert family members
9. **Draft Contributions**: Save contributions as drafts before submitting
10. **Contribution Discussions**: Dedicated discussion threads for each contribution

## Migration

Migration `20251214112554_AddPhase4_3ContributionSystem` creates all necessary tables:
- Contributions
- ContributionApprovals
- FactCitations
- ConflictResolutions
- ActivityFeedItems
- ContributionScores

Migration runs automatically on application startup.

## Success Criteria

All Phase 4.3 requirements have been successfully implemented:

✅ **Create contribution workflow (suggest edits)**: Users can submit suggestions for any entity field  
✅ **Implement approval process for changes**: Role-based review workflow with approve/reject/request info  
✅ **Add fact-checking and citation requirements**: Automatic detection and citation linking  
✅ **Create conflict resolution for disputed information**: Full conflict tracking and resolution workflow  
✅ **Build activity feed showing recent contributions**: Comprehensive activity tracking with public/private control  
✅ **Add contribution leaderboard/gamification**: Point system, ranks, and leaderboard

## Conclusion

Phase 4.3 establishes a robust contribution and validation system that empowers all family members to participate in building the family tree while maintaining data quality through review workflows and citation requirements. The gamification elements encourage ongoing participation, and the activity feed provides transparency into who is contributing what.

This completes the Collaboration & Communication phase (Phase 4) of the RushtonRoots roadmap, providing a comprehensive platform for family members to work together on preserving and enhancing their shared family history.

---

**Implementation Completed**: December 2025  
**Next Phase**: Phase 5 - Research & DNA Integration
