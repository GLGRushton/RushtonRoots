import { Component, OnInit, OnDestroy, HostListener, Input, Output, EventEmitter } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';
import { 
  ParentChildCard, 
  ParentChildSearchFilters,
  ParentChildSortOption,
  PARENT_CHILD_SORT_OPTIONS,
  ParentChildActionEvent,
  RELATIONSHIP_TYPES 
} from '../../models/parent-child.model';

/**
 * ParentChildIndexComponent - Main index page for parent-child relationships
 * Phase 5.1: Parent-Child Relationships
 */
@Component({
  selector: 'app-parent-child-index',
  standalone: false,
  templateUrl: './parent-child-index.component.html',
  styleUrls: ['./parent-child-index.component.scss']
})
export class ParentChildIndexComponent implements OnInit, OnDestroy {
  /**
   * All parent-child relationships from server
   */
  @Input() relationships: ParentChildCard[] = [];

  /**
   * Whether the current user can create relationships
   */
  @Input() canCreate: boolean = true;

  /**
   * Whether the current user can edit relationships
   */
  @Input() canEdit: boolean = true;

  /**
   * Whether the current user can delete relationships
   */
  @Input() canDelete: boolean = true;

  /**
   * All parent-child relationships
   */
  allRelationships: ParentChildCard[] = [];

  /**
   * Filtered relationships
   */
  filteredRelationships: ParentChildCard[] = [];

  /**
   * Event emitted when a relationship action is triggered
   */
  @Output() action = new EventEmitter<ParentChildActionEvent>();

  /**
   * Event emitted when create button is clicked
   */
  @Output() create = new EventEmitter<void>();

  /**
   * Loading state
   */
  isLoading = false;

  /**
   * Search control
   */
  searchControl = new FormControl('');

  /**
   * Selected sort option
   */
  sortOption: string = 'childName-asc';

  /**
   * Available sort options
   */
  sortOptions = PARENT_CHILD_SORT_OPTIONS;

  /**
   * Available relationship types
   */
  relationshipTypes = RELATIONSHIP_TYPES;

  /**
   * Selected relationship type filter
   */
  selectedType: string = '';

  /**
   * Verified only filter
   */
  verifiedOnly = false;

  /**
   * Grid columns
   */
  gridColumns = 3;

  /**
   * Subject for component destruction
   */
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.calculateGridColumns();
    this.loadRelationships();
    this.setupSearch();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Setup search with debouncing
   */
  private setupSearch(): void {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.applyFilters();
      });
  }

  /**
   * Load parent-child relationships from input or use sample data
   */
  private loadRelationships(): void {
    this.isLoading = true;

    // Use input data if available, otherwise use sample data
    if (this.relationships && this.relationships.length > 0) {
      // Transform input data to ensure dates are proper Date objects
      this.allRelationships = this.relationships.map(r => ({
        ...r,
        childBirthDate: r.childBirthDate ? new Date(r.childBirthDate) : undefined,
        createdDateTime: new Date(r.createdDateTime),
        updatedDateTime: new Date(r.updatedDateTime)
      }));
    } else {
      // Use sample data for demonstration/testing
      this.allRelationships = this.getSampleData();
    }
    
    this.filteredRelationships = [...this.allRelationships];
    this.applyFilters();
    this.isLoading = false;
  }

  /**
   * Get sample data for demonstration
   */
  private getSampleData(): ParentChildCard[] {
    return [
      {
        id: 1,
        parentPersonId: 101,
        childPersonId: 201,
        parentName: 'John Smith',
        childName: 'Emily Smith',
        parentPhotoUrl: undefined,
        childPhotoUrl: undefined,
        relationshipType: 'biological',
        relationshipTypeDisplay: 'Biological',
        relationshipTypeIcon: 'bloodtype',
        relationshipTypeColor: 'primary',
        childBirthDate: new Date('2005-06-15'),
        childAge: 18,
        isVerified: true,
        createdDateTime: new Date('2024-01-15'),
        updatedDateTime: new Date('2024-01-15')
      },
      {
        id: 2,
        parentPersonId: 102,
        childPersonId: 202,
        parentName: 'Sarah Johnson',
        childName: 'Michael Johnson',
        parentPhotoUrl: undefined,
        childPhotoUrl: undefined,
        relationshipType: 'adopted',
        relationshipTypeDisplay: 'Adopted',
        relationshipTypeIcon: 'volunteer_activism',
        relationshipTypeColor: 'accent',
        childBirthDate: new Date('2010-03-20'),
        childAge: 13,
        isVerified: true,
        createdDateTime: new Date('2024-02-10'),
        updatedDateTime: new Date('2024-02-10')
      },
      {
        id: 3,
        parentPersonId: 103,
        childPersonId: 203,
        parentName: 'David Brown',
        childName: 'Sophie Brown',
        parentPhotoUrl: undefined,
        childPhotoUrl: undefined,
        relationshipType: 'biological',
        relationshipTypeDisplay: 'Biological',
        relationshipTypeIcon: 'bloodtype',
        relationshipTypeColor: 'primary',
        childBirthDate: new Date('2015-09-10'),
        childAge: 8,
        isVerified: false,
        confidence: 85,
        createdDateTime: new Date('2024-03-05'),
        updatedDateTime: new Date('2024-03-05')
      }
    ];
  }

  /**
   * Apply filters to relationships
   */
  applyFilters(): void {
    const searchText = this.searchControl.value?.toLowerCase() || '';

    this.filteredRelationships = this.allRelationships.filter(relationship => {
      // Search filter
      const matchesSearch = !searchText || 
        relationship.parentName.toLowerCase().includes(searchText) ||
        relationship.childName.toLowerCase().includes(searchText);

      // Type filter
      const matchesType = !this.selectedType || 
        relationship.relationshipType === this.selectedType;

      // Verified filter
      const matchesVerified = !this.verifiedOnly || 
        relationship.isVerified;

      return matchesSearch && matchesType && matchesVerified;
    });

    this.applySorting();
  }

  /**
   * Apply sorting to filtered relationships
   */
  private applySorting(): void {
    const [field, direction] = this.sortOption.split('-');

    this.filteredRelationships.sort((a, b) => {
      let comparison = 0;

      switch (field) {
        case 'childName':
          comparison = a.childName.localeCompare(b.childName);
          break;
        case 'parentName':
          comparison = a.parentName.localeCompare(b.parentName);
          break;
        case 'childBirthDate':
          const dateA = a.childBirthDate?.getTime() || 0;
          const dateB = b.childBirthDate?.getTime() || 0;
          comparison = dateA - dateB;
          break;
        case 'created':
          comparison = a.createdDateTime.getTime() - b.createdDateTime.getTime();
          break;
        case 'updated':
          comparison = a.updatedDateTime.getTime() - b.updatedDateTime.getTime();
          break;
      }

      return direction === 'desc' ? -comparison : comparison;
    });
  }

  /**
   * Handle sort change
   */
  onSortChange(): void {
    this.applyFilters();
  }

  /**
   * Handle type filter change
   */
  onTypeFilterChange(): void {
    this.applyFilters();
  }

  /**
   * Handle verified filter change
   */
  onVerifiedFilterChange(): void {
    this.applyFilters();
  }

  /**
   * Handle relationship action
   */
  onRelationshipAction(event: ParentChildActionEvent): void {
    this.action.emit(event);
  }

  /**
   * Navigate to create relationship
   */
  onCreate(): void {
    this.create.emit();
  }

  /**
   * Calculate grid columns based on window width
   */
  @HostListener('window:resize')
  calculateGridColumns(): void {
    const width = window.innerWidth;
    if (width < 768) {
      this.gridColumns = 1;
    } else if (width < 1200) {
      this.gridColumns = 2;
    } else if (width < 1600) {
      this.gridColumns = 3;
    } else {
      this.gridColumns = 4;
    }
  }

  /**
   * Get grid template columns CSS
   */
  getGridColumns(): string {
    return `repeat(${this.gridColumns}, 1fr)`;
  }
}
