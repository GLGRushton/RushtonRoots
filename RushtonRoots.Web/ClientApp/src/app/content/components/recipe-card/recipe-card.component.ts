import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Recipe } from '../../models/content.model';

/**
 * RecipeCardComponent
 * Displays a recipe in a card format with image, title, metadata, and actions
 */
@Component({
  selector: 'app-recipe-card',
  standalone: false,
  templateUrl: './recipe-card.component.html',
  styleUrls: ['./recipe-card.component.scss']
})
export class RecipeCardComponent {
  /**
   * Recipe to display
   */
  @Input() recipe!: Recipe;

  /**
   * Card elevation level (0, 2, 4, 8)
   */
  @Input() elevation: number = 2;

  /**
   * Whether to show the full description or truncate it
   */
  @Input() truncateDescription: boolean = true;

  /**
   * Whether to show action buttons
   */
  @Input() showActions: boolean = true;

  /**
   * Whether the user can edit this recipe
   */
  @Input() canEdit: boolean = false;

  /**
   * Event emitted when the view button is clicked
   */
  @Output() viewRecipe = new EventEmitter<number>();

  /**
   * Event emitted when the edit button is clicked
   */
  @Output() editRecipe = new EventEmitter<number>();

  /**
   * Event emitted when the delete button is clicked
   */
  @Output() deleteRecipe = new EventEmitter<number>();

  /**
   * Event emitted when the print button is clicked
   */
  @Output() printRecipe = new EventEmitter<number>();

  /**
   * Event emitted when a user rates the recipe
   */
  @Output() rateRecipe = new EventEmitter<{ recipeId: number, rating: number }>();

  /**
   * Handle view action
   */
  onView(): void {
    this.viewRecipe.emit(this.recipe.id);
  }

  /**
   * Handle edit action
   */
  onEdit(): void {
    this.editRecipe.emit(this.recipe.id);
  }

  /**
   * Handle delete action
   */
  onDelete(): void {
    this.deleteRecipe.emit(this.recipe.id);
  }

  /**
   * Handle print action
   */
  onPrint(): void {
    this.printRecipe.emit(this.recipe.id);
  }

  /**
   * Handle rating action
   */
  onRate(rating: number): void {
    this.rateRecipe.emit({ recipeId: this.recipe.id, rating });
  }

  /**
   * Get difficulty color
   */
  getDifficultyColor(): string {
    switch (this.recipe.difficulty) {
      case 'easy': return '#4caf50';
      case 'medium': return '#ff9800';
      case 'hard': return '#f44336';
      default: return '#757575';
    }
  }

  /**
   * Get difficulty label
   */
  getDifficultyLabel(): string {
    switch (this.recipe.difficulty) {
      case 'easy': return 'Easy';
      case 'medium': return 'Medium';
      case 'hard': return 'Hard';
      default: return 'Unknown';
    }
  }

  /**
   * Get star array for rating display
   */
  getStars(): number[] {
    return [1, 2, 3, 4, 5];
  }

  /**
   * Check if star should be filled
   */
  isStarFilled(star: number): boolean {
    return star <= Math.round(this.recipe.averageRating);
  }

  /**
   * Format time in hours and minutes
   */
  formatTime(minutes: number): string {
    if (minutes < 60) {
      return `${minutes}m`;
    }
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
  }

  /**
   * Get truncated description
   */
  getDescription(): string {
    if (!this.truncateDescription) {
      return this.recipe.description;
    }
    return this.recipe.description.length > 150
      ? this.recipe.description.substring(0, 150) + '...'
      : this.recipe.description;
  }
}
