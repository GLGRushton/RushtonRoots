import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PersonDeleteDialogData, PersonDeleteOptions } from '../../models/person-delete.model';

/**
 * PersonDeleteDialogComponent - Confirmation dialog for person deletion
 * 
 * Features:
 * - Display person summary (name, dates, photo)
 * - Warning about cascade deletes
 * - List of related data that will be affected
 * - Confirmation checkbox
 * - Soft delete, archive, and hard delete options
 * - Admin-only hard delete option
 * - Optional: Transfer relationships to another person
 * 
 * Usage:
 * constructor(private dialog: MatDialog) {}
 * 
 * const dialogRef = this.dialog.open(PersonDeleteDialogComponent, {
 *   width: '600px',
 *   data: personDeleteData
 * });
 * 
 * dialogRef.afterClosed().subscribe(result => {
 *   if (result) {
 *     // Handle deletion based on result.deleteType
 *   }
 * });
 */
@Component({
  selector: 'app-person-delete-dialog',
  standalone: false,
  templateUrl: './person-delete-dialog.component.html',
  styleUrls: ['./person-delete-dialog.component.scss']
})
export class PersonDeleteDialogComponent implements OnInit {
  deleteForm: FormGroup;
  isAdmin = false; // TODO: Inject AuthService and get user role: constructor(private authService: AuthService) { this.isAdmin = this.authService.isAdmin(); }
  selectedDeleteType: 'soft' | 'archive' | 'hard' = 'soft';

  constructor(
    public dialogRef: MatDialogRef<PersonDeleteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: PersonDeleteDialogData,
    private fb: FormBuilder
  ) {
    this.deleteForm = this.fb.group({
      deleteType: ['soft', Validators.required],
      transferPersonId: [null],
      confirmationCheckbox: [false, Validators.requiredTrue]
    });
  }

  ngOnInit(): void {
    // Subscribe to delete type changes
    this.deleteForm.get('deleteType')?.valueChanges.subscribe(value => {
      this.selectedDeleteType = value;
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
   * Get age or lifespan string
   */
  getLifespan(): string {
    if (!this.data.dateOfBirth) return '';
    
    const birthDate = typeof this.data.dateOfBirth === 'string' 
      ? new Date(this.data.dateOfBirth) 
      : this.data.dateOfBirth;
    
    if (this.data.isDeceased && this.data.dateOfDeath) {
      const deathDate = typeof this.data.dateOfDeath === 'string' 
        ? new Date(this.data.dateOfDeath) 
        : this.data.dateOfDeath;
      const years = deathDate.getFullYear() - birthDate.getFullYear();
      return `(${years} years)`;
    } else {
      const today = new Date();
      const years = today.getFullYear() - birthDate.getFullYear();
      return `(${years} years old)`;
    }
  }

  /**
   * Check if there are any related data items
   */
  hasRelatedData(): boolean {
    const { relatedData } = this.data;
    return relatedData.relationships.total > 0 ||
           relatedData.householdMemberships > 0 ||
           relatedData.photos > 0 ||
           relatedData.stories > 0 ||
           relatedData.documents > 0 ||
           relatedData.lifeEvents > 0;
  }

  /**
   * Get total count of all related items
   */
  getTotalRelatedItems(): number {
    const { relatedData } = this.data;
    return relatedData.relationships.total +
           relatedData.householdMemberships +
           relatedData.photos +
           relatedData.stories +
           relatedData.documents +
           relatedData.lifeEvents;
  }

  /**
   * Get warning message based on delete type
   */
  getWarningMessage(): string {
    switch (this.selectedDeleteType) {
      case 'soft':
        return 'This person will be marked as deleted and hidden from most views. This action can be undone by an administrator.';
      case 'archive':
        return 'This person will be archived and only visible in archive views. Their data will be preserved for historical purposes.';
      case 'hard':
        return 'This person and ALL related data will be permanently deleted. This action CANNOT be undone!';
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
        return 'Delete Person';
      case 'archive':
        return 'Archive Person';
      case 'hard':
        return 'Permanently Delete';
      default:
        return 'Delete';
    }
  }

  /**
   * Get button color based on delete type
   * Hard delete and archive use 'warn' to indicate destructive actions
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
    this.dialogRef.close(null);
  }

  /**
   * Handle delete button click
   */
  onDelete(): void {
    if (this.deleteForm.valid) {
      const result: PersonDeleteOptions = {
        deleteType: this.deleteForm.value.deleteType,
        transferRelationshipsTo: this.deleteForm.value.transferPersonId,
        confirmed: this.deleteForm.value.confirmationCheckbox
      };
      this.dialogRef.close(result);
    }
  }
}
