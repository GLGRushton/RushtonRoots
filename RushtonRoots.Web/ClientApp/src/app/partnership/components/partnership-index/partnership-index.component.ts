import { Component, OnInit, Input, HostListener } from '@angular/core';
import { 
  PartnershipCard, 
  PartnershipSearchFilters, 
  PartnershipSortOption,
  PartnershipActionEvent,
  PartnershipStatus,
  PARTNERSHIP_SORT_OPTIONS,
  PARTNERSHIP_TYPES
} from '../../models/partnership.model';

/**
 * PartnershipIndexComponent - Main index page for partnerships
 * Phase 5.1: Partnership Management
 */
@Component({
  selector: 'app-partnership-index',
  templateUrl: './partnership-index.component.html',
  styleUrls: ['./partnership-index.component.scss']
})
export class PartnershipIndexComponent implements OnInit {
  /**
   * All partnerships data
   */
  @Input() partnerships: PartnershipCard[] = [];

  /**
   * Whether the current user can create partnerships
   */
  @Input() canCreate: boolean = true;

  /**
   * Whether the current user can edit partnerships
   */
  @Input() canEdit: boolean = true;

  /**
   * Whether the current user can delete partnerships
   */
  @Input() canDelete: boolean = true;

  /**
   * Filtered partnerships to display
   */
  filteredPartnerships: PartnershipCard[] = [];

  /**
   * Current search filters
   */
  filters: PartnershipSearchFilters = {};

  /**
   * Current sort option
   */
  currentSort: PartnershipSortOption = PARTNERSHIP_SORT_OPTIONS[0];

  /**
   * Available sort options
   */
  sortOptions = PARTNERSHIP_SORT_OPTIONS;

  /**
   * Available partnership types
   */
  partnershipTypes = PARTNERSHIP_TYPES;

  /**
   * Loading state
   */
  loading: boolean = false;

  /**
   * Grid columns based on screen size
   */
  gridColumns: number = 3;

  /**
   * Search text input
   */
  searchText: string = '';

  /**
   * Debounce timer for search
   */
  private searchDebounceTimer?: any;

  ngOnInit(): void {
    this.updateGridColumns();
    this.applyFiltersAndSort();
  }

  /**
   * Update grid columns based on window width
   */
  @HostListener('window:resize', ['$event'])
  onResize(): void {
    this.updateGridColumns();
  }

  /**
   * Update grid columns
   */
  private updateGridColumns(): void {
    const width = window.innerWidth;
    if (width < 600) {
      this.gridColumns = 1;
    } else if (width < 960) {
      this.gridColumns = 2;
    } else if (width < 1280) {
      this.gridColumns = 3;
    } else {
      this.gridColumns = 4;
    }
  }

  /**
   * Handle search text change
   */
  onSearchChange(searchText: string): void {
    this.searchText = searchText;
    
    // Debounce search
    if (this.searchDebounceTimer) {
      clearTimeout(this.searchDebounceTimer);
    }
    
    this.searchDebounceTimer = setTimeout(() => {
      this.filters.searchText = searchText.toLowerCase();
      this.applyFiltersAndSort();
    }, 300);
  }

  /**
   * Handle filter change
   */
  onFilterChange(): void {
    this.applyFiltersAndSort();
  }

  /**
   * Handle sort change
   */
  onSortChange(sortOption: PartnershipSortOption): void {
    this.currentSort = sortOption;
    this.applyFiltersAndSort();
  }

  /**
   * Clear all filters
   */
  clearFilters(): void {
    this.filters = {};
    this.searchText = '';
    this.applyFiltersAndSort();
  }

  /**
   * Apply filters and sorting
   */
  private applyFiltersAndSort(): void {
    let result = [...this.partnerships];

    // Apply search filter
    if (this.filters.searchText) {
      const searchLower = this.filters.searchText.toLowerCase();
      result = result.filter(p =>
        p.personAName.toLowerCase().includes(searchLower) ||
        p.personBName.toLowerCase().includes(searchLower) ||
        p.partnershipTypeDisplay.toLowerCase().includes(searchLower) ||
        p.location?.toLowerCase().includes(searchLower)
      );
    }

    // Apply partnership type filter
    if (this.filters.partnershipType) {
      result = result.filter(p => p.partnershipType === this.filters.partnershipType);
    }

    // Apply status filter
    if (this.filters.status) {
      result = result.filter(p => p.status === this.filters.status);
    }

    // Apply person filter
    if (this.filters.personId) {
      result = result.filter(p => 
        p.personAId === this.filters.personId || 
        p.personBId === this.filters.personId
      );
    }

    // Apply sorting
    result.sort((a, b) => {
      const direction = this.currentSort.direction === 'asc' ? 1 : -1;
      
      switch (this.currentSort.field) {
        case 'startDate':
          const dateA = a.startDate ? new Date(a.startDate).getTime() : 0;
          const dateB = b.startDate ? new Date(b.startDate).getTime() : 0;
          return (dateA - dateB) * direction;
        
        case 'endDate':
          const endA = a.endDate ? new Date(a.endDate).getTime() : 0;
          const endB = b.endDate ? new Date(b.endDate).getTime() : 0;
          return (endA - endB) * direction;
        
        case 'personAName':
          return a.personAName.localeCompare(b.personAName) * direction;
        
        case 'personBName':
          return a.personBName.localeCompare(b.personBName) * direction;
        
        case 'partnershipType':
          return a.partnershipType.localeCompare(b.partnershipType) * direction;
        
        case 'createdDate':
          return (new Date(a.createdDateTime).getTime() - new Date(b.createdDateTime).getTime()) * direction;
        
        default:
          return 0;
      }
    });

    this.filteredPartnerships = result;
  }

  /**
   * Handle card action
   */
  onCardAction(event: PartnershipActionEvent): void {
    switch (event.action) {
      case 'view':
        this.viewPartnership(event.partnership);
        break;
      case 'edit':
        this.editPartnership(event.partnership);
        break;
      case 'delete':
        this.deletePartnership(event.partnership);
        break;
      case 'timeline':
        this.viewTimeline(event.partnership);
        break;
    }
  }

  /**
   * View partnership details
   */
  viewPartnership(partnership: PartnershipCard): void {
    // Navigate to details page or open dialog
    console.log('View partnership:', partnership);
    window.location.href = `/Partnership/Details/${partnership.id}`;
  }

  /**
   * Edit partnership
   */
  editPartnership(partnership: PartnershipCard): void {
    // Navigate to edit page or open dialog
    console.log('Edit partnership:', partnership);
    window.location.href = `/Partnership/Edit/${partnership.id}`;
  }

  /**
   * Delete partnership
   */
  deletePartnership(partnership: PartnershipCard): void {
    if (confirm(`Are you sure you want to delete the partnership between ${partnership.personAName} and ${partnership.personBName}?`)) {
      // Call API to delete
      console.log('Delete partnership:', partnership);
      window.location.href = `/Partnership/Delete/${partnership.id}`;
    }
  }

  /**
   * View partnership timeline
   */
  viewTimeline(partnership: PartnershipCard): void {
    // Open timeline view
    console.log('View timeline:', partnership);
    // This could open a dialog or navigate to a dedicated timeline page
  }

  /**
   * Create new partnership
   */
  createPartnership(): void {
    window.location.href = '/Partnership/Create';
  }

  /**
   * Get active filter count
   */
  getActiveFilterCount(): number {
    let count = 0;
    if (this.filters.searchText) count++;
    if (this.filters.partnershipType) count++;
    if (this.filters.status) count++;
    if (this.filters.personId) count++;
    return count;
  }
}
