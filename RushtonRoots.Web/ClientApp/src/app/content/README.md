# Content Module - Phase 7.2: Recipes, Stories, & Traditions

## Overview

The Content Module provides comprehensive functionality for managing and displaying family recipes, stories, and traditions. This module implements Phase 7.2 of the UI Design Plan with beautiful card-based layouts, masonry grid displays, filtering capabilities, and rich detail views.

## Features

### Recipe Management
- **RecipeCardComponent**: Beautiful card display with ratings, difficulty badges, and metadata
- **RecipeDetailsComponent**: Full recipe details with:
  - Tabbed interface (Recipe, Ratings & Reviews, Comments)
  - Ingredient list with serving size adjustment
  - Step-by-step instructions
  - Nutrition information
  - Print-friendly view
  - Rating system (1-5 stars)
  - Comment system with nested replies

### Story Management
- **StoryCardComponent**: Engaging card display with:
  - Event date and location
  - Related people chips
  - Media count badges
  - Featured highlighting

### Tradition Management
- **TraditionCardComponent**: Tradition cards featuring:
  - Frequency badges (Daily, Weekly, Monthly, Yearly, Occasional)
  - Season and timing information
  - Related people and recipes
  - Historical context (year started)

### Content Grid
- **ContentGridComponent**: Universal masonry grid layout with:
  - Responsive columns (1-4 based on screen size)
  - Search and filter capabilities
  - Multiple sort options
  - Category and tag filtering
  - Featured content highlighting

## Components

### RecipeCardComponent

Displays a recipe in a compact card format.

**Inputs:**
- `recipe: Recipe` - Recipe to display
- `elevation: number` - Card elevation (0, 2, 4, 8) - default: 2
- `truncateDescription: boolean` - Whether to truncate description - default: true
- `showActions: boolean` - Whether to show action buttons - default: true
- `canEdit: boolean` - Whether user can edit - default: false

**Outputs:**
- `viewRecipe: EventEmitter<number>` - Emits recipe ID when view clicked
- `editRecipe: EventEmitter<number>` - Emits recipe ID when edit clicked
- `deleteRecipe: EventEmitter<number>` - Emits recipe ID when delete clicked
- `printRecipe: EventEmitter<number>` - Emits recipe ID when print clicked
- `rateRecipe: EventEmitter<{recipeId: number, rating: number}>` - Emits when rated

**Features:**
- Star rating display (1-5 stars)
- Difficulty badge (Easy, Medium, Hard)
- Recipe metadata (time, servings, ingredients count)
- Category, origin, and cuisine chips
- Featured badge
- Author information with avatar
- View count
- Responsive design

### RecipeDetailsComponent

Displays full recipe details with interactive features.

**Inputs:**
- `recipe: Recipe` - Recipe to display
- `canEdit: boolean` - Whether user can edit - default: false
- `userHasRated: boolean` - Whether user has rated - default: false
- `userRating: number` - User's current rating - default: 0

**Outputs:**
- `editRecipe: EventEmitter<number>` - Emits recipe ID when edit clicked
- `deleteRecipe: EventEmitter<number>` - Emits recipe ID when delete clicked
- `submitRating: EventEmitter<{recipeId, rating, review}>` - Emits rating submission
- `submitComment: EventEmitter<{recipeId, comment, parentCommentId?}>` - Emits comment

**Features:**
- Tabbed interface (Recipe, Ratings & Reviews, Comments)
- Serving size adjuster with automatic quantity scaling
- Ingredient checklist
- Step-by-step instructions with optional images
- Nutrition information panel
- Print-friendly view (triggered by print button)
- Rating submission with optional review text
- Comment system with threaded replies
- Recipe metadata display
- Responsive design

### StoryCardComponent

Displays a family story in a card format.

**Inputs:**
- `story: Story` - Story to display
- `elevation: number` - Card elevation (0, 2, 4, 8) - default: 2
- `truncateSummary: boolean` - Whether to truncate summary - default: true
- `showActions: boolean` - Whether to show action buttons - default: true
- `canEdit: boolean` - Whether user can edit - default: false

**Outputs:**
- `viewStory: EventEmitter<number>` - Emits story ID when view clicked
- `editStory: EventEmitter<number>` - Emits story ID when edit clicked
- `deleteStory: EventEmitter<number>` - Emits story ID when delete clicked
- `viewPerson: EventEmitter<number>` - Emits person ID when person clicked

**Features:**
- Event date and location display
- Summary with truncation
- Related people chips with avatars (clickable)
- Media count badges (photos, videos)
- Category and tag chips
- Featured badge
- Author information
- Responsive design

### TraditionCardComponent

Displays a family tradition in a card format.

**Inputs:**
- `tradition: Tradition` - Tradition to display
- `elevation: number` - Card elevation (0, 2, 4, 8) - default: 2
- `truncateDescription: boolean` - Whether to truncate description - default: true
- `showActions: boolean` - Whether to show action buttons - default: true
- `canEdit: boolean` - Whether user can edit - default: false

**Outputs:**
- `viewTradition: EventEmitter<number>` - Emits tradition ID when view clicked
- `editTradition: EventEmitter<number>` - Emits tradition ID when edit clicked
- `deleteTradition: EventEmitter<number>` - Emits tradition ID when delete clicked
- `viewPerson: EventEmitter<number>` - Emits person ID when person clicked
- `viewRecipe: EventEmitter<number>` - Emits recipe ID when recipe clicked

**Features:**
- Frequency badge (Daily, Weekly, Monthly, Yearly, Occasional)
- Season and month information
- Location and historical context (year started)
- Related people participants with avatars
- Related recipes count
- Category and tag chips
- Media information
- Featured badge
- Responsive design

### ContentGridComponent

Universal masonry grid layout for all content types.

**Inputs:**
- `contentType: ContentType` - Type of content (Recipe, Story, Tradition)
- `items: (Recipe | Story | Tradition)[]` - Items to display
- `showFilters: boolean` - Whether to show filters - default: true
- `showSort: boolean` - Whether to show sort options - default: true
- `canEdit: boolean` - Whether user can edit - default: false
- `filters: ContentSearchFilters` - Current filters

**Outputs:**
- `filtersChange: EventEmitter<ContentSearchFilters>` - Emits when filters change
- `viewItem: EventEmitter<{type, id}>` - Emits when item viewed
- `editItem: EventEmitter<{type, id}>` - Emits when item edited
- `deleteItem: EventEmitter<{type, id}>` - Emits when item deleted

**Features:**
- Responsive masonry grid (1-4 columns based on screen size)
- Search by title, description, or tags
- Sort options: Recently Published, Oldest First, Title A-Z/Z-A, Most Viewed, Highest Rated, Featured First
- Advanced filtering: Featured only, Category
- Filter chips with clear functionality
- Empty state messaging
- Loading state with spinner
- Results summary
- Dynamic column calculation on window resize

## Data Models

### Recipe
```typescript
interface Recipe extends BaseContent {
  description: string;
  prepTime: number; // minutes
  cookTime: number; // minutes
  totalTime: number; // minutes
  servings: number;
  difficulty: RecipeDifficulty; // Easy, Medium, Hard
  cuisine?: string;
  imageUrl?: string;
  ingredients: RecipeIngredient[];
  instructions: RecipeInstruction[];
  nutritionInfo?: NutritionInfo;
  ratings: RecipeRating[];
  averageRating: number;
  comments: RecipeComment[];
  origin?: string; // e.g., "Grandma's Recipe"
  yearOrigin?: number;
}
```

### Story
```typescript
interface Story extends BaseContent {
  summary: string;
  content: string; // Markdown supported
  imageUrl?: string;
  location?: string;
  dateOfEvent?: Date;
  relatedPeople: StoryPerson[];
  media: StoryMedia[];
}
```

### Tradition
```typescript
interface Tradition extends BaseContent {
  description: string;
  content: string; // Markdown supported
  imageUrl?: string;
  frequency: TraditionFrequency; // Daily, Weekly, Monthly, Yearly, Occasional
  season?: string;
  monthsActive?: number[]; // 1-12
  location?: string;
  startedYear?: number;
  relatedPeople: StoryPerson[];
  relatedRecipes?: number[];
  media: StoryMedia[];
}
```

## Usage Examples

### Using RecipeCardComponent in TypeScript

```typescript
import { Recipe, RecipeDifficulty, ContentStatus } from './content/models/content.model';

// Sample recipe data
const recipe: Recipe = {
  id: 1,
  title: "Grandma's Apple Pie",
  slug: "grandmas-apple-pie",
  description: "A classic apple pie recipe passed down through generations.",
  authorId: "user123",
  authorName: "Jane Doe",
  createdDate: new Date(),
  updatedDate: new Date(),
  publishedDate: new Date(),
  status: ContentStatus.Published,
  tags: ['dessert', 'holiday', 'traditional'],
  categoryId: 1,
  categoryName: "Desserts",
  viewCount: 152,
  featured: true,
  prepTime: 30,
  cookTime: 60,
  totalTime: 90,
  servings: 8,
  difficulty: RecipeDifficulty.Medium,
  cuisine: "American",
  imageUrl: "/assets/recipes/apple-pie.jpg",
  ingredients: [
    { id: 1, quantity: "6-8", unit: "cups", ingredient: "sliced apples", order: 1 },
    { id: 2, quantity: "3/4", unit: "cup", ingredient: "sugar", order: 2 }
  ],
  instructions: [
    { id: 1, stepNumber: 1, instruction: "Preheat oven to 425°F." },
    { id: 2, stepNumber: 2, instruction: "Mix apples with sugar and spices." }
  ],
  ratings: [],
  averageRating: 4.5,
  comments: [],
  origin: "Grandma Rose",
  yearOrigin: 1952
};

// Handle events
onViewRecipe(id: number) {
  console.log('View recipe:', id);
}

onEditRecipe(id: number) {
  console.log('Edit recipe:', id);
}
```

### Using Components in Razor Views

```html
<!-- Recipe Card -->
<app-recipe-card
  recipe='@Json.Serialize(Model.Recipe)'
  can-edit="true"
  elevation="2">
</app-recipe-card>

<!-- Recipe Details -->
<app-recipe-details
  recipe='@Json.Serialize(Model.Recipe)'
  can-edit="@Model.CanEdit"
  user-has-rated="@Model.UserHasRated"
  user-rating="@Model.UserRating">
</app-recipe-details>

<!-- Story Card -->
<app-story-card
  story='@Json.Serialize(Model.Story)'
  can-edit="true">
</app-story-card>

<!-- Tradition Card -->
<app-tradition-card
  tradition='@Json.Serialize(Model.Tradition)'
  can-edit="true">
</app-tradition-card>

<!-- Content Grid (Recipes) -->
<app-content-grid
  content-type="recipe"
  items='@Json.Serialize(Model.Recipes)'
  show-filters="true"
  show-sort="true"
  can-edit="@Model.CanEdit">
</app-content-grid>
```

### Using ContentGridComponent

```typescript
import { ContentType } from './content/models/content.model';

// In component
contentType = ContentType.Recipe;
recipes: Recipe[] = [...]; // Load from service

onFiltersChange(filters: ContentSearchFilters) {
  console.log('Filters changed:', filters);
  // Update backend query or apply client-side filtering
}

onViewItem(event: { type: ContentType, id: number }) {
  console.log('View item:', event.type, event.id);
  // Navigate to detail view
}
```

## Styling

All components use Material Design and follow the RushtonRoots theme:
- Primary color: #2e7d32 (Forest Green)
- Accent color: #66bb6a (Light Green)
- Typography: Segoe UI, Roboto, Helvetica Neue
- 8px grid spacing system
- Responsive breakpoints: 600px, 960px, 1280px, 1920px

## Print Styles

RecipeDetailsComponent includes print-optimized styles:
- Simplified layout
- Hidden interactive elements
- Page break optimization
- Clear ingredient and instruction lists
- Included nutrition information

## Accessibility

All components include:
- Proper ARIA labels
- Keyboard navigation support
- Screen reader friendly markup
- Sufficient color contrast
- Focus indicators

## Responsive Design

Components adapt to different screen sizes:
- **Mobile (<600px)**: Single column, stacked layout
- **Tablet (600-960px)**: 2 columns
- **Desktop (960-1280px)**: 3 columns
- **Large Desktop (>1280px)**: 4 columns

## Integration with Backend

### C# Models

Create corresponding C# models in `RushtonRoots.Domain/UI/Models/`:

```csharp
public class RecipeViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public int TotalTime { get; set; }
    public int Servings { get; set; }
    public RecipeDifficulty Difficulty { get; set; }
    public string? Cuisine { get; set; }
    public string? ImageUrl { get; set; }
    public List<RecipeIngredientViewModel> Ingredients { get; set; }
    public List<RecipeInstructionViewModel> Instructions { get; set; }
    // ... other properties
}
```

### API Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    [HttpGet]
    public async Task<IActionResult> GetRecipes()
    {
        var recipes = await _recipeService.GetAllAsync();
        return Ok(recipes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipe(int id)
    {
        var recipe = await _recipeService.GetByIdAsync(id);
        if (recipe == null) return NotFound();
        return Ok(recipe);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeRequest request)
    {
        var recipe = await _recipeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }
}
```

## Testing

Component tests should cover:
- Rendering with different data
- User interactions (clicks, form inputs)
- Output events
- Responsive behavior
- Print functionality
- Filter and sort operations

## Future Enhancements

- Recipe scaling with ingredient substitutions
- Social sharing capabilities
- Recipe collections/cookbooks
- Story timeline view
- Tradition calendar integration
- Import/export recipes (JSON, PDF)
- Recipe video support
- Multi-language support

## Dependencies

- @angular/core: ^19.0.0
- @angular/material: ^19.0.0
- @angular/cdk: ^19.0.0
- @angular/forms: ^19.0.0
- @angular/common: ^19.0.0

## License

Part of the RushtonRoots family tree application.

## Contributors

- Development Team
- Phase 7.2 Implementation: December 2025
