# ViewBag/ViewData Contracts Documentation

**Date:** December 2025  
**Version:** 1.0  
**Purpose:** Document all ViewBag and ViewData contracts used throughout the application

---

## Overview

This document provides a comprehensive reference for all ViewBag and ViewData contracts used in RushtonRoots Razor views. Each controller that populates ViewBag or ViewData for its views is documented below with the data structure contract.

---

## Table of Contents

1. [HomeController](#homecontroller)
2. [AdminController](#admincontroller)
3. [StoryViewController](#storyviewcontroller)
4. [TraditionViewController](#traditionviewcontroller)

---

## HomeController

**View:** `Views/Home/Index.cshtml`  
**Purpose:** Display home page with family statistics, recent additions, upcoming events, and activity feed

### ViewBag Contract

| Property | Type | Description | Source |
|----------|------|-------------|--------|
| `TotalMembers` | int | Total count of non-deleted people | IHomePageService.GetStatisticsAsync() |
| `TotalPhotos` | int | Total count of media items (placeholder) | IHomePageService.GetStatisticsAsync() |
| `TotalStories` | int | Total count of published stories | IHomePageService.GetStatisticsAsync() |
| `ActiveHouseholds` | int | Total count of households | IHomePageService.GetStatisticsAsync() |
| `OldestAncestor` | PersonSummary? | Oldest family member by birth date | IHomePageService.GetStatisticsAsync() + GetPersonSummaryByNameAsync() |
| `NewestMember` | PersonSummary? | Most recently added family member | IHomePageService.GetStatisticsAsync() + GetPersonSummaryByNameAsync() |
| `RecentAdditions` | List\<object\> | Recently added people | IHomePageService.GetRecentAdditionsAsync() |
| `UpcomingBirthdays` | List\<object\> | Upcoming birthdays (next 30 days) | IHomePageService.GetUpcomingBirthdaysAsync() |
| `UpcomingAnniversaries` | List\<object\> | Upcoming anniversaries (next 30 days) | IHomePageService.GetUpcomingAnniversariesAsync() |
| `ActivityFeed` | List\<object\> | Recent activity feed (last 20 items) | IHomePageService.GetActivityFeedAsync() |

### PersonSummary Object Structure

```csharp
{
    id: int,
    fullName: string,
    photoUrl: string?,
    birthDate: string?,  // Format: "yyyy-MM-dd"
    deathDate: string?,  // Format: "yyyy-MM-dd"
    age: int?
}
```

### RecentAdditions Object Structure

```csharp
{
    person: {
        id: int,
        fullName: string,
        photoUrl: string?
    },
    addedDate: DateTime,
    addedBy: string
}
```

### UpcomingEvent Object Structure (Birthdays & Anniversaries)

```csharp
{
    id: int,
    personId: int,
    personName: string,
    photoUrl: string?,
    eventType: string,  // "birthday" or "anniversary"
    eventDate: DateTime,
    daysUntil: int,
    description: string  // "Turning X" or "X years"
}
```

### ActivityFeed Object Structure

```csharp
{
    id: string,
    type: string,  // "member_added", "photo_uploaded", "story_published", "comment_posted"
    icon: string,  // Material Icons name
    color: string, // Hex color code
    title: string,
    description: string,
    timestamp: DateTime,
    userName: string,
    userPhotoUrl: string?,
    relatedUrl: string?
}
```

---

## AdminController

**View:** `Views/Admin/Dashboard.cshtml`  
**Purpose:** Display admin dashboard with system statistics and recent activity

### ViewData Contract

| Property | Type | Description | Source |
|----------|------|-------------|--------|
| `TotalUsers` | int | Total count of application users | IAdminDashboardService.GetSystemStatisticsAsync() |
| `TotalHouseholds` | int | Total count of households | IAdminDashboardService.GetSystemStatisticsAsync() |
| `TotalPersons` | int | Total count of people | IAdminDashboardService.GetSystemStatisticsAsync() |
| `MediaItems` | int | Total count of media items (placeholder) | IAdminDashboardService.GetSystemStatisticsAsync() |
| `RecentActivity` | List\<RecentActivity\> | Recent system activity (last 20 items) | IAdminDashboardService.GetRecentActivityAsync() |

### RecentActivity Object Structure

```csharp
{
    ActivityType: string,  // "PersonAdded", "StoryPublished", "PhotoAdded"
    Description: string,
    UserName: string,
    CreatedDateTime: DateTime,
    ActionUrl: string?
}
```

---

## StoryViewController

**View:** `Views/StoryView/Index_Angular.cshtml`  
**Purpose:** Display stories index page (Angular component with noscript fallback)

### ViewBag Contract

| Property | Type | Description | Source |
|----------|------|-------------|--------|
| `Stories` | List\<StoryViewModel\> | All published stories (for noscript fallback) | IStoryService.GetAllAsync(publishedOnly: true) |
| `Categories` | List\<string\> | Available story categories | TODO: Implement category service |
| `FeaturedStories` | List\<object\> | Featured stories | TODO: Implement featured stories |
| `RecentStories` | List\<object\> | Recently published stories | TODO: Implement recent stories |
| `CanEdit` | bool | Whether current user can edit stories | User.IsInRole("Admin") \|\| User.IsInRole("HouseholdAdmin") |

### Notes

- The Angular component (`app-story-index`) fetches data from API endpoints directly (`/api/story`)
- ViewBag data is primarily for noscript fallback scenarios
- Categories, FeaturedStories, and RecentStories are placeholders pending feature implementation

---

## TraditionViewController

**View:** `Views/Tradition/Index_Angular.cshtml`  
**Purpose:** Display traditions index page (Angular component with noscript fallback)

### ViewBag Contract

| Property | Type | Description | Source |
|----------|------|-------------|--------|
| `Traditions` | List\<TraditionViewModel\> | All published traditions (for noscript fallback) | ITraditionService.GetAllAsync(publishedOnly: true) |
| `Categories` | List\<string\> | Available tradition categories | TODO: Implement category service |
| `FeaturedTraditions` | List\<object\> | Featured traditions | TODO: Implement featured traditions |
| `RecentTraditions` | List\<object\> | Recently published traditions | TODO: Implement recent traditions |
| `CanEdit` | bool | Whether current user can edit traditions | User.IsInRole("Admin") \|\| User.IsInRole("HouseholdAdmin") |

### Notes

- The Angular component (`app-tradition-index`) fetches data from API endpoints directly (`/api/tradition`)
- ViewBag data is primarily for noscript fallback scenarios
- Categories, FeaturedTraditions, and RecentTraditions are placeholders pending feature implementation

---

## Best Practices

### When to Use ViewBag vs ViewData

- **ViewBag**: Use for dynamic properties that are set once and read in the view (preferred for simplicity)
- **ViewData**: Use when you need dictionary-style access or when passing data to layout/partial views

### Null Handling

All ViewBag/ViewData access in views should use null-coalescing operators to provide defaults:

```cshtml
@(ViewBag.PropertyName ?? defaultValue)
@(ViewData["Key"] ?? defaultValue)
```

### Documentation Requirements

When adding new ViewBag/ViewData properties:

1. Document the contract in the controller XML comments (`<remarks>` section)
2. Update this documentation file
3. Ensure views handle null/empty values gracefully
4. Add appropriate unit tests if the property requires complex logic

---

## Related Documentation

- **Controllers**: `RushtonRoots.Web/Controllers/`
- **Views**: `RushtonRoots.Web/Views/`
- **Services**: `RushtonRoots.Application/Services/`
- **View Models**: `RushtonRoots.Domain/UI/Models/`

---

**Document Status:** ✅ Complete  
**Last Updated:** December 2025  
**Maintained By:** Development Team
