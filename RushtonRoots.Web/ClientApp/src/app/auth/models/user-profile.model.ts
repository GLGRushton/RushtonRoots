/**
 * User Profile & Settings Models
 * TypeScript interfaces and types for user profile and settings components
 */

/**
 * User profile data
 */
export interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  displayName: string;
  bio?: string;
  avatarUrl?: string;
  phoneNumber?: string;
  dateOfBirth?: Date;
  location?: string;
  website?: string;
  createdAt: Date;
  updatedAt: Date;
}

/**
 * Profile edit form data
 */
export interface ProfileEditFormData {
  firstName: string;
  lastName: string;
  displayName: string;
  bio?: string;
  phoneNumber?: string;
  dateOfBirth?: Date;
  location?: string;
  website?: string;
}

/**
 * Avatar upload data
 */
export interface AvatarUpload {
  file: File;
  preview: string;
  cropData?: ImageCropData;
}

/**
 * Image crop data
 */
export interface ImageCropData {
  x: number;
  y: number;
  width: number;
  height: number;
  rotation: number;
  scale: number;
}

/**
 * Notification preference settings
 */
export interface NotificationPreferences {
  emailNotifications: {
    familyUpdates: boolean;
    newMembers: boolean;
    comments: boolean;
    mentions: boolean;
    weeklyDigest: boolean;
    monthlyNewsletter: boolean;
  };
  pushNotifications: {
    enabled: boolean;
    familyUpdates: boolean;
    newMembers: boolean;
    comments: boolean;
    mentions: boolean;
  };
  inAppNotifications: {
    familyUpdates: boolean;
    newMembers: boolean;
    comments: boolean;
    mentions: boolean;
  };
}

/**
 * Privacy settings
 */
export interface PrivacySettings {
  profileVisibility: 'public' | 'family' | 'private';
  showEmail: boolean;
  showPhoneNumber: boolean;
  showDateOfBirth: boolean;
  showLocation: boolean;
  allowSearchEngineIndexing: boolean;
  allowFamilyMemberSearch: boolean;
}

/**
 * Connected account
 */
export interface ConnectedAccount {
  id: string;
  provider: string;
  providerAccountId: string;
  email: string;
  connectedAt: Date;
  lastUsed?: Date;
  status: 'active' | 'revoked' | 'expired';
}

/**
 * Available connected account providers
 */
export const CONNECTED_ACCOUNT_PROVIDERS = [
  {
    id: 'google',
    name: 'Google',
    icon: 'google',
    color: '#4285F4',
    description: 'Connect your Google account for easy sign-in'
  },
  {
    id: 'facebook',
    name: 'Facebook',
    icon: 'facebook',
    color: '#1877F2',
    description: 'Connect your Facebook account for easy sign-in'
  },
  {
    id: 'microsoft',
    name: 'Microsoft',
    icon: 'microsoft',
    color: '#00A4EF',
    description: 'Connect your Microsoft account for easy sign-in'
  }
];

/**
 * Account deletion request data
 */
export interface AccountDeletionRequest {
  reason: string;
  feedback?: string;
  confirmEmail: string;
  confirmPassword: string;
  transferDataTo?: string; // User ID to transfer data to
  deleteImmediately: boolean;
}

/**
 * Account deletion reasons
 */
export const ACCOUNT_DELETION_REASONS = [
  { value: 'privacy', label: 'Privacy concerns' },
  { value: 'not_useful', label: 'Service not useful anymore' },
  { value: 'alternative', label: 'Using an alternative service' },
  { value: 'temporary', label: 'Taking a temporary break' },
  { value: 'other', label: 'Other reason' }
];

/**
 * User settings tab
 */
export type UserSettingsTab = 'profile' | 'notifications' | 'privacy' | 'accounts' | 'security';

/**
 * Settings action state
 */
export interface SettingsActionState {
  loading: boolean;
  error: string | null;
  success: boolean;
  message?: string;
}

/**
 * Profile completeness
 */
export interface ProfileCompleteness {
  percentage: number;
  missingFields: string[];
  suggestions: string[];
}
