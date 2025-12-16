/**
 * Content Models
 * TypeScript interfaces and types for recipe, story, and tradition functionality
 */

/**
 * Base content interface with common properties
 */
export interface BaseContent {
  id: number;
  title: string;
  slug: string;
  authorId: string;
  authorName: string;
  authorAvatar?: string;
  createdDate: Date;
  updatedDate: Date;
  publishedDate?: Date;
  status: ContentStatus;
  tags: string[];
  categoryId: number;
  categoryName?: string;
  viewCount: number;
  featured: boolean;
}

/**
 * Content status enum
 */
export enum ContentStatus {
  Draft = 'draft',
  Published = 'published',
  Archived = 'archived'
}

/**
 * Represents a family recipe
 */
export interface Recipe extends BaseContent {
  description: string;
  prepTime: number; // in minutes
  cookTime: number; // in minutes
  totalTime: number; // in minutes
  servings: number;
  difficulty: RecipeDifficulty;
  cuisine?: string;
  imageUrl?: string;
  ingredients: RecipeIngredient[];
  instructions: RecipeInstruction[];
  nutritionInfo?: NutritionInfo;
  ratings: RecipeRating[];
  averageRating: number;
  comments: RecipeComment[];
  origin?: string; // Who the recipe is from (e.g., "Grandma's Recipe")
  yearOrigin?: number; // When the recipe started in the family
}

/**
 * Recipe difficulty levels
 */
export enum RecipeDifficulty {
  Easy = 'easy',
  Medium = 'medium',
  Hard = 'hard'
}

/**
 * Represents a recipe ingredient
 */
export interface RecipeIngredient {
  id: number;
  quantity: string;
  unit: string;
  ingredient: string;
  notes?: string;
  order: number;
}

/**
 * Represents a recipe instruction step
 */
export interface RecipeInstruction {
  id: number;
  stepNumber: number;
  instruction: string;
  imageUrl?: string;
  duration?: number; // optional duration in minutes for this step
}

/**
 * Nutrition information for a recipe
 */
export interface NutritionInfo {
  calories?: number;
  protein?: number;
  carbohydrates?: number;
  fat?: number;
  fiber?: number;
  sugar?: number;
  sodium?: number;
  servingSize?: string;
}

/**
 * Recipe rating by a user
 */
export interface RecipeRating {
  id: number;
  recipeId: number;
  userId: string;
  userName: string;
  rating: number; // 1-5 stars
  review?: string;
  createdDate: Date;
}

/**
 * Recipe comment
 */
export interface RecipeComment {
  id: number;
  recipeId: number;
  userId: string;
  userName: string;
  userAvatar?: string;
  comment: string;
  createdDate: Date;
  updatedDate?: Date;
  parentCommentId?: number; // For nested replies
  replies?: RecipeComment[];
}

/**
 * Represents a family story
 */
export interface Story extends BaseContent {
  summary: string;
  content: string; // Full story content (supports markdown)
  imageUrl?: string;
  location?: string;
  dateOfEvent?: Date; // When the story took place
  relatedPeople: StoryPerson[]; // People mentioned in the story
  media: StoryMedia[]; // Photos, videos, documents
}

/**
 * Person related to a story
 */
export interface StoryPerson {
  personId: number;
  personName: string;
  personAvatar?: string;
  role?: string; // Their role in the story (e.g., "protagonist", "narrator")
}

/**
 * Media attached to a story
 */
export interface StoryMedia {
  id: number;
  type: MediaType;
  url: string;
  thumbnailUrl?: string;
  caption?: string;
  order: number;
}

/**
 * Media type enum
 */
export enum MediaType {
  Photo = 'photo',
  Video = 'video',
  Audio = 'audio',
  Document = 'document'
}

/**
 * Represents a family tradition
 */
export interface Tradition extends BaseContent {
  description: string;
  content: string; // Detailed description (supports markdown)
  imageUrl?: string;
  frequency: TraditionFrequency;
  season?: string; // e.g., "Winter", "Summer"
  monthsActive?: number[]; // Month numbers when tradition is active (1-12)
  location?: string;
  startedYear?: number; // When the tradition started in the family
  relatedPeople: StoryPerson[]; // People who participate
  relatedRecipes?: number[]; // Recipe IDs associated with this tradition
  media: StoryMedia[];
}

/**
 * Tradition frequency
 */
export enum TraditionFrequency {
  Daily = 'daily',
  Weekly = 'weekly',
  Monthly = 'monthly',
  Yearly = 'yearly',
  Occasional = 'occasional'
}

/**
 * Content category
 */
export interface ContentCategory {
  id: number;
  name: string;
  slug: string;
  type: ContentType; // recipes, stories, or traditions
  description?: string;
  color?: string;
  icon?: string;
  order: number;
}

/**
 * Content type enum
 */
export enum ContentType {
  Recipe = 'recipe',
  Story = 'story',
  Tradition = 'tradition'
}

/**
 * Search filters for content
 */
export interface ContentSearchFilters {
  searchText: string;
  contentType?: ContentType;
  categoryId?: number;
  tags?: string[];
  authorId?: string;
  status?: ContentStatus;
  featured?: boolean;
}

/**
 * Sort options for content
 */
export interface ContentSortOption {
  value: string;
  label: string;
}

/**
 * Sort options configuration
 */
export const CONTENT_SORT_OPTIONS: ContentSortOption[] = [
  { value: 'publishedDate-desc', label: 'Recently Published' },
  { value: 'publishedDate-asc', label: 'Oldest First' },
  { value: 'title-asc', label: 'Title (A-Z)' },
  { value: 'title-desc', label: 'Title (Z-A)' },
  { value: 'viewCount-desc', label: 'Most Viewed' },
  { value: 'rating-desc', label: 'Highest Rated' },
  { value: 'featured', label: 'Featured First' }
];

/**
 * Recipe difficulty configuration
 */
export const RECIPE_DIFFICULTY_CONFIG = {
  [RecipeDifficulty.Easy]: {
    label: 'Easy',
    color: '#4caf50',
    icon: 'sentiment_satisfied'
  },
  [RecipeDifficulty.Medium]: {
    label: 'Medium',
    color: '#ff9800',
    icon: 'sentiment_neutral'
  },
  [RecipeDifficulty.Hard]: {
    label: 'Hard',
    color: '#f44336',
    icon: 'sentiment_very_dissatisfied'
  }
};

/**
 * Tradition frequency configuration
 */
export const TRADITION_FREQUENCY_CONFIG = {
  [TraditionFrequency.Daily]: { label: 'Daily', icon: 'today' },
  [TraditionFrequency.Weekly]: { label: 'Weekly', icon: 'date_range' },
  [TraditionFrequency.Monthly]: { label: 'Monthly', icon: 'calendar_month' },
  [TraditionFrequency.Yearly]: { label: 'Yearly', icon: 'event' },
  [TraditionFrequency.Occasional]: { label: 'Occasional', icon: 'event_available' }
};
