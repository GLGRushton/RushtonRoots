import { Component, Input, OnInit } from '@angular/core';
import { Recipe, ContentSearchFilters, ContentType } from '../../models/content.model';
import { BreadcrumbItem } from '../../../shared/components/breadcrumb/breadcrumb.component';

/**
 * RecipeIndexComponent
 * Main component for recipe listing with categories, favorites, and recent recipes
 * Supports routing to recipe details view
 */
@Component({
  selector: 'app-recipe-index',
  standalone: false,
  templateUrl: './recipe-index.component.html',
  styleUrls: ['./recipe-index.component.scss']
})
export class RecipeIndexComponent implements OnInit {
  /**
   * All recipes to display
   */
  @Input() recipes: Recipe[] = [];

  /**
   * Categories available
   */
  @Input() categories: string[] = [];

  /**
   * Favorite recipes
   */
  @Input() favorites: Recipe[] = [];

  /**
   * Recent recipes
   */
  @Input() recent: Recipe[] = [];

  /**
   * Whether user can edit recipes
   */
  @Input() canEdit: boolean = false;

  /**
   * Current view mode (list or detail)
   */
  @Input() viewMode: 'list' | 'detail' = 'list';

  /**
   * Recipe ID to display in detail mode
   */
  @Input() recipeId?: number;

  /**
   * Recipe slug to display in detail mode
   */
  @Input() recipeSlug?: string;

  /**
   * Breadcrumb items for navigation
   */
  breadcrumbs: BreadcrumbItem[] = [];

  /**
   * Current recipe for detail view
   */
  currentRecipe?: Recipe;

  /**
   * Search filters
   */
  filters: ContentSearchFilters = {
    searchText: '',
    contentType: ContentType.Recipe,
    categoryId: undefined,
    tags: [],
    authorId: undefined,
    status: undefined,
    featured: undefined
  };

  /**
   * Content type for content-grid
   */
  contentType = ContentType.Recipe;

  /**
   * Selected category for filtering
   */
  selectedCategory?: string;

  /**
   * Whether to show ingredient search
   */
  showIngredientSearch: boolean = false;

  /**
   * Ingredient search text
   */
  ingredientSearchText: string = '';

  /**
   * Filtered recipes based on ingredient search
   */
  filteredRecipes: Recipe[] = [];

  ngOnInit(): void {
    this.updateBreadcrumbs();
    this.filteredRecipes = this.recipes;

    // If in detail mode, find and set the current recipe
    if (this.viewMode === 'detail') {
      if (this.recipeId) {
        this.currentRecipe = this.recipes.find(r => r.id === this.recipeId);
      } else if (this.recipeSlug) {
        this.currentRecipe = this.recipes.find(r => this.slugify(r.title) === this.recipeSlug);
      }
      
      if (this.currentRecipe) {
        this.updateBreadcrumbs();
      }
    }
  }

  /**
   * Update breadcrumb navigation
   */
  updateBreadcrumbs(): void {
    this.breadcrumbs = [
      { label: 'Home', url: '/', icon: 'home' },
      { label: 'Recipes', url: '/Recipe' }
    ];

    if (this.viewMode === 'detail' && this.currentRecipe) {
      if (this.currentRecipe.categoryName) {
        this.breadcrumbs.push({
          label: this.currentRecipe.categoryName,
          url: `/Recipe?category=${encodeURIComponent(this.currentRecipe.categoryName)}`
        });
      }
      this.breadcrumbs.push({
        label: this.currentRecipe.title
      });
    }
  }

  /**
   * Handle category selection
   */
  onCategorySelected(category: string): void {
    this.selectedCategory = category;
    this.filters = {
      ...this.filters,
      searchText: category
    };
    this.filterByCategory(category);
  }

  /**
   * Filter recipes by category
   */
  filterByCategory(category: string): void {
    if (!category) {
      this.filteredRecipes = this.recipes;
      return;
    }
    this.filteredRecipes = this.recipes.filter(r => 
      r.categoryName?.toLowerCase() === category.toLowerCase()
    );
  }

  /**
   * Handle view recipe
   */
  onViewRecipe(recipeId: number): void {
    window.location.href = `/Recipe?recipeId=${recipeId}`;
  }

  /**
   * Handle edit recipe
   */
  onEditRecipe(recipeId: number): void {
    window.location.href = `/Recipe/Edit/${recipeId}`;
  }

  /**
   * Handle delete recipe
   */
  onDeleteRecipe(recipeId: number): void {
    if (confirm('Are you sure you want to delete this recipe?')) {
      window.location.href = `/Recipe/Delete/${recipeId}`;
    }
  }

  /**
   * Handle filters change from content-grid
   */
  onFiltersChange(filters: ContentSearchFilters): void {
    this.filters = filters;
    this.applyFilters();
  }

  /**
   * Apply current filters
   */
  applyFilters(): void {
    let filtered = this.recipes;

    // Text search
    if (this.filters.searchText) {
      const searchLower = this.filters.searchText.toLowerCase();
      filtered = filtered.filter(r =>
        r.title.toLowerCase().includes(searchLower) ||
        r.description?.toLowerCase().includes(searchLower) ||
        r.categoryName?.toLowerCase().includes(searchLower)
      );
    }

    // Ingredient search
    if (this.ingredientSearchText.trim()) {
      filtered = this.searchByIngredient(filtered, this.ingredientSearchText);
    }

    this.filteredRecipes = filtered;
  }

  /**
   * Search recipes by ingredient
   */
  searchByIngredient(recipes: Recipe[], searchText: string): Recipe[] {
    const searchLower = searchText.toLowerCase().trim();
    return recipes.filter(recipe =>
      recipe.ingredients.some(ingredient =>
        ingredient.ingredient.toLowerCase().includes(searchLower)
      )
    );
  }

  /**
   * Toggle ingredient search panel
   */
  toggleIngredientSearch(): void {
    this.showIngredientSearch = !this.showIngredientSearch;
  }

  /**
   * Handle ingredient search
   */
  onIngredientSearch(): void {
    this.applyFilters();
  }

  /**
   * Clear ingredient search
   */
  clearIngredientSearch(): void {
    this.ingredientSearchText = '';
    this.applyFilters();
  }

  /**
   * Handle item view from content-grid
   */
  onViewItem(event: { type: ContentType, id: number }): void {
    if (event.type === ContentType.Recipe) {
      this.onViewRecipe(event.id);
    }
  }

  /**
   * Handle item edit from content-grid
   */
  onEditItem(event: { type: ContentType, id: number }): void {
    if (event.type === ContentType.Recipe) {
      this.onEditRecipe(event.id);
    }
  }

  /**
   * Handle item delete from content-grid
   */
  onDeleteItem(event: { type: ContentType, id: number }): void {
    if (event.type === ContentType.Recipe) {
      this.onDeleteRecipe(event.id);
    }
  }

  /**
   * Handle view recipe card
   */
  onRecipeCardView(recipeId: number): void {
    this.onViewRecipe(recipeId);
  }

  /**
   * Handle edit recipe card
   */
  onRecipeCardEdit(recipeId: number): void {
    this.onEditRecipe(recipeId);
  }

  /**
   * Handle delete recipe card
   */
  onRecipeCardDelete(recipeId: number): void {
    this.onDeleteRecipe(recipeId);
  }

  /**
   * Handle submit rating from detail view
   */
  onSubmitRating(event: { recipeId: number, rating: number, review: string }): void {
    // TODO: Implement rating submission to backend
    console.log('Submit rating:', event);
    alert('Rating submission will be implemented with backend integration');
  }

  /**
   * Handle submit comment from detail view
   */
  onSubmitComment(event: { recipeId: number, comment: string, parentCommentId?: number }): void {
    // TODO: Implement comment submission to backend
    console.log('Submit comment:', event);
    alert('Comment submission will be implemented with backend integration');
  }

  /**
   * Handle edit from detail view
   */
  onDetailEdit(recipeId: number): void {
    this.onEditRecipe(recipeId);
  }

  /**
   * Handle delete from detail view
   */
  onDetailDelete(recipeId: number): void {
    this.onDeleteRecipe(recipeId);
  }

  /**
   * Convert title to URL-friendly slug
   */
  private slugify(text: string): string {
    return text
      .toLowerCase()
      .replace(/[^\w\s-]/g, '')
      .replace(/\s+/g, '-')
      .replace(/-+/g, '-')
      .trim();
  }
}
