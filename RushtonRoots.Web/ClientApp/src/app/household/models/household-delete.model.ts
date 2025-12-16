/**
 * Models and interfaces for Household Delete Dialog component
 */

/**
 * Data required for the household delete dialog
 */
export interface HouseholdDeleteDialogData {
  householdId: number;
  householdName: string;
  anchorPersonName?: string;
  anchorPersonId?: number;
  memberCount: number;
  createdDate?: Date | string;
  
  // Related data counts that will be affected
  relatedData: HouseholdRelatedData;
  
  // Current user information for admin checks
  isAdmin?: boolean;
}

/**
 * Related data that will be affected by deletion
 */
export interface HouseholdRelatedData {
  members: number;
  events: number;
  sharedMedia: number;
  documents: number;
  permissions: number;
}

/**
 * Options for deleting a household
 */
export interface HouseholdDeleteOptions {
  deleteType: 'soft' | 'hard' | 'archive';
  notifyMembers: boolean; // Option to send notification emails to all members
  confirmed: boolean;
}

/**
 * Result of household deletion operation
 */
export interface HouseholdDeleteResult {
  success: boolean;
  deleteType: 'soft' | 'hard' | 'archive';
  message?: string;
  error?: string;
}
