/**
 * Models and interfaces for Partnership Delete Dialog component
 */

/**
 * Data required for the partnership delete dialog
 */
export interface PartnershipDeleteDialogData {
  partnershipId: number;
  
  // Partner A information
  personAId: number;
  personAName: string;
  personAPhotoUrl?: string;
  personABirthDate?: Date | string;
  personADeathDate?: Date | string;
  personAIsDeceased: boolean;
  
  // Partner B information
  personBId: number;
  personBName: string;
  personBPhotoUrl?: string;
  personBBirthDate?: Date | string;
  personBDeathDate?: Date | string;
  personBIsDeceased: boolean;
  
  // Partnership information
  partnershipType: string; // e.g., 'Married', 'Partnered', etc.
  startDate?: Date | string;
  endDate?: Date | string; // If already ended
  location?: string;
  notes?: string;
  
  // Related data counts that will be affected
  relatedData: PartnershipRelatedData;
}

/**
 * Related data that will be affected by deletion
 */
export interface PartnershipRelatedData {
  children: number; // Children from this partnership
  sharedEvents: number; // Events involving both partners
  photos: number; // Photos tagged with both partners
  stories: number; // Stories about this partnership
  documents: number; // Marriage certificates, etc.
}

/**
 * Options for deleting a partnership
 */
export interface PartnershipDeleteOptions {
  deleteType: 'soft' | 'hard' | 'end'; // 'end' = mark as ended instead of delete
  endDate?: Date; // Required if deleteType is 'end'
  transferChildrenTo?: number; // Partnership ID to transfer children to (if any)
  confirmed: boolean;
}

/**
 * Result of partnership deletion operation
 */
export interface PartnershipDeleteResult {
  success: boolean;
  deleteType: 'soft' | 'hard' | 'end';
  message?: string;
  error?: string;
}
