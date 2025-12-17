import { Component, Input, OnInit } from '@angular/core';
import { Tradition, ContentCategory, ContentSearchFilters, ContentType } from '../../models/content.model';
import { 
  TraditionParticipant, 
  RelatedTraditionRecipe, 
  RelatedTraditionStory, 
  TraditionOccurrence 
} from '../tradition-details/tradition-details.component';

/**
 * TraditionIndexComponent
 * Container component for tradition listing and detail views
 * Integrates ContentGridComponent for listing and TraditionDetailsComponent for detail view
 */
@Component({
  selector: 'app-tradition-index',
  standalone: false,
  templateUrl: './tradition-index.component.html',
  styleUrls: ['./tradition-index.component.scss']
})
export class TraditionIndexComponent implements OnInit {
  /**
   * List of all traditions
   */
  @Input() traditions: Tradition[] = [];

  /**
   * Tradition categories
   */
  @Input() categories: ContentCategory[] = [];

  /**
   * Featured traditions
   */
  @Input() featuredTraditions: Tradition[] = [];

  /**
   * Recent traditions
   */
  @Input() recentTraditions: Tradition[] = [];

  /**
   * Whether the user can edit traditions
   */
  @Input() canEdit: boolean = false;

  /**
   * Tradition ID from query parameter
   */
  @Input() traditionId: number = 0;

  /**
   * Tradition slug from query parameter
   */
  @Input() slug: string = '';

  /**
   * Category filter from query parameter
   */
  @Input() categoryFilter: string = '';

  /**
   * Frequency filter from query parameter
   */
  @Input() frequencyFilter: string = '';

  /**
   * Current view mode
   */
  viewMode: 'list' | 'detail' = 'list';

  /**
   * Currently selected tradition
   */
  selectedTradition: Tradition | null = null;

  /**
   * Participants for the selected tradition
   */
  traditionParticipants: TraditionParticipant[] = [];

  /**
   * Related recipes for the selected tradition
   */
  relatedRecipes: RelatedTraditionRecipe[] = [];

  /**
   * Related stories for the selected tradition
   */
  relatedStories: RelatedTraditionStory[] = [];

  /**
   * Past occurrences for the selected tradition
   */
  pastOccurrences: TraditionOccurrence[] = [];

  /**
   * Next occurrence for the selected tradition
   */
  nextOccurrence?: TraditionOccurrence;

  /**
   * User interaction flags
   */
  hasFavorited: boolean = false;

  /**
   * Search filters
   */
  searchFilters: ContentSearchFilters = {
    searchText: '',
    contentType: ContentType.Tradition
  };

  /**
   * Breadcrumb items
   */
  breadcrumbItems: Array<{ label: string; url: string }> = [];

  ngOnInit(): void {
    // Determine view mode based on query parameters
    if (this.traditionId || this.slug) {
      this.viewMode = 'detail';
      this.loadTraditionDetail();
    } else {
      this.viewMode = 'list';
      this.buildListBreadcrumbs();
    }

    // Apply category filter if provided
    if (this.categoryFilter) {
      const category = this.categories.find(c => c.slug === this.categoryFilter);
      if (category) {
        this.searchFilters.categoryId = category.id;
      }
    }

    // Apply frequency filter if provided
    if (this.frequencyFilter) {
      this.searchFilters.searchText = this.frequencyFilter;
    }
  }

  /**
   * Load tradition details
   */
  loadTraditionDetail(): void {
    // Find tradition by ID or slug
    if (this.traditionId) {
      this.selectedTradition = this.traditions.find(t => t.id === this.traditionId) || null;
    } else if (this.slug) {
      this.selectedTradition = this.traditions.find(t => t.slug === this.slug) || null;
    }

    if (this.selectedTradition) {
      this.buildDetailBreadcrumbs();
      this.loadParticipants();
      this.loadRelatedRecipes();
      this.loadRelatedStories();
      this.loadOccurrences();
      this.checkUserInteractions();
    }
  }

  /**
   * Load participants for the selected tradition
   */
  loadParticipants(): void {
    if (!this.selectedTradition) return;

    // Map related people to participants with roles
    this.traditionParticipants = this.selectedTradition.relatedPeople.map(person => ({
      ...person,
      participantRole: 'participant' as const
    }));

    // TODO: Fetch actual participants with roles from API
  }

  /**
   * Load related recipes
   */
  loadRelatedRecipes(): void {
    if (!this.selectedTradition || !this.selectedTradition.relatedRecipes) return;

    // TODO: Fetch from API
    // For now, using mock data
    this.relatedRecipes = [];
  }

  /**
   * Load related stories
   */
  loadRelatedStories(): void {
    if (!this.selectedTradition) return;

    // TODO: Fetch from API
    // For now, using mock data
    this.relatedStories = [];
  }

  /**
   * Load occurrences (past and next)
   */
  loadOccurrences(): void {
    if (!this.selectedTradition) return;

    // TODO: Fetch from API
    // For now, using mock data
    this.pastOccurrences = [];
    this.nextOccurrence = undefined;
  }

  /**
   * Check user interactions (favorited)
   */
  checkUserInteractions(): void {
    // TODO: Fetch from API
    this.hasFavorited = false;
  }

  /**
   * Build breadcrumbs for list view
   */
  buildListBreadcrumbs(): void {
    this.breadcrumbItems = [
      { label: 'Home', url: '/' },
      { label: 'Traditions', url: '/Tradition' }
    ];

    // Add category to breadcrumbs if filtered
    if (this.categoryFilter) {
      const category = this.categories.find(c => c.slug === this.categoryFilter);
      if (category) {
        this.breadcrumbItems.push({
          label: category.name,
          url: `/Tradition?category=${category.slug}`
        });
      }
    }
  }

  /**
   * Build breadcrumbs for detail view
   */
  buildDetailBreadcrumbs(): void {
    this.breadcrumbItems = [
      { label: 'Home', url: '/' },
      { label: 'Traditions', url: '/Tradition' }
    ];

    if (this.selectedTradition) {
      // Add category to breadcrumbs
      if (this.selectedTradition.categoryName) {
        const category = this.categories.find(c => c.name === this.selectedTradition?.categoryName);
        if (category) {
          this.breadcrumbItems.push({
            label: category.name,
            url: `/Tradition?category=${category.slug}`
          });
        }
      }

      // Add tradition title to breadcrumbs
      this.breadcrumbItems.push({
        label: this.selectedTradition.title,
        url: `/Tradition?traditionId=${this.selectedTradition.id}`
      });
    }
  }

  /**
   * Handle view tradition event from grid
   */
  onViewTradition(traditionId: number): void {
    window.location.href = `/Tradition?traditionId=${traditionId}`;
  }

  /**
   * Handle edit tradition event
   */
  onEditTradition(traditionId: number): void {
    window.location.href = `/Tradition/Edit/${traditionId}`;
  }

  /**
   * Handle delete tradition event
   */
  onDeleteTradition(traditionId: number): void {
    if (confirm('Are you sure you want to delete this tradition?')) {
      window.location.href = `/Tradition/Delete/${traditionId}`;
    }
  }

  /**
   * Handle view person event
   */
  onViewPerson(personId: number): void {
    window.location.href = `/Person/Details/${personId}`;
  }

  /**
   * Handle view recipe event
   */
  onViewRecipe(recipeId: number): void {
    window.location.href = `/Recipe?recipeId=${recipeId}`;
  }

  /**
   * Handle view story event
   */
  onViewStory(storyId: number): void {
    window.location.href = `/StoryView?storyId=${storyId}`;
  }

  /**
   * Handle create calendar event
   */
  onCreateCalendarEvent(traditionId: number): void {
    // TODO: Implement calendar event creation
    alert('Calendar event creation coming soon!');
  }

  /**
   * Handle favorite tradition
   */
  onFavoriteTradition(traditionId: number): void {
    // TODO: Implement favorite functionality
    this.hasFavorited = !this.hasFavorited;
    alert(this.hasFavorited ? 'Added to favorites!' : 'Removed from favorites!');
  }

  /**
   * Handle share tradition
   */
  onShareTradition(traditionId: number): void {
    const url = `${window.location.origin}/Tradition?traditionId=${traditionId}`;
    navigator.clipboard.writeText(url).then(() => {
      alert('Tradition link copied to clipboard!');
    });
  }

  /**
   * Handle print tradition
   */
  onPrintTradition(traditionId: number): void {
    window.print();
  }

  /**
   * Handle category filter selection
   */
  onCategorySelected(categoryId: number): void {
    const category = this.categories.find(c => c.id === categoryId);
    if (category) {
      window.location.href = `/Tradition?category=${category.slug}`;
    }
  }

  /**
   * Handle search filter change
   */
  onSearchChange(filters: ContentSearchFilters): void {
    this.searchFilters = filters;
  }

  /**
   * Get filtered traditions for display
   */
  getFilteredTraditions(): Tradition[] {
    let filtered = [...this.traditions];

    // Apply search text filter
    if (this.searchFilters.searchText) {
      const search = this.searchFilters.searchText.toLowerCase();
      filtered = filtered.filter(t =>
        t.title.toLowerCase().includes(search) ||
        t.description.toLowerCase().includes(search) ||
        t.frequency.toLowerCase().includes(search)
      );
    }

    // Apply category filter
    if (this.searchFilters.categoryId) {
      filtered = filtered.filter(t => t.categoryId === this.searchFilters.categoryId);
    }

    // Apply tags filter
    if (this.searchFilters.tags && this.searchFilters.tags.length > 0) {
      filtered = filtered.filter(t =>
        this.searchFilters.tags!.some(tag => t.tags.includes(tag))
      );
    }

    // Apply status filter
    if (this.searchFilters.status) {
      filtered = filtered.filter(t => t.status === this.searchFilters.status);
    }

    // Apply featured filter
    if (this.searchFilters.featured !== undefined) {
      filtered = filtered.filter(t => t.featured === this.searchFilters.featured);
    }

    return filtered;
  }
}
