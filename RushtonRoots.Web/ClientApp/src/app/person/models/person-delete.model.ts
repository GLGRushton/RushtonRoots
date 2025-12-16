/**
 * Models and interfaces for Person Delete Dialog component
 */

/**
 * Data required for the person delete dialog
 */
export interface PersonDeleteDialogData {
  personId: number;
  fullName: string;
  photoUrl?: string;
  dateOfBirth?: Date | string;
  dateOfDeath?: Date | string;
  isDeceased: boolean;
  householdName?: string;
  
  // Related data counts that will be affected
  relatedData: PersonRelatedData;
}

/**
 * Related data that will be affected by deletion
 */
export interface PersonRelatedData {
  relationships: RelationshipSummary;
  householdMemberships: number;
  photos: number;
  stories: number;
  documents: number;
  lifeEvents: number;
}

/**
 * Summary of relationships that will be affected
 */
export interface RelationshipSummary {
  parents: number;
  children: number;
  spouses: number;
  siblings: number;
  total: number;
}

/**
 * Options for deleting a person
 */
export interface PersonDeleteOptions {
  deleteType: 'soft' | 'hard' | 'archive';
  transferRelationshipsTo?: number; // Person ID to transfer relationships to
  confirmed: boolean;
}

/**
 * Result of person deletion operation
 */
export interface PersonDeleteResult {
  success: boolean;
  deleteType: 'soft' | 'hard' | 'archive';
  message?: string;
  error?: string;
}
