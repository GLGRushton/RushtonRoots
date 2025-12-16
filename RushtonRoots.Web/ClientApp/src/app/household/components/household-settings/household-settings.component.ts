import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { HouseholdDetails, HouseholdSettings } from '../../models/household-details.model';

/**
 * HouseholdSettingsComponent - Manage household settings
 * 
 * Features:
 * - Privacy settings
 * - Member permissions
 * - Notification preferences
 * - Settings validation
 */
@Component({
  selector: 'app-household-settings',
  standalone: false,
  templateUrl: './household-settings.component.html',
  styleUrls: ['./household-settings.component.scss']
})
export class HouseholdSettingsComponent implements OnInit {
  @Input() household!: HouseholdDetails;
  @Input() canEdit = false;

  @Output() settingsUpdated = new EventEmitter<any>();

  settings: HouseholdSettings = {
    privacy: 'FamilyOnly',
    allowMemberInvites: true,
    requireApprovalForNewMembers: false,
    allowMemberPhotos: true,
    allowMemberEdits: false,
    notifyOnNewMembers: true,
    notifyOnEdits: false
  };

  privacyOptions = [
    { value: 'Public', label: 'Public', description: 'Anyone can view this household', icon: 'public' },
    { value: 'FamilyOnly', label: 'Family Only', description: 'Only family members can view', icon: 'group' },
    { value: 'Private', label: 'Private', description: 'Only household members can view', icon: 'lock' }
  ];

  hasUnsavedChanges = false;

  ngOnInit(): void {
    if (this.household.settings) {
      this.settings = { ...this.household.settings };
    } else {
      // Initialize from household properties
      this.settings.privacy = this.household.privacy;
      this.settings.allowMemberInvites = this.household.allowMemberInvites;
    }
  }

  onSettingChange(): void {
    this.hasUnsavedChanges = true;
  }

  onSave(): void {
    this.settingsUpdated.emit(this.settings);
    this.hasUnsavedChanges = false;
  }

  onReset(): void {
    this.ngOnInit();
    this.hasUnsavedChanges = false;
  }

  getPrivacyOption(value: string) {
    return this.privacyOptions.find(o => o.value === value) || this.privacyOptions[1];
  }
}
