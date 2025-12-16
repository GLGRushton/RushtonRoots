# Wiki Module - Phase 7.1 Implementation

## Overview

The Wiki Module provides a comprehensive knowledge base system for RushtonRoots, allowing users to create, edit, and organize genealogical documentation in a structured wiki format.

## Components

### 1. WikiIndexComponent (`app-wiki-index`)

Main wiki navigation and article listing component.

**Features:**
- Grid and list view modes
- Article search with real-time filtering
- Category filtering
- Status filtering (Published, Draft, Archived)
- Multiple sort options (title, date, views)
- Responsive design with mobile support
- Empty state handling
- Article cards with metadata

**Usage:**
```html
<app-wiki-index></app-wiki-index>
```

**Sample Data:**
The component includes sample articles demonstrating:
- Different categories (Family History, Research Tips, Record Types, How-To Guides, Best Practices)
- Various statuses (Published, Draft, Archived)
- Locked articles
- Version tracking
- View counts

### 2. WikiArticleComponent (`app-wiki-article`)

Displays wiki articles with rendered markdown content and table of contents.

**Features:**
- Markdown content rendering with `marked` library
- Auto-generated table of contents from headings
- Sticky table of contents sidebar
- Hierarchical TOC with up to 6 heading levels
- Article metadata display (author, date, version, views)
- Breadcrumb navigation
- Print-friendly view
- Version history access
- Responsive design

**Usage:**
```html
<app-wiki-article [articleId]="1"></app-wiki-article>
<!-- or -->
<app-wiki-article [articleSlug]="'getting-started-genealogy'"></app-wiki-article>
```

**Inputs:**
- `articleId?: number` - Article ID to load
- `articleSlug?: string` - Article slug to load

**Markdown Styling:**
Comprehensive styling for markdown elements:
- Headings (h1-h6) with hierarchy
- Paragraphs with proper spacing
- Links in brand color (#2e7d32)
- Lists (ordered and unordered)
- Blockquotes with left border
- Code blocks with syntax highlighting
- Tables with striped rows
- Images with responsive sizing
- Horizontal rules

### 3. MarkdownEditorComponent (`app-markdown-editor`)

Full-featured markdown editor with live preview and toolbar.

**Features:**
- Rich toolbar with common markdown actions
- Side-by-side markdown and preview modes
- Fullscreen editing mode
- Keyboard shortcuts (Ctrl+B, Ctrl+I, Ctrl+K, Ctrl+Z, Ctrl+Y)
- Undo/Redo support (50 action history)
- Character, word, and line count
- Form integration via ControlValueAccessor
- Disabled state support
- Real-time preview rendering

**Usage in Forms:**
```typescript
// In component
form = this.fb.group({
  content: ['']
});

// In template
<app-markdown-editor formControlName="content"></app-markdown-editor>
```

**Usage as standalone:**
```html
<app-markdown-editor 
  [(ngModel)]="content"
  [placeholder]="'Write your article content...'"
  [minHeight]="'500px'"
  (contentChange)="onContentChange($event)">
</app-markdown-editor>
```

**Inputs:**
- `placeholder: string` - Placeholder text (default: 'Write your content in markdown...')
- `minHeight: string` - Minimum editor height (default: '400px')

**Outputs:**
- `contentChange: EventEmitter<string>` - Emits when content changes

**Toolbar Actions:**
1. **Text Formatting:**
   - Bold (`**text**`) - Ctrl+B
   - Italic (`*text*`) - Ctrl+I
   - Strikethrough (`~~text~~`)

2. **Headings:**
   - Heading 1-3 (`# ## ###`)

3. **Links & Media:**
   - Link (`[text](url)`) - Ctrl+K
   - Image (`![alt](url)`)
   - Code (`` `code` ``)
   - Code Block (` ```language ` `)

4. **Lists & Quotes:**
   - Blockquote (`> text`)
   - Unordered List (`- item`)
   - Ordered List (`1. item`)
   - Table (auto-generated template)

5. **Other:**
   - Horizontal Rule (`---`)
   - Undo - Ctrl+Z
   - Redo - Ctrl+Y/Ctrl+Shift+Z
   - Preview toggle
   - Fullscreen toggle

## Data Models

### WikiArticle
```typescript
interface WikiArticle {
  id: number;
  title: string;
  slug: string;
  content: string; // Markdown
  categoryId: number;
  categoryName?: string;
  authorId: string;
  authorName?: string;
  createdDate: Date;
  updatedDate: Date;
  publishedDate?: Date;
  status: WikiArticleStatus;
  version: number;
  viewCount: number;
  tags: string[];
  isLocked: boolean;
  lockedBy?: string;
  lockedUntil?: Date;
}
```

### WikiCategory
```typescript
interface WikiCategory {
  id: number;
  name: string;
  slug: string;
  description?: string;
  parentId?: number;
  articleCount: number;
  order: number;
  icon?: string;
  color?: string;
}
```

### TocEntry
```typescript
interface TocEntry {
  id: string;
  level: number; // 1-6 for h1-h6
  title: string;
  anchor: string;
  children?: TocEntry[];
}
```

## Dependencies

### NPM Packages
- **marked** (^14.1.5) - Markdown parsing library
- **@types/marked** - TypeScript definitions for marked

### Angular Material Components Used
- MatCardModule
- MatFormFieldModule
- MatInputModule
- MatSelectModule
- MatButtonModule
- MatIconModule
- MatChipsModule
- MatTooltipModule
- MatDividerModule
- MatProgressSpinnerModule
- MatMenuModule

## Integration with Backend

Currently using sample data. To integrate with backend:

1. **WikiIndexComponent:**
   - Replace `loadCategories()` with HTTP call to `/api/wiki/categories`
   - Replace `loadArticles()` with HTTP call to `/api/wiki/articles`
   - Add filters as query parameters

2. **WikiArticleComponent:**
   - Replace `loadArticle()` with HTTP call to `/api/wiki/articles/{id}` or `/api/wiki/articles/slug/{slug}`
   - Implement edit navigation
   - Implement version history

3. **MarkdownEditorComponent:**
   - Already form-compatible via ControlValueAccessor
   - Add image upload handler for toolbar image action
   - Implement autosave functionality

## Styling

All components follow the RushtonRoots design system:

- **Primary Color:** #2e7d32 (Forest Green)
- **Typography:** 'Segoe UI', Roboto, 'Helvetica Neue'
- **Spacing:** 8px grid system
- **Border Radius:** 4px, 8px for cards
- **Shadows:** Material Design elevation

### Responsive Breakpoints
- **Mobile:** < 600px
- **Tablet:** 600px - 960px
- **Desktop:** > 960px

## Future Enhancements

Phase 7.1 delivers the core wiki functionality. Future enhancements could include:

1. **Wiki Search Component** - Advanced search with highlighting
2. **Wiki Category Navigation** - Tree-view category browser
3. **Version History UI** - Compare versions and revert changes
4. **Collaborative Editing** - Real-time editing indicators
5. **Comments** - Article discussion threads
6. **Bookmarks** - Save favorite articles
7. **Export** - PDF/Word export functionality
8. **Templates** - Article templates for common types

## Testing

To test the wiki module:

1. **Wiki Index:**
   ```html
   <app-wiki-index></app-wiki-index>
   ```
   - Test filtering by category, status
   - Test search functionality
   - Test sort options
   - Test grid/list view toggle

2. **Wiki Article:**
   ```html
   <app-wiki-article></app-wiki-article>
   ```
   - Verify markdown rendering
   - Test table of contents navigation
   - Test responsive layout
   - Test print view

3. **Markdown Editor:**
   ```html
   <app-markdown-editor [(ngModel)]="testContent"></app-markdown-editor>
   ```
   - Test all toolbar actions
   - Test keyboard shortcuts
   - Test undo/redo
   - Test preview mode
   - Test fullscreen mode

## Accessibility

All components meet WCAG 2.1 AA standards:

- Keyboard navigation support
- ARIA labels on interactive elements
- Focus indicators
- Semantic HTML
- Color contrast ratios
- Screen reader compatibility

## Browser Support

- Chrome/Edge 90+
- Firefox 88+
- Safari 14+
- Mobile browsers (iOS Safari, Chrome Mobile)

## Phase 7.1 Completion

✅ **Delivered Components:**
- WikiIndexComponent - Main wiki navigation and article listing
- WikiArticleComponent - Article display with table of contents
- MarkdownEditorComponent - Full-featured markdown editor

✅ **Key Features:**
- Markdown editing and rendering
- Table of contents generation
- Article organization by categories
- Search and filtering
- Responsive design
- Material Design integration

✅ **Success Criteria Met:**
- Wiki is easy to navigate ✅
- Wiki is easy to edit ✅
- Components are reusable ✅
- Full markdown support ✅
- Professional UI/UX ✅

---

**Last Updated:** December 2024  
**Module Version:** 1.0  
**Phase:** 7.1 Complete
