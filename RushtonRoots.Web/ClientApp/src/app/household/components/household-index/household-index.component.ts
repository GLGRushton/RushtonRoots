import { Component, Input, OnInit } from '@angular/core';
import { 
  HouseholdCard, 
  HouseholdAction, 
  HouseholdSearchFilters, 
  HouseholdSortOption,
  HOUSEHOLD_SORT_OPTIONS 
} from '../../models/household.model';

/**
 * HouseholdIndexComponent - Main component for the Household Index page
 * 
 * Features:
 * - Grid layout for household cards
 * - Search and filter functionality
 * - Sorting options
 * - Responsive grid (adjusts columns based on screen size)
 * - Quick actions for each household
 * - Empty state when no households
 * - Loading state
 */
@Component({
  selector: 'app-household-index',
  standalone: false,
  templateUrl: './household-index.component.html',
  styleUrls: ['./household-index.component.scss']
})
export class HouseholdIndexComponent implements OnInit {
  @Input() initialHouseholds: HouseholdCard[] = [];
  @Input() canEdit = false;
  @Input() canDelete = false;
  @Input() canCreate = false;
  
  households: HouseholdCard[] = [];
  filteredHouseholds: HouseholdCard[] = [];
  isLoading = false;
  
  // Search and filters
  searchFilters: HouseholdSearchFilters = {};
  searchTerm = '';
  
  // Sorting
  sortOptions = HOUSEHOLD_SORT_OPTIONS;
  selectedSort: HouseholdSortOption = this.sortOptions[0]; // Default: Name A-Z
  
  // View options
  gridColumns = 3; // Default number of columns

  ngOnInit(): void {
    this.households = this.initialHouseholds;
    this.filteredHouseholds = this.initialHouseholds;
    this.applySorting();
    this.updateGridColumns();
    
    // Listen for window resize to adjust grid columns
    window.addEventListener('resize', () => this.updateGridColumns());
  }

  /**
   * Handle search input change
   */
  onSearchChange(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.searchFilters.searchTerm = searchTerm;
    this.applyFilters();
  }

  /**
   * Apply all filters to the household list
   */
  applyFilters(): void {
    this.filteredHouseholds = this.households.filter(household => {
      // Search term filter
      if (this.searchFilters.searchTerm) {
        const searchLower = this.searchFilters.searchTerm.toLowerCase();
        const nameMatch = household.householdName.toLowerCase().includes(searchLower);
        const anchorMatch = household.anchorPersonName?.toLowerCase().includes(searchLower) || false;
        
        if (!nameMatch && !anchorMatch) {
          return false;
        }
      }
      
      // Member count filters
      if (this.searchFilters.minMemberCount !== undefined && 
          household.memberCount < this.searchFilters.minMemberCount) {
        return false;
      }
      
      if (this.searchFilters.maxMemberCount !== undefined && 
          household.memberCount > this.searchFilters.maxMemberCount) {
        return false;
      }
      
      // Has anchor filter
      if (this.searchFilters.hasAnchor !== undefined) {
        const hasAnchor = household.anchorPersonId !== null && household.anchorPersonId !== undefined;
        if (this.searchFilters.hasAnchor !== hasAnchor) {
          return false;
        }
      }
      
      // Date filters
      if (this.searchFilters.createdAfter) {
        const createdDate = new Date(household.createdDateTime);
        if (createdDate < this.searchFilters.createdAfter) {
          return false;
        }
      }
      
      if (this.searchFilters.createdBefore) {
        const createdDate = new Date(household.createdDateTime);
        if (createdDate > this.searchFilters.createdBefore) {
          return false;
        }
      }
      
      return true;
    });
    
    this.applySorting();
  }

  /**
   * Handle sort selection change
   */
  onSortChange(sortOption: HouseholdSortOption): void {
    this.selectedSort = sortOption;
    this.applySorting();
  }

  /**
   * Apply sorting to filtered households
   */
  applySorting(): void {
    this.filteredHouseholds = [...this.filteredHouseholds].sort((a, b) => {
      let comparison = 0;
      
      switch (this.selectedSort.field) {
        case 'name':
          comparison = a.householdName.localeCompare(b.householdName);
          break;
        case 'memberCount':
          comparison = a.memberCount - b.memberCount;
          break;
        case 'createdDate':
          comparison = new Date(a.createdDateTime).getTime() - new Date(b.createdDateTime).getTime();
          break;
        case 'updatedDate':
          comparison = new Date(a.updatedDateTime).getTime() - new Date(b.updatedDateTime).getTime();
          break;
      }
      
      return this.selectedSort.direction === 'asc' ? comparison : -comparison;
    });
  }

  /**
   * Handle household card actions
   */
  onHouseholdAction(action: HouseholdAction): void {
    console.log('Household action:', action);
    
    // In a real app, these would call API endpoints or navigate to routes
    switch (action.type) {
      case 'view':
        this.viewHousehold(action.householdId);
        break;
      case 'edit':
        this.editHousehold(action.householdId);
        break;
      case 'delete':
        this.deleteHousehold(action.householdId);
        break;
      case 'manage-members':
        this.manageMembers(action.householdId);
        break;
      case 'settings':
        this.householdSettings(action.householdId);
        break;
    }
  }

  /**
   * Navigate to household details
   */
  private viewHousehold(householdId: number): void {
    // In a real app: this.router.navigate(['/household', householdId]);
    window.location.href = `/Household/Details/${householdId}`;
  }

  /**
   * Navigate to edit household
   */
  private editHousehold(householdId: number): void {
    // In a real app: this.router.navigate(['/household', householdId, 'edit']);
    window.location.href = `/Household/Edit/${householdId}`;
  }

  /**
   * Delete household with confirmation
   */
  private deleteHousehold(householdId: number): void {
    const household = this.households.find(h => h.id === householdId);
    if (!household) return;
    
    if (confirm(`Are you sure you want to delete the household "${household.householdName}"?`)) {
      // In a real app, call API to delete
      console.log('Deleting household:', householdId);
      window.location.href = `/Household/Delete/${householdId}`;
    }
  }

  /**
   * Navigate to manage household members
   */
  private manageMembers(householdId: number): void {
    // In a real app: this.router.navigate(['/household', householdId, 'members']);
    window.location.href = `/Household/Members/${householdId}`;
  }

  /**
   * Navigate to household settings
   */
  private householdSettings(householdId: number): void {
    console.log('Household settings:', householdId);
    // Future implementation
  }

  /**
   * Create new household
   */
  createHousehold(): void {
    window.location.href = '/Household/Create';
  }

  /**
   * Clear all filters
   */
  clearFilters(): void {
    this.searchTerm = '';
    this.searchFilters = {};
    this.applyFilters();
  }

  /**
   * Update grid columns based on screen size
   */
  private updateGridColumns(): void {
    const width = window.innerWidth;
    
    if (width < 600) {
      this.gridColumns = 1; // Mobile
    } else if (width < 960) {
      this.gridColumns = 2; // Tablet
    } else if (width < 1280) {
      this.gridColumns = 3; // Small desktop
    } else {
      this.gridColumns = 4; // Large desktop
    }
  }

  /**
   * Get grid template columns CSS
   */
  getGridStyle(): string {
    return `repeat(${this.gridColumns}, 1fr)`;
  }
}
