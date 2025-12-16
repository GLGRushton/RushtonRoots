import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HouseholdDeleteDialogData, HouseholdDeleteOptions, HouseholdRelatedData } from '../../models/household-delete.model';

/**
 * HouseholdDeleteDialogComponent - Confirmation dialog for household deletion
 * 
 * Features:
 * - Display household summary (name, member count, anchor person)
 * - Warning about impacts (members losing access, events, media, documents)
 * - List of related data that will be affected
 * - Confirmation checkbox
 * - Option to notify all members via email
 * - Soft delete, archive, and hard delete options
 * - Admin-only hard delete option
 * 
 * Usage as Angular Element in Razor view:
 * <app-household-delete-dialog
 *   household-id="123"
 *   household-name="Smith Family"
 *   anchor-person-name="John Smith"
 *   member-count="5"
 *   created-date="2020-01-15"
 *   is-admin="true"
 *   related-data='{"members":5,"events":12,"sharedMedia":45,"documents":8,"permissions":15}'>
 * </app-household-delete-dialog>
 */
@Component({
  selector: 'app-household-delete-dialog',
  standalone: false,
  templateUrl: './household-delete-dialog.component.html',
  styleUrls: ['./household-delete-dialog.component.scss']
})
export class HouseholdDeleteDialogComponent implements OnInit {
  @Input('household-id') householdId: number = 0;
  @Input('household-name') householdName: string = '';
  @Input('anchor-person-name') anchorPersonName?: string;
  @Input('anchor-person-id') anchorPersonId?: number;
  @Input('member-count') memberCount: number = 0;
  @Input('created-date') createdDate?: string;
  @Input('is-admin') isAdmin: string = 'false';
  @Input('related-data') relatedDataJson: string = '{}';
  
  @Output() deleteConfirmed = new EventEmitter<HouseholdDeleteOptions>();
  @Output() deleteCancelled = new EventEmitter<void>();
  
  deleteForm: FormGroup;
  selectedDeleteType: 'soft' | 'archive' | 'hard' = 'soft';
  data: HouseholdDeleteDialogData;
  
  constructor(private fb: FormBuilder) {
    this.deleteForm = this.fb.group({
      deleteType: ['soft', Validators.required],
      notifyMembers: [true], // Default to notifying members
      confirmationCheckbox: [false, Validators.requiredTrue]
    });
    
    // Initialize data with defaults
    this.data = {
      householdId: 0,
      householdName: '',
      memberCount: 0,
      relatedData: {
        members: 0,
        events: 0,
        sharedMedia: 0,
        documents: 0,
        permissions: 0
      }
    };
  }

  ngOnInit(): void {
    // Parse inputs and populate data object
    this.data = {
      householdId: this.householdId,
      householdName: this.householdName,
      anchorPersonName: this.anchorPersonName,
      anchorPersonId: this.anchorPersonId,
      memberCount: this.memberCount,
      createdDate: this.createdDate,
      isAdmin: this.isAdmin === 'true',
      relatedData: this.parseRelatedData()
    };
    
    // Subscribe to delete type changes
    this.deleteForm.get('deleteType')?.valueChanges.subscribe(value => {
      this.selectedDeleteType = value;
    });
  }
  
  /**
   * Parse related data JSON string
   */
  private parseRelatedData(): HouseholdRelatedData {
    try {
      return JSON.parse(this.relatedDataJson);
    } catch (e) {
      console.error('Failed to parse related data JSON:', e);
      return {
        members: 0,
        events: 0,
        sharedMedia: 0,
        documents: 0,
        permissions: 0
      };
    }
  }

  /**
   * Get formatted date string
   */
  formatDate(date?: Date | string): string {
    if (!date) return 'Unknown';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  }

  /**
   * Check if there are any related data items
   */
  hasRelatedData(): boolean {
    const { relatedData } = this.data;
    return relatedData.members > 0 ||
           relatedData.events > 0 ||
           relatedData.sharedMedia > 0 ||
           relatedData.documents > 0 ||
           relatedData.permissions > 0;
  }

  /**
   * Get total count of all related items
   */
  getTotalRelatedItems(): number {
    const { relatedData } = this.data;
    return relatedData.members +
           relatedData.events +
           relatedData.sharedMedia +
           relatedData.documents +
           relatedData.permissions;
  }

  /**
   * Get warning message based on delete type
   */
  getWarningMessage(): string {
    switch (this.selectedDeleteType) {
      case 'soft':
        return 'This household will be marked as deleted and hidden from most views. Members will lose access to household features. This action can be undone by an administrator.';
      case 'archive':
        return 'This household will be archived and only visible in archive views. All data will be preserved for historical purposes, but members will no longer have active access.';
      case 'hard':
        return 'This household and ALL related data will be permanently deleted. All members will lose access and all associated events, media, and documents will be removed. This action CANNOT be undone!';
      default:
        return '';
    }
  }

  /**
   * Get button text based on delete type
   */
  getDeleteButtonText(): string {
    switch (this.selectedDeleteType) {
      case 'soft':
        return 'Delete Household';
      case 'archive':
        return 'Archive Household';
      case 'hard':
        return 'Permanently Delete';
      default:
        return 'Delete';
    }
  }

  /**
   * Get button color based on delete type
   * Hard delete uses 'warn' to indicate highly destructive action
   * Archive uses 'warn' as it's still a significant action
   * Soft delete uses 'accent' as it's less destructive
   */
  getDeleteButtonColor(): 'primary' | 'accent' | 'warn' {
    if (this.selectedDeleteType === 'hard' || this.selectedDeleteType === 'archive') {
      return 'warn';
    }
    return 'accent';
  }

  /**
   * Handle cancel button click
   */
  onCancel(): void {
    this.deleteCancelled.emit();
  }

  /**
   * Handle delete button click
   */
  onDelete(): void {
    if (this.deleteForm.valid) {
      const result: HouseholdDeleteOptions = {
        deleteType: this.deleteForm.value.deleteType,
        notifyMembers: this.deleteForm.value.notifyMembers,
        confirmed: this.deleteForm.value.confirmationCheckbox
      };
      this.deleteConfirmed.emit(result);
    }
  }
}
