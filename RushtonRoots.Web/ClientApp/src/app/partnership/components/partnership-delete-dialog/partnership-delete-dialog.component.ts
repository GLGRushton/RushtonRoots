import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PartnershipDeleteDialogData, PartnershipDeleteOptions } from '../../models/partnership-delete.model';

/**
 * PartnershipDeleteDialogComponent - Confirmation dialog for partnership deletion
 * 
 * Features:
 * - Display partnership summary (both partners with photos)
 * - Warning about impacts (children, shared events, photos)
 * - List of related data that will be affected
 * - Three deletion options:
 *   1. Soft Delete: Mark as deleted, can be restored by admin
 *   2. End Partnership: Mark with end date, preserve as historical record
 *   3. Hard Delete (admin only): Permanently delete all data
 * - Confirmation checkbox
 * - Optional: Transfer children to another partnership
 * 
 * Usage (as Angular Element):
 * <app-partnership-delete-dialog
 *   [partnership-id]="1"
 *   [person-a-name]="'John Doe'"
 *   ...
 *   (deleteConfirmed)="handleDelete($event)"
 *   (deleteCancelled)="handleCancel()">
 * </app-partnership-delete-dialog>
 */
@Component({
  selector: 'app-partnership-delete-dialog',
  standalone: false,
  templateUrl: './partnership-delete-dialog.component.html',
  styleUrls: ['./partnership-delete-dialog.component.scss']
})
export class PartnershipDeleteDialogComponent implements OnInit {
  // Inputs (for Angular Elements attribute binding)
  @Input('partnership-id') partnershipId: number = 0;
  @Input('person-a-id') personAId: number = 0;
  @Input('person-a-name') personAName: string = '';
  @Input('person-a-photo-url') personAPhotoUrl?: string;
  @Input('person-a-birth-date') personABirthDate?: Date | string;
  @Input('person-a-death-date') personADeathDate?: Date | string;
  @Input('person-a-is-deceased') personAIsDeceased: boolean = false;
  @Input('person-b-id') personBId: number = 0;
  @Input('person-b-name') personBName: string = '';
  @Input('person-b-photo-url') personBPhotoUrl?: string;
  @Input('person-b-birth-date') personBBirthDate?: Date | string;
  @Input('person-b-death-date') personBDeathDate?: Date | string;
  @Input('person-b-is-deceased') personBIsDeceased: boolean = false;
  @Input('partnership-type') partnershipType: string = '';
  @Input('start-date') startDate?: Date | string;
  @Input('end-date') endDate?: Date | string;
  @Input('location') location?: string;
  @Input('notes') notes?: string;
  @Input('related-data') relatedDataJson?: string; // JSON string for Angular Elements
  @Input('is-admin') isAdmin: boolean = false;

  // Outputs
  @Output() deleteConfirmed = new EventEmitter<PartnershipDeleteOptions>();
  @Output() deleteCancelled = new EventEmitter<void>();

  deleteForm: FormGroup;
  selectedDeleteType: 'soft' | 'end' | 'hard' = 'end'; // Default to "end partnership"
  relatedData: any = {
    children: 0,
    sharedEvents: 0,
    photos: 0,
    stories: 0,
    documents: 0
  };

  constructor(private fb: FormBuilder) {
    this.deleteForm = this.fb.group({
      deleteType: ['end', Validators.required], // Default to "end partnership"
      endDate: [new Date()], // Default to today for end partnership
      transferChildrenTo: [null],
      confirmationCheckbox: [false, Validators.requiredTrue]
    });
  }

  ngOnInit(): void {
    // Parse related data if provided as JSON string
    if (this.relatedDataJson) {
      try {
        this.relatedData = JSON.parse(this.relatedDataJson);
      } catch (e) {
        console.error('Error parsing related data:', e);
      }
    }

    // Subscribe to delete type changes to update validation
    this.deleteForm.get('deleteType')?.valueChanges.subscribe(value => {
      this.selectedDeleteType = value;
      
      // Make end date required if delete type is 'end'
      const endDateControl = this.deleteForm.get('endDate');
      if (value === 'end') {
        endDateControl?.setValidators([Validators.required]);
      } else {
        endDateControl?.clearValidators();
      }
      endDateControl?.updateValueAndValidity();
    });
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
   * Get lifespan string for a person
   */
  getPersonLifespan(birthDate?: Date | string, deathDate?: Date | string, isDeceased?: boolean): string {
    if (!birthDate) return '';
    
    const birth = typeof birthDate === 'string' ? new Date(birthDate) : birthDate;
    const birthYear = birth.getFullYear();
    
    if (isDeceased && deathDate) {
      const death = typeof deathDate === 'string' ? new Date(deathDate) : deathDate;
      const deathYear = death.getFullYear();
      return `(${birthYear} - ${deathYear})`;
    } else {
      return `(b. ${birthYear})`;
    }
  }

  /**
   * Check if there are any related data items
   */
  hasRelatedData(): boolean {
    return this.relatedData.children > 0 ||
           this.relatedData.sharedEvents > 0 ||
           this.relatedData.photos > 0 ||
           this.relatedData.stories > 0 ||
           this.relatedData.documents > 0;
  }

  /**
   * Get total count of all related items
   */
  getTotalRelatedItems(): number {
    return this.relatedData.children +
           this.relatedData.sharedEvents +
           this.relatedData.photos +
           this.relatedData.stories +
           this.relatedData.documents;
  }

  /**
   * Get warning message based on delete type
   */
  getWarningMessage(): string {
    switch (this.selectedDeleteType) {
      case 'soft':
        return 'This partnership will be marked as deleted and hidden from most views. This action can be undone by an administrator.';
      case 'end':
        return 'This partnership will be marked as ended with the specified date. The historical record will be preserved, and children will retain their parent partnership reference.';
      case 'hard':
        return 'This partnership and ALL related data will be permanently deleted. Children will lose their parent partnership reference. This action CANNOT be undone!';
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
        return 'Delete Partnership';
      case 'end':
        return 'End Partnership';
      case 'hard':
        return 'Permanently Delete';
      default:
        return 'Delete';
    }
  }

  /**
   * Get button color based on delete type
   * Hard delete uses 'warn' to indicate highly destructive action
   * End partnership uses 'primary' as it's the recommended option
   * Soft delete uses 'accent' as it's moderately destructive
   */
  getDeleteButtonColor(): 'primary' | 'accent' | 'warn' {
    if (this.selectedDeleteType === 'hard') {
      return 'warn';
    } else if (this.selectedDeleteType === 'end') {
      return 'primary';
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
   * Handle delete/end button click
   */
  onDelete(): void {
    if (this.deleteForm.valid) {
      const result: PartnershipDeleteOptions = {
        deleteType: this.deleteForm.value.deleteType,
        endDate: this.deleteForm.value.deleteType === 'end' ? this.deleteForm.value.endDate : undefined,
        transferChildrenTo: this.deleteForm.value.transferChildrenTo,
        confirmed: this.deleteForm.value.confirmationCheckbox
      };
      this.deleteConfirmed.emit(result);
    }
  }
}
