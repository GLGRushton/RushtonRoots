/**
 * TypeScript interfaces for Household Details components
 * These models support Phase 4.2 - Household Details & Members
 */

import { HouseholdMember } from './household.model';

/**
 * Detailed household information
 */
export interface HouseholdDetails {
  id: number;
  householdName: string;
  description?: string;
  anchorPersonId?: number;
  anchorPersonName?: string;
  anchorPersonPhotoUrl?: string;
  memberCount: number;
  createdDateTime: Date | string;
  updatedDateTime: Date | string;
  createdByUserId?: number;
  createdByUserName?: string;
  privacy: 'Public' | 'FamilyOnly' | 'Private';
  allowMemberInvites: boolean;
  members?: HouseholdMemberDetails[];
  settings?: HouseholdSettings;
}

/**
 * Extended household member with additional details
 */
export interface HouseholdMemberDetails extends HouseholdMember {
  email?: string;
  phone?: string;
  role: HouseholdRole;
  permissions: HouseholdPermissions;
  joinedDate: Date | string;
  invitedBy?: string;
  status: 'Active' | 'Invited' | 'Inactive';
  lastActive?: Date | string;
}

/**
 * Household member roles
 */
export type HouseholdRole = 'Owner' | 'Admin' | 'Member' | 'Viewer';

/**
 * Role configuration
 */
export interface RoleConfig {
  role: HouseholdRole;
  label: string;
  description: string;
  color: string;
  icon: string;
}

/**
 * Available roles with their configurations
 */
export const HOUSEHOLD_ROLES: RoleConfig[] = [
  {
    role: 'Owner',
    label: 'Owner',
    description: 'Full control over household and all settings',
    color: 'primary',
    icon: 'star'
  },
  {
    role: 'Admin',
    label: 'Admin',
    description: 'Can manage members and edit household information',
    color: 'accent',
    icon: 'admin_panel_settings'
  },
  {
    role: 'Member',
    label: 'Member',
    description: 'Can view and contribute to household content',
    color: 'default',
    icon: 'person'
  },
  {
    role: 'Viewer',
    label: 'Viewer',
    description: 'Can only view household information',
    color: 'default',
    icon: 'visibility'
  }
];

/**
 * Household member permissions
 */
export interface HouseholdPermissions {
  canEdit: boolean;
  canDelete: boolean;
  canInviteMembers: boolean;
  canManageMembers: boolean;
  canEditSettings: boolean;
  canViewPrivateInfo: boolean;
  canUploadPhotos: boolean;
  canEditPhotos: boolean;
}

/**
 * Household settings
 */
export interface HouseholdSettings {
  privacy: 'Public' | 'FamilyOnly' | 'Private';
  allowMemberInvites: boolean;
  requireApprovalForNewMembers: boolean;
  allowMemberPhotos: boolean;
  allowMemberEdits: boolean;
  notifyOnNewMembers: boolean;
  notifyOnEdits: boolean;
}

/**
 * Household activity event
 */
export interface HouseholdActivityEvent {
  id: number;
  householdId: number;
  eventType: 'member_joined' | 'member_left' | 'member_invited' | 'household_created' | 
             'household_updated' | 'photo_uploaded' | 'member_role_changed' | 'settings_changed';
  title: string;
  description?: string;
  timestamp: Date | string;
  userId?: number;
  userName?: string;
  userPhotoUrl?: string;
  metadata?: any;
  icon?: string;
}

/**
 * Member invitation data
 */
export interface MemberInvitation {
  email: string;
  firstName?: string;
  lastName?: string;
  role: HouseholdRole;
  personalMessage?: string;
}

/**
 * Household details tab
 */
export interface HouseholdDetailsTab {
  label: string;
  icon?: string;
  content: 'overview' | 'members' | 'settings' | 'activity';
  badge?: number;
}

/**
 * Member action types
 */
export type MemberActionType = 'edit' | 'change-role' | 'remove' | 'resend-invite' | 'view-profile';

/**
 * Member action event
 */
export interface MemberActionEvent {
  action: MemberActionType;
  memberId: number;
  memberPersonId: number;
  data?: any;
}
