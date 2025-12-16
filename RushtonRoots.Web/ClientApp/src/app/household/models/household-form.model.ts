/**
 * Models and interfaces for Household Form components
 */

/**
 * Form data for creating or editing a household
 */
export interface HouseholdFormData {
  // Basic Information
  id?: number; // For edit mode
  householdName: string;
  description?: string;
  
  // Anchor Person
  anchorPersonId?: number;
  anchorPersonName?: string; // For display in edit mode
  
  // Initial Members (for create mode)
  initialMemberIds?: number[];
  initialMembers?: HouseholdFormMember[];
  
  // Privacy Settings
  privacyLevel: 'public' | 'family' | 'private';
  
  // Permissions
  allowMemberInvites: boolean;
  allowMemberEdits: boolean;
  allowMemberUploads: boolean;
}

/**
 * Person selector option for autocomplete
 */
export interface PersonOption {
  id: number;
  fullName: string;
  firstName: string;
  lastName: string;
  dateOfBirth?: Date | string;
  photoUrl?: string;
  householdName?: string;
}

/**
 * Member selected for the household
 */
export interface HouseholdFormMember {
  personId: number;
  fullName: string;
  photoUrl?: string;
  role: 'admin' | 'editor' | 'contributor' | 'viewer';
  canInvite: boolean;
  canEdit: boolean;
  canUpload: boolean;
}

/**
 * Privacy level option
 */
export interface PrivacyOption {
  value: 'public' | 'family' | 'private';
  label: string;
  description: string;
  icon: string;
}

/**
 * Available privacy options for households
 */
export const PRIVACY_OPTIONS: PrivacyOption[] = [
  {
    value: 'public',
    label: 'Public',
    description: 'Visible to everyone, including non-family members',
    icon: 'public'
  },
  {
    value: 'family',
    label: 'Family Only',
    description: 'Visible only to registered family members',
    icon: 'people'
  },
  {
    value: 'private',
    label: 'Private',
    description: 'Visible only to household members',
    icon: 'lock'
  }
];

/**
 * Member role option
 */
export interface MemberRoleOption {
  value: 'admin' | 'editor' | 'contributor' | 'viewer';
  label: string;
  description: string;
  icon: string;
  color: string;
}

/**
 * Available member roles
 */
export const MEMBER_ROLES: MemberRoleOption[] = [
  {
    value: 'admin',
    label: 'Admin',
    description: 'Full control over household and members',
    icon: 'admin_panel_settings',
    color: 'warn'
  },
  {
    value: 'editor',
    label: 'Editor',
    description: 'Can edit household information and manage members',
    icon: 'edit',
    color: 'primary'
  },
  {
    value: 'contributor',
    label: 'Contributor',
    description: 'Can add content but not manage household',
    icon: 'person',
    color: 'accent'
  },
  {
    value: 'viewer',
    label: 'Viewer',
    description: 'Can only view household information',
    icon: 'visibility',
    color: ''
  }
];

/**
 * Validation error interface
 */
export interface HouseholdValidationError {
  field: string;
  message: string;
}
