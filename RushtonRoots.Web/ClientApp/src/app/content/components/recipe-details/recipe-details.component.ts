import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Recipe, RecipeComment, RecipeRating } from '../../models/content.model';

/**
 * RecipeDetailsComponent
 * Displays full recipe details with ingredients, instructions, ratings, and comments
 * Includes print-friendly view functionality
 */
@Component({
  selector: 'app-recipe-details',
  standalone: false,
  templateUrl: './recipe-details.component.html',
  styleUrls: ['./recipe-details.component.scss']
})
export class RecipeDetailsComponent implements OnInit {
  /**
   * Recipe to display
   */
  @Input() recipe!: Recipe;

  /**
   * Whether the user can edit this recipe
   */
  @Input() canEdit: boolean = false;

  /**
   * Whether the user has already rated this recipe
   */
  @Input() userHasRated: boolean = false;

  /**
   * Current user's rating (if they've rated)
   */
  @Input() userRating: number = 0;

  /**
   * Event emitted when user wants to edit the recipe
   */
  @Output() editRecipe = new EventEmitter<number>();

  /**
   * Event emitted when user wants to delete the recipe
   */
  @Output() deleteRecipe = new EventEmitter<number>();

  /**
   * Event emitted when user submits a rating
   */
  @Output() submitRating = new EventEmitter<{ recipeId: number, rating: number, review: string }>();

  /**
   * Event emitted when user submits a comment
   */
  @Output() submitComment = new EventEmitter<{ recipeId: number, comment: string, parentCommentId?: number }>();

  /**
   * Current selected tab
   */
  selectedTab: number = 0;

  /**
   * Whether print view is active
   */
  isPrintView: boolean = false;

  /**
   * New rating being submitted
   */
  newRating: number = 0;

  /**
   * New review text
   */
  newReview: string = '';

  /**
   * New comment text
   */
  newComment: string = '';

  /**
   * Reply comment ID (if replying to a comment)
   */
  replyToCommentId: number | null = null;

  /**
   * Reply comment text
   */
  replyText: string = '';

  /**
   * Serving size multiplier
   */
  servingMultiplier: number = 1;

  ngOnInit(): void {
    if (this.userHasRated && this.userRating > 0) {
      this.newRating = this.userRating;
    }
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
    if (confirm('Are you sure you want to delete this recipe?')) {
      this.deleteRecipe.emit(this.recipe.id);
    }
  }

  /**
   * Handle print action
   */
  onPrint(): void {
    this.isPrintView = true;
    setTimeout(() => {
      window.print();
      this.isPrintView = false;
    }, 100);
  }

  /**
   * Submit rating
   */
  onSubmitRating(): void {
    if (this.newRating > 0) {
      this.submitRating.emit({
        recipeId: this.recipe.id,
        rating: this.newRating,
        review: this.newReview
      });
      this.newReview = '';
    }
  }

  /**
   * Submit comment
   */
  onSubmitComment(): void {
    if (this.newComment.trim()) {
      this.submitComment.emit({
        recipeId: this.recipe.id,
        comment: this.newComment
      });
      this.newComment = '';
    }
  }

  /**
   * Submit reply to comment
   */
  onSubmitReply(commentId: number): void {
    if (this.replyText.trim()) {
      this.submitComment.emit({
        recipeId: this.recipe.id,
        comment: this.replyText,
        parentCommentId: commentId
      });
      this.replyText = '';
      this.replyToCommentId = null;
    }
  }

  /**
   * Start replying to a comment
   */
  startReply(commentId: number): void {
    this.replyToCommentId = commentId;
    this.replyText = '';
  }

  /**
   * Cancel reply
   */
  cancelReply(): void {
    this.replyToCommentId = null;
    this.replyText = '';
  }

  /**
   * Get star array for rating
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
      return `${minutes} min`;
    }
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
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
   * Scale ingredient quantity
   */
  scaleQuantity(quantity: string): string {
    if (this.servingMultiplier === 1) {
      return quantity;
    }
    
    // Try to parse and scale numeric quantities
    const numMatch = quantity.match(/^(\d+(\.\d+)?)/);
    if (numMatch) {
      const num = parseFloat(numMatch[1]);
      const scaled = num * this.servingMultiplier;
      return quantity.replace(numMatch[1], scaled.toString());
    }
    
    return quantity;
  }

  /**
   * Increase serving size
   */
  increaseServings(): void {
    this.servingMultiplier += 0.5;
  }

  /**
   * Decrease serving size
   */
  decreaseServings(): void {
    if (this.servingMultiplier > 0.5) {
      this.servingMultiplier -= 0.5;
    }
  }

  /**
   * Reset serving size
   */
  resetServings(): void {
    this.servingMultiplier = 1;
  }

  /**
   * Get scaled servings
   */
  getScaledServings(): number {
    return Math.round(this.recipe.servings * this.servingMultiplier);
  }
}
