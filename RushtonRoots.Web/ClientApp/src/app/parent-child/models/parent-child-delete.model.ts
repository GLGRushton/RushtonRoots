/**
 * Models and interfaces for ParentChild Delete Dialog component
 */

/**
 * Data required for the parent-child delete dialog
 */
export interface ParentChildDeleteDialogData {
  relationshipId: number;
  
  // Parent information
  parentId: number;
  parentName: string;
  parentPhotoUrl?: string;
  parentBirthDate?: Date | string;
  parentDeathDate?: Date | string;
  parentIsDeceased: boolean;
  
  // Child information
  childId: number;
  childName: string;
  childPhotoUrl?: string;
  childBirthDate?: Date | string;
  childDeathDate?: Date | string;
  childIsDeceased: boolean;
  
  // Relationship information
  relationshipType: string; // e.g., 'Biological', 'Adopted', 'Step', 'Guardian', 'Foster'
  isVerified: boolean;
  
  // Related data counts that will be affected
  relatedData: ParentChildRelatedData;
}

/**
 * Related data that will be affected by deletion
 */
export interface ParentChildRelatedData {
  // Lineage impacts
  lineageImpact: LineageImpact;
  
  // Sibling relationships
  siblings: number; // Number of sibling relationships that may be affected
  
  // Family tree visualization
  treeNodes: number; // Number of nodes in family tree affected
  
  // Evidence and documentation
  evidence: number; // Evidence items attached to this relationship
  photos: number; // Photos tagged with both parent and child
  stories: number; // Stories about this relationship
}

/**
 * Impact on lineage and ancestry
 */
export interface LineageImpact {
  ancestorsLost: number; // How many ancestors child will lose access to
  descendantsLost: number; // How many descendants parent will lose access to
  generationsAffected: number; // How many generations are impacted
}

/**
 * Relationship impact summary for display
 */
export interface RelationshipImpactSummary {
  description: string;
  severity: 'low' | 'medium' | 'high' | 'critical';
  icon: string;
  color: string;
}

/**
 * Options for deleting a parent-child relationship
 */
export interface ParentChildDeleteOptions {
  deleteType: 'soft' | 'hard' | 'disputed'; // 'disputed' = mark as disputed instead of delete
  disputeReason?: string; // Required if deleteType is 'disputed'
  confirmed: boolean;
}

/**
 * Result of parent-child deletion operation
 */
export interface ParentChildDeleteResult {
  success: boolean;
  deleteType: 'soft' | 'hard' | 'disputed';
  message?: string;
  error?: string;
}
