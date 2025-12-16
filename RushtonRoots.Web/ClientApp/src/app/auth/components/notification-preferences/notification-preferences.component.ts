import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NotificationPreferences } from '../../models/user-profile.model';

/**
 * NotificationPreferencesComponent - Manage notification settings
 * 
 * Features:
 * - Email notification preferences
 * - Push notification preferences
 * - In-app notification preferences
 * - Organized by notification type
 * - Save functionality
 * 
 * Usage:
 * <app-notification-preferences 
 *   [preferences]="notificationPreferences"
 *   (preferencesUpdate)="handleUpdate($event)">
 * </app-notification-preferences>
 */
@Component({
  selector: 'app-notification-preferences',
  templateUrl: './notification-preferences.component.html',
  styleUrls: ['./notification-preferences.component.scss'],
  standalone: false
})
export class NotificationPreferencesComponent implements OnInit {
  /** Current notification preferences */
  @Input() preferences: NotificationPreferences | null = null;
  
  /** Event emitted when preferences are updated */
  @Output() preferencesUpdate = new EventEmitter<NotificationPreferences>();

  notificationForm!: FormGroup;
  hasChanges = false;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initializeForm();
    
    // Track changes
    this.notificationForm.valueChanges.subscribe(() => {
      this.hasChanges = true;
    });
  }

  /**
   * Initialize the notification preferences form
   */
  private initializeForm(): void {
    const defaults = this.preferences || this.getDefaultPreferences();
    
    this.notificationForm = this.fb.group({
      emailNotifications: this.fb.group({
        familyUpdates: [defaults.emailNotifications.familyUpdates],
        newMembers: [defaults.emailNotifications.newMembers],
        comments: [defaults.emailNotifications.comments],
        mentions: [defaults.emailNotifications.mentions],
        weeklyDigest: [defaults.emailNotifications.weeklyDigest],
        monthlyNewsletter: [defaults.emailNotifications.monthlyNewsletter]
      }),
      pushNotifications: this.fb.group({
        enabled: [defaults.pushNotifications.enabled],
        familyUpdates: [defaults.pushNotifications.familyUpdates],
        newMembers: [defaults.pushNotifications.newMembers],
        comments: [defaults.pushNotifications.comments],
        mentions: [defaults.pushNotifications.mentions]
      }),
      inAppNotifications: this.fb.group({
        familyUpdates: [defaults.inAppNotifications.familyUpdates],
        newMembers: [defaults.inAppNotifications.newMembers],
        comments: [defaults.inAppNotifications.comments],
        mentions: [defaults.inAppNotifications.mentions]
      })
    });
  }

  /**
   * Get default notification preferences
   */
  private getDefaultPreferences(): NotificationPreferences {
    return {
      emailNotifications: {
        familyUpdates: true,
        newMembers: true,
        comments: true,
        mentions: true,
        weeklyDigest: false,
        monthlyNewsletter: false
      },
      pushNotifications: {
        enabled: false,
        familyUpdates: false,
        newMembers: false,
        comments: false,
        mentions: false
      },
      inAppNotifications: {
        familyUpdates: true,
        newMembers: true,
        comments: true,
        mentions: true
      }
    };
  }

  /**
   * Save notification preferences
   */
  savePreferences(): void {
    const updatedPreferences: NotificationPreferences = this.notificationForm.value;
    this.preferencesUpdate.emit(updatedPreferences);
    this.hasChanges = false;
  }

  /**
   * Reset to defaults
   */
  resetToDefaults(): void {
    const defaults = this.getDefaultPreferences();
    this.notificationForm.patchValue(defaults);
    this.hasChanges = true;
  }

  /**
   * Check if push notifications are enabled
   */
  get pushNotificationsEnabled(): boolean {
    return this.notificationForm.get('pushNotifications.enabled')?.value || false;
  }
}
