import { Component, Input, Output, EventEmitter, OnInit, HostListener } from '@angular/core';
import { Recipe, Story, Tradition, ContentType, ContentSearchFilters, ContentSortOption, CONTENT_SORT_OPTIONS } from '../../models/content.model';

/**
 * ContentGridComponent
 * Displays content (recipes, stories, traditions) in a masonry grid layout
 * with filtering and sorting capabilities
 */
@Component({
  selector: 'app-content-grid',
  standalone: false,
  templateUrl: './content-grid.component.html',
  styleUrls: ['./content-grid.component.scss']
})
export class ContentGridComponent implements OnInit {
  /**
   * Type of content to display
   */
  @Input() contentType!: ContentType;

  /**
   * Items to display (can be recipes, stories, or traditions)
   */
  @Input() items: (Recipe | Story | Tradition)[] = [];

  /**
   * Whether to show filters
   */
  @Input() showFilters: boolean = true;

  /**
   * Whether to show sort options
   */
  @Input() showSort: boolean = true;

  /**
   * Whether user can edit items
   */
  @Input() canEdit: boolean = false;

  /**
   * Current search filters
   */
  @Input() filters: ContentSearchFilters = {
    searchText: '',
    contentType: undefined,
    categoryId: undefined,
    tags: [],
    authorId: undefined,
    status: undefined,
    featured: undefined
  };

  /**
   * Event emitted when filters change
   */
  @Output() filtersChange = new EventEmitter<ContentSearchFilters>();

  /**
   * Event emitted when view item is clicked
   */
  @Output() viewItem = new EventEmitter<{ type: ContentType, id: number }>();

  /**
   * Event emitted when edit item is clicked
   */
  @Output() editItem = new EventEmitter<{ type: ContentType, id: number }>();

  /**
   * Event emitted when delete item is clicked
   */
  @Output() deleteItem = new EventEmitter<{ type: ContentType, id: number }>();

  /**
   * Available sort options
   */
  sortOptions: ContentSortOption[] = CONTENT_SORT_OPTIONS;

  /**
   * Current sort option
   */
  currentSort: string = 'publishedDate-desc';

  /**
   * Filtered and sorted items
   */
  filteredItems: (Recipe | Story | Tradition)[] = [];

  /**
   * Number of columns in the grid
   */
  columns: number = 3;

  /**
   * Whether advanced filters are expanded
   */
  showAdvancedFilters: boolean = false;

  /**
   * Content type enum for template
   */
  ContentType = ContentType;

  ngOnInit(): void {
    this.calculateColumns();
    this.applyFiltersAndSort();
  }

  /**
   * Calculate number of columns based on screen width
   */
  @HostListener('window:resize', ['$event'])
  onResize(): void {
    this.calculateColumns();
  }

  /**
   * Calculate columns
   */
  calculateColumns(): void {
    const width = window.innerWidth;
    if (width < 600) {
      this.columns = 1;
    } else if (width < 960) {
      this.columns = 2;
    } else if (width < 1280) {
      this.columns = 3;
    } else {
      this.columns = 4;
    }
  }

  /**
   * Apply filters and sorting
   */
  applyFiltersAndSort(): void {
    let filtered = [...this.items];

    // Apply search filter
    if (this.filters.searchText) {
      const search = this.filters.searchText.toLowerCase();
      filtered = filtered.filter(item => 
        item.title.toLowerCase().includes(search) ||
        (item as any).description?.toLowerCase().includes(search) ||
        (item as any).summary?.toLowerCase().includes(search) ||
        item.tags.some(tag => tag.toLowerCase().includes(search))
      );
    }

    // Apply category filter
    if (this.filters.categoryId) {
      filtered = filtered.filter(item => item.categoryId === this.filters.categoryId);
    }

    // Apply tag filter
    if (this.filters.tags && this.filters.tags.length > 0) {
      filtered = filtered.filter(item => 
        this.filters.tags!.some(tag => item.tags.includes(tag))
      );
    }

    // Apply featured filter
    if (this.filters.featured !== undefined) {
      filtered = filtered.filter(item => item.featured === this.filters.featured);
    }

    // Apply sorting
    filtered = this.sortItems(filtered);

    this.filteredItems = filtered;
  }

  /**
   * Sort items
   */
  sortItems(items: (Recipe | Story | Tradition)[]): (Recipe | Story | Tradition)[] {
    const [field, direction] = this.currentSort.split('-');

    return items.sort((a, b) => {
      let aValue: any;
      let bValue: any;

      switch (field) {
        case 'publishedDate':
          aValue = a.publishedDate ? new Date(a.publishedDate).getTime() : 0;
          bValue = b.publishedDate ? new Date(b.publishedDate).getTime() : 0;
          break;
        case 'title':
          aValue = a.title.toLowerCase();
          bValue = b.title.toLowerCase();
          break;
        case 'viewCount':
          aValue = a.viewCount;
          bValue = b.viewCount;
          break;
        case 'rating':
          aValue = (a as Recipe).averageRating || 0;
          bValue = (b as Recipe).averageRating || 0;
          break;
        case 'featured':
          aValue = a.featured ? 1 : 0;
          bValue = b.featured ? 1 : 0;
          break;
        default:
          return 0;
      }

      if (direction === 'desc') {
        return bValue > aValue ? 1 : bValue < aValue ? -1 : 0;
      } else {
        return aValue > bValue ? 1 : aValue < bValue ? -1 : 0;
      }
    });
  }

  /**
   * Handle search text change
   */
  onSearchChange(searchText: string): void {
    this.filters.searchText = searchText;
    this.applyFiltersAndSort();
    this.filtersChange.emit(this.filters);
  }

  /**
   * Handle sort change
   */
  onSortChange(sort: string): void {
    this.currentSort = sort;
    this.applyFiltersAndSort();
  }

  /**
   * Toggle advanced filters
   */
  toggleAdvancedFilters(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
  }

  /**
   * Clear all filters
   */
  clearFilters(): void {
    this.filters = {
      searchText: '',
      contentType: this.contentType,
      categoryId: undefined,
      tags: [],
      authorId: undefined,
      status: undefined,
      featured: undefined
    };
    this.applyFiltersAndSort();
    this.filtersChange.emit(this.filters);
  }

  /**
   * Check if item is Recipe
   */
  isRecipe(item: any): item is Recipe {
    return this.contentType === ContentType.Recipe;
  }

  /**
   * Check if item is Story
   */
  isStory(item: any): item is Story {
    return this.contentType === ContentType.Story;
  }

  /**
   * Check if item is Tradition
   */
  isTradition(item: any): item is Tradition {
    return this.contentType === ContentType.Tradition;
  }

  /**
   * Handle view event
   */
  onViewItem(id: number): void {
    this.viewItem.emit({ type: this.contentType, id });
  }

  /**
   * Handle edit event
   */
  onEditItem(id: number): void {
    this.editItem.emit({ type: this.contentType, id });
  }

  /**
   * Handle delete event
   */
  onDeleteItem(id: number): void {
    this.deleteItem.emit({ type: this.contentType, id });
  }

  /**
   * Get content type label
   */
  getContentTypeLabel(): string {
    switch (this.contentType) {
      case ContentType.Recipe:
        return 'Recipes';
      case ContentType.Story:
        return 'Stories';
      case ContentType.Tradition:
        return 'Traditions';
      default:
        return 'Content';
    }
  }
}
