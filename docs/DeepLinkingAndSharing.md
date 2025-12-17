# RushtonRoots - Deep Linking and Sharing Documentation

**Last Updated**: December 17, 2025  
**Phase**: 12.4 - Deep Linking and Sharing  
**Status**: ✅ Complete

---

## Table of Contents

1. [Overview](#overview)
2. [Features Implemented](#features-implemented)
3. [Social Meta Tags](#social-meta-tags)
4. [Share Functionality](#share-functionality)
5. [ShareDialogComponent](#sharedialogcomponent)
6. [SocialMetaService](#socialmetaservice)
7. [ShareService](#shareservice)
8. [Deep Linking Support](#deep-linking-support)
9. [Usage Examples](#usage-examples)
10. [Integration Guide](#integration-guide)
11. [Testing](#testing)
12. [Best Practices](#best-practices)
13. [Future Enhancements](#future-enhancements)

---

## Overview

Phase 12.4 implements comprehensive deep linking and social media sharing functionality for RushtonRoots. This enables users to easily share family tree content (person profiles, stories, recipes, traditions, photos) on social media platforms and via email, while ensuring rich previews with Open Graph and Twitter Card meta tags.

### Key Goals

- Enable sharing of all major content types
- Support social media platforms (Facebook, Twitter, LinkedIn, WhatsApp)
- Provide rich preview metadata (Open Graph, Twitter Cards)
- Ensure all pages support deep linking
- Create shareable public links for family content
- Support native mobile sharing where available

---

## Features Implemented

### ✅ Completed Features

1. **SocialMetaService**
   - Dynamic Open Graph meta tag updates
   - Twitter Card meta tag support
   - Automatic SEO-friendly descriptions
   - Image URL handling (relative to absolute)
   - Content-specific meta tags (person, story, recipe, tradition, photo, household, wiki)

2. **ShareService**
   - Native Web Share API integration (mobile devices)
   - Clipboard API fallback for desktop
   - Email sharing via mailto links
   - Social media direct links (Facebook, Twitter, LinkedIn, WhatsApp)
   - Share link generation for all content types
   - Success/error notifications via MatSnackBar

3. **ShareDialogComponent**
   - Comprehensive sharing dialog UI
   - Native share button (mobile)
   - Copy link button with visual feedback
   - Email share button
   - Social media buttons (Facebook, Twitter, LinkedIn, WhatsApp)
   - Shareable link preview
   - Material Design styling
   - Mobile-responsive layout

4. **Meta Tags in _Layout.cshtml**
   - Server-side meta tag rendering
   - ViewData-based meta tag configuration
   - Open Graph tags (og:title, og:description, og:type, og:url, og:image, og:site_name)
   - Twitter Card tags (twitter:card, twitter:title, twitter:description, twitter:image)
   - Article-specific tags (article:author, article:published_time, article:section)
   - Profile-specific tags (profile:first_name, profile:last_name)

5. **Deep Linking Support**
   - All major pages support shareable URLs
   - URL parameter tracking (utm_source, utm_medium)
   - Public link generation methods
   - Family tree view links (ready for backend implementation)

---

## Social Meta Tags

### Implementation in _Layout.cshtml

The `_Layout.cshtml` file now includes comprehensive meta tags that can be configured via `ViewData` from any controller:

```cshtml
@* Standard Meta Tags *@
<meta name="description" content="@(ViewData["Description"] ?? "RushtonRoots - Preserve your family history and connect with your heritage")" />

@* Open Graph Meta Tags (Facebook, LinkedIn) *@
<meta property="og:title" content="@(ViewData["OgTitle"] ?? ViewData["Title"] ?? "RushtonRoots")" />
<meta property="og:description" content="@(ViewData["OgDescription"] ?? ViewData["Description"] ?? "...")" />
<meta property="og:type" content="@(ViewData["OgType"] ?? "website")" />
<meta property="og:url" content="@(ViewData["OgUrl"] ?? $"{Context.Request.Scheme}://{Context.Request.Host}{Context.Request.Path}")" />
<meta property="og:image" content="@(ViewData["OgImage"] ?? $"{Context.Request.Scheme}://{Context.Request.Host}/assets/images/rushton-roots-og-image.jpg")" />
<meta property="og:site_name" content="RushtonRoots" />

@* Twitter Card Meta Tags *@
<meta name="twitter:card" content="summary_large_image" />
<meta name="twitter:title" content="@(ViewData["TwitterTitle"] ?? ViewData["OgTitle"] ?? ...)" />
<meta name="twitter:description" content="@(ViewData["TwitterDescription"] ?? ...)" />
<meta name="twitter:image" content="@(ViewData["TwitterImage"] ?? ...)" />
```

### Controller Example

```csharp
public IActionResult Details(int id)
{
    var person = _personService.GetById(id);
    
    // Set meta tags for social sharing
    ViewData["Title"] = $"{person.FirstName} {person.LastName} - Family Profile";
    ViewData["Description"] = $"{person.FirstName} {person.LastName}, {person.Occupation ?? "Family Member"}";
    ViewData["OgType"] = "profile";
    ViewData["OgImage"] = person.PhotoUrl ?? "/assets/images/default-person.jpg";
    ViewData["ProfileFirstName"] = person.FirstName;
    ViewData["ProfileLastName"] = person.LastName;
    
    return View(person);
}
```

### Supported Content Types

| Content Type | OgType | Special Meta Tags |
|--------------|--------|-------------------|
| Person Profile | `profile` | `profile:first_name`, `profile:last_name` |
| Story | `article` | `article:author`, `article:published_time` |
| Recipe | `article` | `recipe:duration` (custom) |
| Tradition | `article` | `article:section` |
| Household | `profile` | Default profile tags |
| Wiki Article | `article` | `article:section` (category) |
| Photo | `photo` | Image-specific tags |
| Home/Index | `website` | Default website tags |

---

## Share Functionality

### ShareService API

The `ShareService` provides multiple sharing methods:

#### Main Share Method

```typescript
async share(options: ShareOptions): Promise<ShareResult>
```

Automatically selects the best sharing method:
1. Native Web Share API (mobile devices) if supported
2. Clipboard API (desktop) as fallback

#### Specialized Share Methods

```typescript
// Copy link to clipboard
async copyToClipboard(url: string): Promise<ShareResult>

// Share via email
shareViaEmail(options: ShareOptions): ShareResult

// Share on social media
shareOnFacebook(url: string): void
shareOnTwitter(options: ShareOptions): void
shareOnLinkedIn(url: string): void
shareOnWhatsApp(options: ShareOptions): void
```

#### Link Generation Methods

```typescript
generatePersonShareLink(personId: number, includeParams?: boolean): string
generateStoryShareLink(storyId: number): string
generateRecipeShareLink(recipeId: number): string
generateTraditionShareLink(traditionId: number): string
generatePhotoShareLink(photoId: number): string
generateHouseholdShareLink(householdId: number): string
generateWikiShareLink(articleId: number): string
generatePublicFamilyTreeLink(personId: number, token?: string): string
```

---

## ShareDialogComponent

### Features

- **Native Share Button**: Uses Web Share API on mobile devices
- **Copy Link**: Clipboard API with visual feedback (icon changes to checkmark)
- **Email**: Opens email client with pre-filled subject and body
- **Social Media**: Facebook, Twitter, LinkedIn, WhatsApp sharing
- **Link Preview**: Shows title, description, and shareable URL
- **Responsive Design**: Adapts to mobile and desktop screens

### Usage

```typescript
import { MatDialog } from '@angular/material/dialog';
import { ShareDialogComponent, ShareDialogData } from './share-dialog/share-dialog.component';

constructor(private dialog: MatDialog) {}

openShareDialog() {
  const dialogData: ShareDialogData = {
    title: 'John Doe - Family Profile',
    text: 'Check out this family member profile',
    url: window.location.href,
    showSocialButtons: true,
    showEmailButton: true,
    showCopyButton: true
  };

  const dialogRef = this.dialog.open(ShareDialogComponent, {
    width: '500px',
    data: dialogData
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result?.shared) {
      console.log(`Shared via ${result.method}`);
    }
  });
}
```

### Dialog Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `title` | `string` | required | Title for share preview |
| `text` | `string` | required | Description text |
| `url` | `string` | required | URL to share |
| `showSocialButtons` | `boolean` | `true` | Show Facebook, Twitter, LinkedIn, WhatsApp buttons |
| `showEmailButton` | `boolean` | `true` | Show email share button |
| `showCopyButton` | `boolean` | `true` | Show copy link button |

---

## SocialMetaService

### Features

- Dynamically updates HTML meta tags for social media sharing
- Supports all major content types
- Automatically truncates descriptions to 160 characters
- Converts relative URLs to absolute URLs
- Builds SEO-friendly descriptions

### API Methods

```typescript
// Update meta tags for different content types
updatePersonMeta(person: PersonMetaData): void
updateStoryMeta(story: StoryMetaData): void
updateRecipeMeta(recipe: RecipeMetaData): void
updateTraditionMeta(tradition: TraditionMetaData): void
updatePhotoMeta(photo: PhotoMetaData): void
updateHouseholdMeta(household: HouseholdMetaData): void
updateWikiMeta(article: WikiMetaData): void

// Reset to default meta tags
resetToDefault(): void
```

### Usage Example

```typescript
import { SocialMetaService } from './social-meta.service';

constructor(private socialMetaService: SocialMetaService) {}

ngOnInit() {
  // For a person profile page
  this.socialMetaService.updatePersonMeta({
    id: 1,
    firstName: 'John',
    lastName: 'Doe',
    dateOfBirth: '1950-01-15',
    occupation: 'Engineer',
    photoUrl: '/photos/john-doe.jpg',
    placeOfBirth: 'New York, NY'
  });
}
```

### Meta Tag Updates

The service updates the following tags:

**Standard Tags:**
- `<title>` (page title)
- `<meta name="description">`

**Open Graph Tags:**
- `og:title`
- `og:description`
- `og:url`
- `og:image`
- `og:type` (website, article, profile, photo)
- `og:site_name`

**Twitter Card Tags:**
- `twitter:card` (summary_large_image)
- `twitter:title`
- `twitter:description`
- `twitter:image`
- `twitter:url`

**Content-Specific Tags:**
- Article: `article:author`, `article:published_time`, `article:section`
- Profile: Included in description string

---

## Deep Linking Support

### URL Structure

All major pages support deep linking with shareable URLs:

| Content Type | URL Format | Example |
|--------------|------------|---------|
| Person Profile | `/Person/Details/{id}` | `/Person/Details/42` |
| Story | `/StoryView?id={id}` | `/StoryView?id=7` |
| Recipe | `/Recipe?id={id}` | `/Recipe?id=15` |
| Tradition | `/Tradition?id={id}` | `/Tradition?id=3` |
| Photo | `/Media/Photo/{id}` | `/Media/Photo/123` |
| Household | `/Household/Details/{id}` | `/Household/Details/5` |
| Wiki Article | `/Wiki/Article/{id}` | `/Wiki/Article/28` |
| Family Tree (Public) | `/FamilyTree/Public/{id}?token={token}` | `/FamilyTree/Public/1?token=abc123` |

### URL Tracking Parameters

Share links include UTM parameters for analytics:

```
/Person/Details/42?utm_source=share&utm_medium=social
```

This enables tracking of:
- Traffic source (share link)
- Medium (social, email, etc.)
- Campaign information (optional)

### Public Sharing Links

The `ShareService` includes a method for generating public family tree view links:

```typescript
generatePublicFamilyTreeLink(personId: number, token?: string): string
```

**Note**: This feature requires backend implementation for:
1. Generating secure share tokens
2. Token validation
3. Privacy controls (what data is publicly visible)
4. Token expiration

---

## Usage Examples

### Example 1: Share Button on Person Profile

```typescript
// person-details.component.ts
import { MatDialog } from '@angular/material/dialog';
import { ShareDialogComponent } from '../../../shared/components/share-dialog/share-dialog.component';
import { ShareService } from '../../../shared/services/share.service';
import { SocialMetaService } from '../../../shared/services/social-meta.service';

export class PersonDetailsComponent implements OnInit {
  @Input() person: PersonDetails;

  constructor(
    private dialog: MatDialog,
    private shareService: ShareService,
    private socialMetaService: SocialMetaService
  ) {}

  ngOnInit() {
    // Update social meta tags
    this.socialMetaService.updatePersonMeta({
      id: this.person.id,
      firstName: this.person.firstName,
      lastName: this.person.lastName,
      dateOfBirth: this.person.dateOfBirth,
      dateOfDeath: this.person.dateOfDeath,
      photoUrl: this.person.photoUrl,
      occupation: this.person.occupation,
      placeOfBirth: this.person.placeOfBirth
    });
  }

  onShareClick() {
    const shareUrl = this.shareService.generatePersonShareLink(this.person.id, true);
    
    this.dialog.open(ShareDialogComponent, {
      width: '500px',
      data: {
        title: `${this.person.firstName} ${this.person.lastName} - Family Profile`,
        text: `Check out ${this.person.firstName}'s family profile on RushtonRoots`,
        url: shareUrl,
        showSocialButtons: true
      }
    });
  }
}
```

```html
<!-- person-details.component.html -->
<button mat-raised-button color="primary" (click)="onShareClick()">
  <mat-icon>share</mat-icon>
  Share Profile
</button>
```

### Example 2: Quick Share (No Dialog)

```typescript
// story-details.component.ts
async quickShare() {
  const shareUrl = this.shareService.generateStoryShareLink(this.story.id);
  
  const result = await this.shareService.share({
    title: this.story.title,
    text: 'Read this family story on RushtonRoots',
    url: shareUrl
  });

  if (result.success && result.method === 'clipboard') {
    // Link copied to clipboard
    console.log('Share link copied!');
  }
}
```

### Example 3: Server-Side Meta Tags

```csharp
// PersonController.cs
public IActionResult Details(int id)
{
    var person = _personService.GetById(id);
    
    if (person == null)
    {
        return NotFound();
    }

    // Set meta tags for social sharing
    ViewData["Title"] = $"{person.FirstName} {person.LastName}";
    ViewData["Description"] = $"{person.FirstName} {person.LastName}, {person.Occupation ?? "Family Member"}. " +
                             $"Born {person.DateOfBirth?.ToString("MMMM d, yyyy")} in {person.PlaceOfBirth ?? "Unknown"}";
    
    ViewData["OgType"] = "profile";
    ViewData["OgTitle"] = $"{person.FirstName} {person.LastName} - RushtonRoots Family Profile";
    ViewData["OgDescription"] = ViewData["Description"];
    ViewData["OgImage"] = person.PhotoUrl ?? "/assets/images/default-person.jpg";
    ViewData["OgUrl"] = $"{Request.Scheme}://{Request.Host}/Person/Details/{person.Id}";
    
    ViewData["ProfileFirstName"] = person.FirstName;
    ViewData["ProfileLastName"] = person.LastName;

    return View(person);
}
```

---

## Integration Guide

### Step 1: Add Share Button to Component

1. Import required services and components
2. Inject `MatDialog` and `ShareService`
3. Add share button to template
4. Implement share handler method

### Step 2: Update Social Meta Tags

1. Inject `SocialMetaService` in component
2. Call appropriate `update*Meta()` method in `ngOnInit()`
3. Pass relevant data from component

### Step 3: Configure Server-Side Meta Tags (Optional)

1. Set `ViewData` properties in controller action
2. Supported properties:
   - `Title`
   - `Description`
   - `OgTitle`, `OgDescription`, `OgType`, `OgImage`, `OgUrl`
   - `TwitterTitle`, `TwitterDescription`, `TwitterImage`
   - `ArticleAuthor`, `ArticlePublishedTime`, `ArticleSection`
   - `ProfileFirstName`, `ProfileLastName`

### Step 4: Test Sharing

1. Open page in browser
2. Click share button
3. Test native share (mobile) or copy link (desktop)
4. Test social media links
5. Validate meta tags using:
   - Facebook Sharing Debugger: https://developers.facebook.com/tools/debug/
   - Twitter Card Validator: https://cards-dev.twitter.com/validator
   - LinkedIn Post Inspector: https://www.linkedin.com/post-inspector/

---

## Testing

### Manual Testing Checklist

- [ ] Test native share on mobile device (iOS Safari, Android Chrome)
- [ ] Test clipboard copy on desktop browsers (Chrome, Firefox, Edge, Safari)
- [ ] Test email share opens email client correctly
- [ ] Test Facebook share preview shows correct title, description, image
- [ ] Test Twitter share preview shows correct Twitter Card
- [ ] Test LinkedIn share preview shows correct Open Graph data
- [ ] Test WhatsApp share on mobile and desktop
- [ ] Test deep links from shared URLs navigate to correct pages
- [ ] Test URL tracking parameters (utm_source, utm_medium)
- [ ] Validate meta tags using social media debugging tools
- [ ] Test share dialog responsiveness on mobile and desktop
- [ ] Test copy link button shows checkmark visual feedback
- [ ] Test share dialog close functionality

### Browser Testing

| Browser | Native Share | Clipboard API | Email | Social Links |
|---------|--------------|---------------|-------|--------------|
| Chrome (Desktop) | ❌ | ✅ | ✅ | ✅ |
| Chrome (Mobile) | ✅ | ✅ | ✅ | ✅ |
| Firefox | ❌ | ✅ | ✅ | ✅ |
| Safari (Desktop) | ❌ | ✅ | ✅ | ✅ |
| Safari (iOS) | ✅ | ✅ | ✅ | ✅ |
| Edge | ❌ | ✅ | ✅ | ✅ |

### Social Media Debugging Tools

1. **Facebook Sharing Debugger**
   - URL: https://developers.facebook.com/tools/debug/
   - Test Open Graph tags
   - Clear cache and re-scrape
   - Preview how link appears on Facebook

2. **Twitter Card Validator**
   - URL: https://cards-dev.twitter.com/validator
   - Test Twitter Card tags
   - Preview how link appears on Twitter

3. **LinkedIn Post Inspector**
   - URL: https://www.linkedin.com/post-inspector/
   - Test Open Graph tags for LinkedIn
   - Preview how link appears on LinkedIn

4. **WhatsApp Link Preview**
   - Share link to WhatsApp Web
   - Check preview rendering
   - Note: WhatsApp uses Open Graph tags

---

## Best Practices

### 1. Meta Tag Management

- Always set meta tags from controller for server-side rendering
- Use `SocialMetaService` for client-side updates (SPA scenarios)
- Provide fallback values for all meta tags
- Keep descriptions concise (160 characters or less)
- Use high-quality images (1200x630px recommended for OG images)
- Always use absolute URLs for images

### 2. Image Guidelines

- **Recommended Size**: 1200x630px (1.91:1 aspect ratio)
- **Minimum Size**: 600x315px
- **Format**: JPEG or PNG
- **File Size**: < 1MB
- **Default Image**: Create a branded default image for pages without specific images

### 3. Description Guidelines

- **Length**: 55-160 characters (Facebook truncates at ~300 characters, Twitter at ~200)
- **Format**: Clear, concise, action-oriented
- **Keywords**: Include relevant keywords for SEO
- **Avoid**: Special characters, excessive emojis, all caps

### 4. URL Best Practices

- Use clean, readable URLs (RESTful structure)
- Add tracking parameters for analytics (utm_source, utm_medium, utm_campaign)
- Ensure URLs are publicly accessible (not behind authentication for public shares)
- Use HTTPS for all share links
- Consider URL shortening for very long URLs (optional enhancement)

### 5. Privacy Considerations

- Respect privacy settings (living vs. deceased persons)
- Provide option to share public vs. private views
- Implement token-based access for sensitive content
- Allow users to control what information is shareable
- Add GDPR-compliant privacy controls

### 6. Accessibility

- Ensure share buttons have proper ARIA labels
- Provide keyboard navigation for share dialog
- Use descriptive button text (not just icons)
- Test with screen readers
- Support high contrast mode

---

## Future Enhancements

### Planned Features (Not Yet Implemented)

1. **URL Shortening**
   - Integrate URL shortening service (Bitly, TinyURL, or custom)
   - Generate short URLs for long share links
   - Track click analytics on shortened URLs

2. **QR Code Generation**
   - Add QR code to ShareDialogComponent
   - Allow users to download QR code image
   - Useful for print materials and offline sharing

3. **Public Family Tree Views**
   - Backend implementation for public link generation
   - Secure token-based access control
   - Privacy settings (what data to show publicly)
   - Token expiration and management
   - Public view customization options

4. **Email Notification Links**
   - Deep links from email notifications
   - Personalized share tracking
   - Email-specific meta tags
   - Preview customization for email clients

5. **Share Analytics**
   - Track share counts per content
   - Track share channel performance
   - User engagement metrics
   - Popular content insights

6. **Custom Share Messages**
   - Allow users to customize share text
   - Pre-fill personalized messages
   - Support for multiple languages

7. **Batch Sharing**
   - Share multiple items at once
   - Create shareable collections
   - Album/story series sharing

8. **Social Media Integration**
   - Direct Facebook/Twitter API integration
   - Post to social media from app
   - Social media account linking
   - Scheduled social media posts

9. **Embed Codes**
   - Generate embeddable widgets
   - Family tree embeds for external sites
   - Customizable embed styles
   - Responsive embed sizing

10. **Share Templates**
    - Pre-designed share templates
    - Branded share graphics
    - Seasonal/holiday share designs
    - Family tree visual shares

---

## File Inventory

### New Files Created

1. `/ClientApp/src/app/shared/services/social-meta.service.ts` - Social meta tag service
2. `/ClientApp/src/app/shared/services/share.service.ts` - Sharing functionality service
3. `/ClientApp/src/app/shared/components/share-dialog/share-dialog.component.ts` - Share dialog component
4. `/ClientApp/src/app/shared/components/share-dialog/share-dialog.component.html` - Share dialog template
5. `/ClientApp/src/app/shared/components/share-dialog/share-dialog.component.scss` - Share dialog styles
6. `/docs/DeepLinkingAndSharing.md` - This documentation file

### Modified Files

1. `/Views/Shared/_Layout.cshtml` - Added Open Graph and Twitter Card meta tags
2. `/ClientApp/src/app/shared/shared.module.ts` - Added ShareDialogComponent to module

### Assets Required (Not Created)

1. `/wwwroot/assets/images/rushton-roots-og-image.jpg` - Default Open Graph image (1200x630px)
   - **TODO**: Create branded default image for social sharing
   - Should include RushtonRoots logo and tagline
   - Use professional design tools or hire designer
   - Follow image guidelines (1200x630px, <1MB, JPEG/PNG)

---

## Conclusion

Phase 12.4 successfully implements comprehensive deep linking and social media sharing functionality for RushtonRoots. The implementation provides:

✅ **Social Meta Tags**: Open Graph and Twitter Card support for rich previews  
✅ **Share Service**: Native share API, clipboard, email, and social media sharing  
✅ **Share Dialog**: User-friendly dialog with multiple sharing options  
✅ **Deep Linking**: All major pages support shareable URLs  
✅ **SEO-Friendly**: Automatic meta descriptions and image handling  
✅ **Mobile-First**: Native share support on mobile devices  
✅ **Extensible**: Ready for future enhancements (URL shortening, QR codes, analytics)

The next steps involve:
1. Manual testing of sharing functionality across browsers and devices
2. Creating the default Open Graph image asset
3. Integration with specific components (Person, Story, Recipe, Tradition, Photo)
4. Implementing backend support for public family tree links
5. Adding share analytics tracking
6. Testing with social media debugging tools

---

## References

- [Open Graph Protocol](https://ogp.me/)
- [Twitter Cards Documentation](https://developer.twitter.com/en/docs/twitter-for-websites/cards/overview/abouts-cards)
- [Facebook Sharing Best Practices](https://developers.facebook.com/docs/sharing/best-practices)
- [Web Share API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Share_API)
- [Clipboard API](https://developer.mozilla.org/en-US/docs/Web/API/Clipboard_API)
- [LinkedIn Post Inspector](https://www.linkedin.com/help/linkedin/answer/a521928)

---

**Document Owner**: Development Team  
**Last Review**: December 17, 2025  
**Next Review**: January 2026
