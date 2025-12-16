/**
 * Partnership-related TypeScript models for Phase 5.1
 */

/**
 * Partnership card data for display in PartnershipCardComponent
 */
export interface PartnershipCard {
  id: number;
  personAId: number;
  personBId: number;
  personAName: string;
  personBName: string;
  personAPhotoUrl?: string;
  personBPhotoUrl?: string;
  partnershipType: string;
  partnershipTypeDisplay: string;
  startDate?: Date;
  endDate?: Date;
  duration?: string; // e.g., "15 years, 3 months"
  status: PartnershipStatus;
  statusDisplay: string;
  statusColor: string;
  location?: string;
  notes?: string;
  createdDateTime: Date;
  updatedDateTime: Date;
}

/**
 * Partnership status enum
 */
export enum PartnershipStatus {
  Current = 'current',
  Ended = 'ended',
  Divorced = 'divorced',
  Widowed = 'widowed',
  Separated = 'separated',
  Unknown = 'unknown'
}

/**
 * Partnership status configuration
 */
export interface PartnershipStatusConfig {
  value: PartnershipStatus;
  display: string;
  color: string;
  icon: string;
}

/**
 * Available partnership statuses with display configurations
 */
export const PARTNERSHIP_STATUSES: PartnershipStatusConfig[] = [
  { value: PartnershipStatus.Current, display: 'Current', color: 'primary', icon: 'favorite' },
  { value: PartnershipStatus.Ended, display: 'Ended', color: 'accent', icon: 'history' },
  { value: PartnershipStatus.Divorced, display: 'Divorced', color: 'warn', icon: 'heart_broken' },
  { value: PartnershipStatus.Widowed, display: 'Widowed', color: 'accent', icon: 'sentiment_very_dissatisfied' },
  { value: PartnershipStatus.Separated, display: 'Separated', color: 'accent', icon: 'pending' },
  { value: PartnershipStatus.Unknown, display: 'Unknown', color: 'accent', icon: 'help_outline' }
];

/**
 * Partnership type configuration
 */
export interface PartnershipTypeConfig {
  value: string;
  display: string;
  icon: string;
  description: string;
}

/**
 * Available partnership types
 */
export const PARTNERSHIP_TYPES: PartnershipTypeConfig[] = [
  { value: 'married', display: 'Married', icon: 'favorite', description: 'Legal marriage' },
  { value: 'partnered', display: 'Partnered', icon: 'volunteer_activism', description: 'Domestic partnership or civil union' },
  { value: 'engaged', display: 'Engaged', icon: 'diamond', description: 'Engaged to be married' },
  { value: 'relationship', display: 'Relationship', icon: 'people', description: 'Romantic relationship' },
  { value: 'commonlaw', display: 'Common Law', icon: 'handshake', description: 'Common law marriage' },
  { value: 'other', display: 'Other', icon: 'more_horiz', description: 'Other type of partnership' }
];

/**
 * Search filters for partnerships
 */
export interface PartnershipSearchFilters {
  searchText?: string;
  partnershipType?: string;
  status?: PartnershipStatus;
  startDateFrom?: Date;
  startDateTo?: Date;
  endDateFrom?: Date;
  endDateTo?: Date;
  personId?: number; // Filter by specific person
}

/**
 * Sort options for partnership list
 */
export enum PartnershipSortField {
  StartDate = 'startDate',
  EndDate = 'endDate',
  PersonAName = 'personAName',
  PersonBName = 'personBName',
  PartnershipType = 'partnershipType',
  CreatedDate = 'createdDate'
}

/**
 * Sort configuration
 */
export interface PartnershipSortOption {
  field: PartnershipSortField;
  direction: 'asc' | 'desc';
  display: string;
}

/**
 * Available sort options
 */
export const PARTNERSHIP_SORT_OPTIONS: PartnershipSortOption[] = [
  { field: PartnershipSortField.StartDate, direction: 'desc', display: 'Start Date (Newest)' },
  { field: PartnershipSortField.StartDate, direction: 'asc', display: 'Start Date (Oldest)' },
  { field: PartnershipSortField.PersonAName, direction: 'asc', display: 'Name (A-Z)' },
  { field: PartnershipSortField.PersonAName, direction: 'desc', display: 'Name (Z-A)' },
  { field: PartnershipSortField.PartnershipType, direction: 'asc', display: 'Type' },
  { field: PartnershipSortField.CreatedDate, direction: 'desc', display: 'Recently Added' }
];

/**
 * Partnership action event
 */
export interface PartnershipActionEvent {
  action: 'view' | 'edit' | 'delete' | 'timeline';
  partnership: PartnershipCard;
}

/**
 * Partnership form data
 */
export interface PartnershipFormData {
  id?: number;
  personAId?: number;
  personBId?: number;
  partnershipType: string;
  startDate?: Date;
  endDate?: Date;
  location?: string;
  notes?: string;
}

/**
 * Person selection option for autocomplete
 */
export interface PersonOption {
  id: number;
  name: string;
  photoUrl?: string;
  birthDate?: Date;
  deathDate?: Date;
  lifeSpan?: string;
}

/**
 * Timeline event for partnership visualization
 */
export interface PartnershipTimelineEvent {
  id: number;
  date: Date;
  dateDisplay: string;
  eventType: 'start' | 'end' | 'marriage' | 'engagement' | 'separation' | 'divorce' | 'other';
  eventTypeDisplay: string;
  title: string;
  description?: string;
  icon: string;
  color: string;
  location?: string;
}

/**
 * Partnership timeline data
 */
export interface PartnershipTimeline {
  partnership: PartnershipCard;
  events: PartnershipTimelineEvent[];
  duration?: string;
  yearsActive?: number;
}
