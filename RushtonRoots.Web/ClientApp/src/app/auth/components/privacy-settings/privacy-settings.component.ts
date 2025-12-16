import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PrivacySettings } from '../../models/user-profile.model';

/**
 * PrivacySettingsComponent - Manage privacy settings
 */
@Component({
  selector: 'app-privacy-settings',
  templateUrl: './privacy-settings.component.html',
  styleUrls: ['./privacy-settings.component.scss'],
  standalone: false
})
export class PrivacySettingsComponent implements OnInit {
  @Input() settings: PrivacySettings | null = null;
  @Output() settingsUpdate = new EventEmitter<PrivacySettings>();

  privacyForm!: FormGroup;
  hasChanges = false;

  visibilityOptions = [
    { value: 'public', label: 'Public', description: 'Anyone can view your profile' },
    { value: 'family', label: 'Family Only', description: 'Only family members can view your profile' },
    { value: 'private', label: 'Private', description: 'Only you can view your profile' }
  ];

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initializeForm();
    this.privacyForm.valueChanges.subscribe(() => {
      this.hasChanges = true;
    });
  }

  private initializeForm(): void {
    const defaults = this.settings || this.getDefaultSettings();
    
    this.privacyForm = this.fb.group({
      profileVisibility: [defaults.profileVisibility],
      showEmail: [defaults.showEmail],
      showPhoneNumber: [defaults.showPhoneNumber],
      showDateOfBirth: [defaults.showDateOfBirth],
      showLocation: [defaults.showLocation],
      allowSearchEngineIndexing: [defaults.allowSearchEngineIndexing],
      allowFamilyMemberSearch: [defaults.allowFamilyMemberSearch]
    });
  }

  private getDefaultSettings(): PrivacySettings {
    return {
      profileVisibility: 'family',
      showEmail: false,
      showPhoneNumber: false,
      showDateOfBirth: true,
      showLocation: true,
      allowSearchEngineIndexing: false,
      allowFamilyMemberSearch: true
    };
  }

  saveSettings(): void {
    const updatedSettings: PrivacySettings = this.privacyForm.value;
    this.settingsUpdate.emit(updatedSettings);
    this.hasChanges = false;
  }

  resetToDefaults(): void {
    const defaults = this.getDefaultSettings();
    this.privacyForm.patchValue(defaults);
    this.hasChanges = true;
  }
}
