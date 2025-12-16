/**
 * TypeScript interfaces for Household components
 * These models correspond to the C# domain models in RushtonRoots.Domain
 */

/**
 * Represents a household card data structure
 */
export interface HouseholdCard {
  id: number;
  householdName: string;
  anchorPersonId?: number;
  anchorPersonName?: string;
  memberCount: number;
  createdDateTime: Date;
  updatedDateTime: Date;
  members?: HouseholdMember[];
}

/**
 * Represents a household member
 */
export interface HouseholdMember {
  personId: number;
  firstName: string;
  lastName: string;
  fullName: string;
  photoUrl?: string;
  relationship?: string;
  isAnchor: boolean;
}

/**
 * Search and filter options for households
 */
export interface HouseholdSearchFilters {
  searchTerm?: string;
  minMemberCount?: number;
  maxMemberCount?: number;
  createdAfter?: Date;
  createdBefore?: Date;
  hasAnchor?: boolean;
}

/**
 * Sorting options for households
 */
export interface HouseholdSortOption {
  field: 'name' | 'memberCount' | 'createdDate' | 'updatedDate';
  direction: 'asc' | 'desc';
  label: string;
}

/**
 * Action that can be performed on a household
 */
export interface HouseholdAction {
  type: 'view' | 'edit' | 'delete' | 'manage-members' | 'settings';
  householdId: number;
  data?: any;
}

/**
 * Available actions for household cards
 */
export const HOUSEHOLD_SORT_OPTIONS: HouseholdSortOption[] = [
  { field: 'name', direction: 'asc', label: 'Name (A-Z)' },
  { field: 'name', direction: 'desc', label: 'Name (Z-A)' },
  { field: 'memberCount', direction: 'desc', label: 'Most Members' },
  { field: 'memberCount', direction: 'asc', label: 'Least Members' },
  { field: 'createdDate', direction: 'desc', label: 'Newest First' },
  { field: 'createdDate', direction: 'asc', label: 'Oldest First' },
  { field: 'updatedDate', direction: 'desc', label: 'Recently Updated' }
];
