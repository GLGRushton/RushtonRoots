/**
 * Parent-Child relationship TypeScript models for Phase 5.2
 */

/**
 * Parent-child relationship card data for display
 */
export interface ParentChildCard {
  id: number;
  parentPersonId: number;
  childPersonId: number;
  parentName: string;
  childName: string;
  parentPhotoUrl?: string;
  childPhotoUrl?: string;
  relationshipType: string;
  relationshipTypeDisplay: string;
  relationshipTypeIcon: string;
  relationshipTypeColor: string;
  childBirthDate?: Date;
  childAge?: number;
  isVerified: boolean;
  confidence?: number; // For AI suggestions: 0-100
  createdDateTime: Date;
  updatedDateTime: Date;
}

/**
 * Relationship type configuration
 */
export interface RelationshipTypeConfig {
  value: string;
  display: string;
  icon: string;
  color: string;
  description: string;
}

/**
 * Available parent-child relationship types
 */
export const RELATIONSHIP_TYPES: RelationshipTypeConfig[] = [
  { 
    value: 'biological', 
    display: 'Biological', 
    icon: 'bloodtype', 
    color: 'primary',
    description: 'Biological parent-child relationship'
  },
  { 
    value: 'adopted', 
    display: 'Adopted', 
    icon: 'volunteer_activism', 
    color: 'accent',
    description: 'Legal adoption'
  },
  { 
    value: 'step', 
    display: 'Step', 
    icon: 'family_restroom', 
    color: 'accent',
    description: 'Step-parent relationship'
  },
  { 
    value: 'guardian', 
    display: 'Guardian', 
    icon: 'shield', 
    color: 'accent',
    description: 'Legal guardian'
  },
  { 
    value: 'foster', 
    display: 'Foster', 
    icon: 'home', 
    color: 'accent',
    description: 'Foster care relationship'
  },
  { 
    value: 'unknown', 
    display: 'Unknown', 
    icon: 'help_outline', 
    color: 'warn',
    description: 'Relationship type unknown'
  }
];

/**
 * Search filters for parent-child relationships
 */
export interface ParentChildSearchFilters {
  searchText?: string;
  relationshipType?: string;
  parentId?: number;
  childId?: number;
  verifiedOnly?: boolean;
  includeUnverified?: boolean;
}

/**
 * Sort options for parent-child relationships
 */
export interface ParentChildSortOption {
  value: string;
  display: string;
}

/**
 * Available sort options
 */
export const PARENT_CHILD_SORT_OPTIONS: ParentChildSortOption[] = [
  { value: 'childName-asc', display: 'Child Name (A-Z)' },
  { value: 'childName-desc', display: 'Child Name (Z-A)' },
  { value: 'parentName-asc', display: 'Parent Name (A-Z)' },
  { value: 'parentName-desc', display: 'Parent Name (Z-A)' },
  { value: 'childBirthDate-asc', display: 'Birth Date (Oldest First)' },
  { value: 'childBirthDate-desc', display: 'Birth Date (Newest First)' },
  { value: 'created-desc', display: 'Recently Created' },
  { value: 'updated-desc', display: 'Recently Updated' }
];

/**
 * Action event data
 */
export interface ParentChildActionEvent {
  action: 'view' | 'edit' | 'delete' | 'verify';
  parentChildId: number;
}

/**
 * Form data for creating/editing parent-child relationships
 */
export interface ParentChildFormData {
  id?: number;
  parentPersonId?: number;
  childPersonId?: number;
  relationshipType: string;
  notes?: string;
  isVerified?: boolean;
}

/**
 * Person option for autocomplete
 */
export interface PersonOption {
  id: number;
  name: string;
  photoUrl?: string;
  birthDate?: Date;
  deathDate?: Date;
  age?: number;
}

/**
 * Mini family tree node
 */
export interface FamilyTreeNode {
  id: number;
  name: string;
  photoUrl?: string;
  birthDate?: Date;
  deathDate?: Date;
  generation: number; // 0 = focus person, -1 = parents, 1 = children
  parents?: FamilyTreeNode[];
  children?: FamilyTreeNode[];
  spouses?: FamilyTreeNode[];
}

/**
 * Relationship suggestion from AI
 */
export interface RelationshipSuggestion {
  id: string;
  parentPersonId: number;
  childPersonId: number;
  parentName: string;
  childName: string;
  parentPhotoUrl?: string;
  childPhotoUrl?: string;
  confidence: number; // 0-100
  reasoning: string;
  suggestedType: string;
  sources?: string[]; // Evidence sources
}

/**
 * Bulk import data
 */
export interface BulkImportData {
  parentName: string;
  childName: string;
  relationshipType: string;
  notes?: string;
}

/**
 * Bulk import result
 */
export interface BulkImportResult {
  total: number;
  successful: number;
  failed: number;
  errors: BulkImportError[];
}

/**
 * Bulk import error
 */
export interface BulkImportError {
  row: number;
  parentName: string;
  childName: string;
  error: string;
}

/**
 * Validation result
 */
export interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
  warnings: ValidationWarning[];
}

/**
 * Validation error
 */
export interface ValidationError {
  type: 'duplicate' | 'circular' | 'age-mismatch' | 'missing-person' | 'already-exists';
  message: string;
  details?: string;
}

/**
 * Validation warning
 */
export interface ValidationWarning {
  type: 'age-gap' | 'multiple-biological' | 'unusual-pattern';
  message: string;
  details?: string;
}

/**
 * Parent-child relationship details for details view
 */
export interface ParentChildDetails {
  id: number;
  parentPersonId: number;
  childPersonId: number;
  parentName: string;
  childName: string;
  parentPhotoUrl?: string;
  childPhotoUrl?: string;
  parentBirthDate?: Date;
  parentDeathDate?: Date;
  childBirthDate?: Date;
  childDeathDate?: Date;
  relationshipType: string;
  relationshipTypeDisplay: string;
  relationshipTypeIcon: string;
  relationshipTypeColor: string;
  relationshipTypeDescription: string;
  isVerified: boolean;
  confidence?: number;
  notes?: string;
  createdDateTime: Date;
  updatedDateTime: Date;
}

/**
 * Evidence item for relationship
 */
export interface ParentChildEvidence {
  id: number;
  type: 'source' | 'document' | 'dna' | 'photo' | 'other';
  title: string;
  description?: string;
  url?: string;
  documentUrl?: string;
  addedDate: Date;
}

/**
 * Timeline event for parent-child relationship
 */
export interface ParentChildEvent {
  id: number;
  title: string;
  date: Date;
  description?: string;
  type: 'birth' | 'adoption' | 'guardianship' | 'other';
  icon: string;
  color: string;
}

/**
 * Tab configuration for details component
 */
export interface ParentChildDetailsTab {
  label: string;
  icon: string;
  content: string;
  badge?: number;
}
