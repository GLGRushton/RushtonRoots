# Phase 7.1 Wiki Module Architecture

## Module Structure

```
wiki/
├── models/
│   └── wiki.model.ts                    # TypeScript interfaces and types
├── components/
│   ├── wiki-index/                      # Article listing and navigation
│   │   ├── wiki-index.component.ts
│   │   ├── wiki-index.component.html
│   │   └── wiki-index.component.scss
│   ├── wiki-article/                    # Article display with TOC
│   │   ├── wiki-article.component.ts
│   │   ├── wiki-article.component.html
│   │   └── wiki-article.component.scss
│   └── markdown-editor/                 # Markdown editor with toolbar
│       ├── markdown-editor.component.ts
│       ├── markdown-editor.component.html
│       └── markdown-editor.component.scss
├── wiki.module.ts                       # Module definition
├── README.md                            # Documentation
└── PHASE_7_1_DEMO.html                 # Demo page

## Data Flow

### WikiIndexComponent
```
User Input → Search/Filter → Apply Filters → Update Display
     ↓             ↓               ↓              ↓
  Search      Category      Sort Articles    Grid/List
  Debounce    Selection                       View
```

### WikiArticleComponent
```
Article ID/Slug → Load Article → Parse Markdown → Generate TOC
                       ↓              ↓              ↓
                   Display         Render        Navigate
                   Metadata        HTML          Sections
```

### MarkdownEditorComponent
```
User Input → Toolbar Actions → Update Content → Parse → Preview
     ↓            ↓                  ↓           ↓         ↓
 Typing      Formatting         Undo/Redo    Marked    Render
 Events      Commands            Stack      Library    HTML
```

## Component Communication

```
Parent Component (Razor View)
          ↓
     Angular Elements
          ↓
   ┌──────┴──────┐
   ↓             ↓             ↓
WikiIndex   WikiArticle   MarkdownEditor
   ↓             ↓             ↓
 Sample      Sample      ControlValueAccessor
  Data        Data         Form Binding
```

## Dependencies

### External Libraries
- **marked** (v14.1.5) - Markdown parsing and rendering
- **@types/marked** - TypeScript definitions

### Angular Material Components
- MatCardModule - Container layouts
- MatFormFieldModule - Input fields
- MatInputModule - Text inputs
- MatSelectModule - Dropdowns
- MatButtonModule - Action buttons
- MatIconModule - Icons
- MatChipsModule - Tags and filters
- MatTooltipModule - Helpful hints
- MatDividerModule - Visual separation
- MatProgressSpinnerModule - Loading states
- MatMenuModule - Action menus
- MatBadgeModule - Count indicators
- MatExpansionModule - Expandable panels
- MatListModule - List displays

## TypeScript Models

### Core Models
1. **WikiArticle** - Complete article with metadata
2. **WikiCategory** - Article categorization
3. **WikiArticleVersion** - Version history
4. **TocEntry** - Table of contents entries
5. **WikiSearchResult** - Search results with highlights
6. **WikiSearchFilters** - Search/filter criteria
7. **ActiveEditor** - Collaborative editing users
8. **MarkdownToolbarButton** - Editor toolbar buttons

### Enums
- **WikiArticleStatus** - draft | published | archived
- **MarkdownToolbarAction** - 17 editor actions

### Constants
- **WIKI_SORT_OPTIONS** - 8 sorting options
- **MARKDOWN_TOOLBAR_BUTTONS** - 17 toolbar actions in 7 groups

## Feature Matrix

| Component | Search | Filter | Sort | CRUD | Preview | Responsive |
|-----------|--------|--------|------|------|---------|------------|
| WikiIndex | ✅     | ✅     | ✅   | View | N/A     | ✅         |
| WikiArticle| N/A   | N/A    | N/A  | View | ✅      | ✅         |
| MarkdownEditor| N/A | N/A   | N/A  | Edit | ✅      | ✅         |

## Responsive Breakpoints

- **Mobile:** < 600px
  - Single column layout
  - Stacked filters
  - Mobile TOC handling
  
- **Tablet:** 600px - 960px
  - 1-2 column grid
  - Adaptive toolbar
  
- **Desktop:** > 960px
  - 3-4 column grid
  - Full toolbar
  - Sticky TOC sidebar

## Accessibility Features

- ✅ Keyboard navigation
- ✅ ARIA labels
- ✅ Focus indicators
- ✅ Semantic HTML
- ✅ Color contrast (WCAG AA)
- ✅ Screen reader support

## Performance Optimizations

- **Debounced Search** - 300ms delay
- **Lazy Rendering** - Only visible content
- **Client-side Filtering** - Fast response
- **Undo/Redo Limit** - 50 actions max
- **Component Caching** - Reusable instances

## Integration Points

### Backend API (To Be Implemented)
```
GET    /api/wiki/articles           - List articles
GET    /api/wiki/articles/{id}      - Get article by ID
GET    /api/wiki/articles/slug/{s}  - Get article by slug
POST   /api/wiki/articles           - Create article
PUT    /api/wiki/articles/{id}      - Update article
DELETE /api/wiki/articles/{id}      - Delete article
GET    /api/wiki/categories         - List categories
GET    /api/wiki/versions/{id}      - Get article versions
```

### Razor Views
```html
<!-- Wiki Index Page -->
<app-wiki-index></app-wiki-index>

<!-- Wiki Article Page -->
<app-wiki-article article-id="1"></app-wiki-article>

<!-- Create/Edit Article Page -->
<form>
  <app-markdown-editor formControlName="content"></app-markdown-editor>
</form>
```

## Testing Strategy

### Unit Tests (To Be Added)
- Component rendering
- Search/filter logic
- Markdown parsing
- Toolbar actions
- Form validation

### Integration Tests (To Be Added)
- API communication
- Component interaction
- User workflows

### E2E Tests (To Be Added)
- Article creation flow
- Search and filter
- Markdown editing
- Article viewing

## Future Enhancements

### Phase 7.2+ Features
1. **Advanced Search** - Full-text search with highlighting
2. **Category Tree** - Hierarchical category navigation
3. **Version Compare** - Side-by-side version comparison
4. **Collaborative Editing** - Real-time multi-user editing
5. **Comments** - Article discussion threads
6. **Bookmarks** - Save favorite articles
7. **Export** - PDF/Word export
8. **Templates** - Pre-defined article templates
9. **Media Library** - Image/file management
10. **Permissions** - Granular access control

## Browser Support

- ✅ Chrome/Edge 90+
- ✅ Firefox 88+
- ✅ Safari 14+
- ✅ Mobile browsers (iOS Safari, Chrome Mobile)

## Build Configuration

### Angular Configuration
```json
{
  "outputPath": "../wwwroot/dist",
  "optimization": true,
  "sourceMap": false,
  "budgets": [
    {
      "type": "initial",
      "maximumWarning": "500kb",
      "maximumError": "1mb"
    }
  ]
}
```

### NPM Scripts
```json
{
  "build": "ng build",
  "watch": "ng build --watch --configuration development"
}
```

## Deployment Checklist

- [x] TypeScript compiled without errors
- [x] All components registered as Angular Elements
- [x] Material Design components imported
- [x] Sample data included for testing
- [x] Documentation completed
- [x] Build successful
- [ ] Backend API implemented
- [ ] Authentication integrated
- [ ] Production deployment

## Maintenance Notes

### Adding New Categories
Update `sampleCategories` array in `WikiIndexComponent` with:
```typescript
{
  id: number,
  name: string,
  slug: string,
  description: string,
  articleCount: number,
  order: number,
  icon: string,    // Material icon name
  color: string    // Hex color code
}
```

### Adding Toolbar Actions
Update `MARKDOWN_TOOLBAR_BUTTONS` in `wiki.model.ts` and implement action handler in `MarkdownEditorComponent.onToolbarAction()`.

### Customizing Markdown Styles
Edit `.markdown-content` styles in:
- `wiki-article.component.scss`
- `markdown-editor.component.scss`

## Security Considerations

- ✅ XSS Prevention - Markdown sanitized by marked library
- ✅ CSRF Protection - Use ASP.NET Core anti-forgery tokens
- ⚠️  Input Validation - Implement server-side validation
- ⚠️  Authorization - Implement role-based access control
- ⚠️  Rate Limiting - Prevent abuse of create/edit operations

## Monitoring & Analytics

### Metrics to Track
- Article views
- Edit frequency
- Search queries
- User engagement
- Error rates
- Load times

---

**Version:** 1.0  
**Last Updated:** December 2024  
**Status:** Phase 7.1 Complete ✅
