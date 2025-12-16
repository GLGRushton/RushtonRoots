import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { 
  UserProfile, 
  ProfileEditFormData, 
  NotificationPreferences, 
  PrivacySettings, 
  ConnectedAccount,
  UserSettingsTab,
  SettingsActionState,
  ProfileCompleteness,
  CONNECTED_ACCOUNT_PROVIDERS
} from '../../models/user-profile.model';

/**
 * UserProfileComponent - Comprehensive user profile and settings management
 * 
 * Features:
 * - Tabbed interface for different settings sections
 * - Profile view and edit mode
 * - Avatar upload with preview
 * - Notification preferences
 * - Privacy settings
 * - Connected accounts management
 * - Account deletion flow
 * - Profile completeness indicator
 * 
 * Usage:
 * <app-user-profile 
 *   [userProfile]="profile" 
 *   [notificationPreferences]="notifications"
 *   [privacySettings]="privacy"
 *   [connectedAccounts]="accounts"
 *   (profileUpdate)="handleProfileUpdate($event)"
 *   (avatarUpload)="handleAvatarUpload($event)"
 *   (notificationsUpdate)="handleNotificationsUpdate($event)"
 *   (privacyUpdate)="handlePrivacyUpdate($event)"
 *   (accountConnect)="handleAccountConnect($event)"
 *   (accountDisconnect)="handleAccountDisconnect($event)"
 *   (accountDelete)="handleAccountDelete($event)">
 * </app-user-profile>
 */
@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
  standalone: false
})
export class UserProfileComponent implements OnInit {
  /** Current user profile data */
  @Input() userProfile: UserProfile | null = null;
  
  /** Notification preferences */
  @Input() notificationPreferences: NotificationPreferences | null = null;
  
  /** Privacy settings */
  @Input() privacySettings: PrivacySettings | null = null;
  
  /** Connected accounts */
  @Input() connectedAccounts: ConnectedAccount[] = [];
  
  /** Whether profile is in edit mode */
  @Input() editMode = false;
  
  /** Can user delete account */
  @Input() canDeleteAccount = true;
  
  /** Event emitted when profile is updated */
  @Output() profileUpdate = new EventEmitter<ProfileEditFormData>();
  
  /** Event emitted when avatar is uploaded */
  @Output() avatarUpload = new EventEmitter<File>();
  
  /** Event emitted when notifications are updated */
  @Output() notificationsUpdate = new EventEmitter<NotificationPreferences>();
  
  /** Event emitted when privacy settings are updated */
  @Output() privacyUpdate = new EventEmitter<PrivacySettings>();
  
  /** Event emitted when account is connected */
  @Output() accountConnect = new EventEmitter<string>();
  
  /** Event emitted when account is disconnected */
  @Output() accountDisconnect = new EventEmitter<string>();
  
  /** Event emitted when account deletion is requested */
  @Output() accountDelete = new EventEmitter<any>();

  /** Active tab */
  activeTab: UserSettingsTab = 'profile';
  
  /** Profile edit form */
  profileForm!: FormGroup;
  
  /** Is edit mode active */
  isEditMode = false;
  
  /** Avatar preview URL */
  avatarPreview: string | null = null;
  
  /** Action state for various operations */
  actionState: SettingsActionState = {
    loading: false,
    error: null,
    success: false
  };
  
  /** Available connected account providers */
  availableProviders = CONNECTED_ACCOUNT_PROVIDERS;
  
  /** Profile completeness data */
  profileCompleteness: ProfileCompleteness = {
    percentage: 0,
    missingFields: [],
    suggestions: []
  };

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initializeProfileForm();
    this.calculateProfileCompleteness();
    this.isEditMode = this.editMode;
  }

  /**
   * Initialize the profile edit form
   */
  private initializeProfileForm(): void {
    this.profileForm = this.fb.group({
      firstName: [this.userProfile?.firstName || '', [Validators.required, Validators.minLength(2)]],
      lastName: [this.userProfile?.lastName || '', [Validators.required, Validators.minLength(2)]],
      displayName: [this.userProfile?.displayName || '', [Validators.required, Validators.minLength(2)]],
      bio: [this.userProfile?.bio || '', [Validators.maxLength(500)]],
      phoneNumber: [this.userProfile?.phoneNumber || '', [Validators.pattern(/^\+?[\d\s-()]+$/)]],
      dateOfBirth: [this.userProfile?.dateOfBirth || null],
      location: [this.userProfile?.location || ''],
      website: [this.userProfile?.website || '', [Validators.pattern(/^https?:\/\/.+/)]]
    });
  }

  /**
   * Calculate profile completeness
   */
  private calculateProfileCompleteness(): void {
    if (!this.userProfile) {
      this.profileCompleteness = { percentage: 0, missingFields: [], suggestions: [] };
      return;
    }

    const fields = [
      { key: 'firstName', label: 'First Name', weight: 15 },
      { key: 'lastName', label: 'Last Name', weight: 15 },
      { key: 'displayName', label: 'Display Name', weight: 10 },
      { key: 'bio', label: 'Bio', weight: 15 },
      { key: 'avatarUrl', label: 'Avatar', weight: 15 },
      { key: 'phoneNumber', label: 'Phone Number', weight: 10 },
      { key: 'dateOfBirth', label: 'Date of Birth', weight: 10 },
      { key: 'location', label: 'Location', weight: 10 }
    ];

    let completedWeight = 0;
    const missingFields: string[] = [];

    fields.forEach(field => {
      const value = (this.userProfile as any)[field.key];
      if (value && value !== '') {
        completedWeight += field.weight;
      } else {
        missingFields.push(field.label);
      }
    });

    this.profileCompleteness = {
      percentage: completedWeight,
      missingFields,
      suggestions: this.generateSuggestions(missingFields)
    };
  }

  /**
   * Generate suggestions for incomplete profile
   */
  private generateSuggestions(missingFields: string[]): string[] {
    const suggestions: string[] = [];
    
    if (missingFields.includes('Avatar')) {
      suggestions.push('Add a profile picture to help others recognize you');
    }
    if (missingFields.includes('Bio')) {
      suggestions.push('Write a short bio to tell your family about yourself');
    }
    if (missingFields.includes('Location')) {
      suggestions.push('Add your location to connect with nearby family members');
    }
    
    return suggestions;
  }

  /**
   * Toggle edit mode
   */
  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
    if (!this.isEditMode) {
      // Reset form if canceling
      this.initializeProfileForm();
      this.avatarPreview = null;
    }
  }

  /**
   * Handle avatar file selection
   */
  onAvatarSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validate file type
      if (!file.type.startsWith('image/')) {
        this.actionState = {
          loading: false,
          error: 'Please select an image file',
          success: false
        };
        return;
      }
      
      // Validate file size (5MB max)
      if (file.size > 5 * 1024 * 1024) {
        this.actionState = {
          loading: false,
          error: 'Image size must be less than 5MB',
          success: false
        };
        return;
      }
      
      // Create preview
      const reader = new FileReader();
      reader.onload = (e) => {
        this.avatarPreview = e.target?.result as string;
      };
      reader.readAsDataURL(file);
      
      // Emit upload event
      this.avatarUpload.emit(file);
    }
  }

  /**
   * Remove avatar preview
   */
  removeAvatarPreview(): void {
    this.avatarPreview = null;
  }

  /**
   * Save profile changes
   */
  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }
    
    this.actionState = { loading: true, error: null, success: false };
    
    const formData: ProfileEditFormData = this.profileForm.value;
    this.profileUpdate.emit(formData);
    
    // Simulate success (in real app, this would be handled by parent component)
    setTimeout(() => {
      this.actionState = { 
        loading: false, 
        error: null, 
        success: true, 
        message: 'Profile updated successfully' 
      };
      this.isEditMode = false;
      this.calculateProfileCompleteness();
    }, 1000);
  }

  /**
   * Check if a provider is connected
   */
  isProviderConnected(providerId: string): boolean {
    return this.connectedAccounts.some(acc => acc.provider === providerId && acc.status === 'active');
  }

  /**
   * Get connected account for provider
   */
  getConnectedAccount(providerId: string): ConnectedAccount | undefined {
    return this.connectedAccounts.find(acc => acc.provider === providerId && acc.status === 'active');
  }

  /**
   * Connect account
   */
  connectAccount(providerId: string): void {
    this.accountConnect.emit(providerId);
  }

  /**
   * Disconnect account
   */
  disconnectAccount(providerId: string): void {
    this.accountDisconnect.emit(providerId);
  }

  /**
   * Get initials for avatar fallback
   */
  getInitials(): string {
    if (!this.userProfile) return '??';
    const first = this.userProfile.firstName?.charAt(0) || '';
    const last = this.userProfile.lastName?.charAt(0) || '';
    return (first + last).toUpperCase() || '??';
  }

  /**
   * Get avatar URL or preview
   */
  getAvatarUrl(): string | null {
    return this.avatarPreview || this.userProfile?.avatarUrl || null;
  }

  /**
   * Handle form field errors
   */
  getFieldError(fieldName: string): string {
    const field = this.profileForm.get(fieldName);
    if (!field || !field.touched || !field.errors) return '';
    
    if (field.errors['required']) return 'This field is required';
    if (field.errors['minlength']) return `Minimum ${field.errors['minlength'].requiredLength} characters`;
    if (field.errors['maxlength']) return `Maximum ${field.errors['maxlength'].requiredLength} characters`;
    if (field.errors['pattern']) return 'Invalid format';
    
    return 'Invalid value';
  }
}
