import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RelationshipImpactSummary, ParentChildDeleteOptions } from '../../models/parent-child-delete.model';

/**
 * ParentChildDeleteDialogComponent - Confirmation dialog for parent-child relationship deletion
 * 
 * Features:
 * - Display relationship summary (parent and child with photos)
 * - Warning about impacts:
 *   - Loss of lineage connection (ancestors/descendants)
 *   - Impact on family tree visualization
 *   - May affect relationship calculations
 *   - Sibling relationships may be affected
 * - Show family tree context (mini tree)
 * - Three deletion options:
 *   1. Soft Delete: Mark as deleted, can be restored by admin
 *   2. Mark as Disputed: Preserve relationship but mark as uncertain/disputed
 *   3. Hard Delete (admin only): Permanently delete all data
 * - Confirmation checkbox
 * - Optional dispute reason for disputed relationships
 * 
 * Usage (as Angular Element):
 * <app-parent-child-delete-dialog
 *   [relationship-id]="1"
 *   [parent-name]="'John Doe'"
 *   [child-name]="'Jane Doe'"
 *   ...
 *   (deleteConfirmed)="handleDelete($event)"
 *   (deleteCancelled)="handleCancel()">
 * </app-parent-child-delete-dialog>
 */
@Component({
  selector: 'app-parent-child-delete-dialog',
  standalone: false,
  templateUrl: './parent-child-delete-dialog.component.html',
  styleUrls: ['./parent-child-delete-dialog.component.scss']
})
export class ParentChildDeleteDialogComponent implements OnInit {
  // Inputs (for Angular Elements attribute binding)
  @Input('relationship-id') relationshipId: number = 0;
  @Input('parent-id') parentId: number = 0;
  @Input('parent-name') parentName: string = '';
  @Input('parent-photo-url') parentPhotoUrl?: string;
  @Input('parent-birth-date') parentBirthDate?: Date | string;
  @Input('parent-death-date') parentDeathDate?: Date | string;
  @Input('parent-is-deceased') parentIsDeceased: boolean = false;
  @Input('child-id') childId: number = 0;
  @Input('child-name') childName: string = '';
  @Input('child-photo-url') childPhotoUrl?: string;
  @Input('child-birth-date') childBirthDate?: Date | string;
  @Input('child-death-date') childDeathDate?: Date | string;
  @Input('child-is-deceased') childIsDeceased: boolean = false;
  @Input('relationship-type') relationshipType: string = '';
  @Input('is-verified') isVerified: boolean = false;
  @Input('related-data') relatedDataJson?: string; // JSON string for Angular Elements
  @Input('is-admin') isAdmin: boolean = false;

  // Outputs
  @Output() deleteConfirmed = new EventEmitter<ParentChildDeleteOptions>();
  @Output() deleteCancelled = new EventEmitter<void>();

  deleteForm: FormGroup;
  selectedDeleteType: 'soft' | 'disputed' | 'hard' = 'soft'; // Default to soft delete
  relatedData: any = {
    lineageImpact: {
      ancestorsLost: 0,
      descendantsLost: 0,
      generationsAffected: 0
    },
    siblings: 0,
    treeNodes: 0,
    evidence: 0,
    photos: 0,
    stories: 0
  };

  // Computed impact summaries
  impactSummaries: RelationshipImpactSummary[] = [];

  constructor(private fb: FormBuilder) {
    this.deleteForm = this.fb.group({
      deleteType: ['soft', Validators.required], // Default to soft delete
      disputeReason: [''],
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
        // Use default structure on parse error
        this.relatedData = {
          lineageImpact: {
            ancestorsLost: 0,
            descendantsLost: 0,
            generationsAffected: 0
          },
          siblings: 0,
          treeNodes: 0,
          evidence: 0,
          photos: 0,
          stories: 0
        };
      }
    }

    // Calculate impact summaries
    this.calculateImpactSummaries();

    // Subscribe to delete type changes to update validation
    this.deleteForm.get('deleteType')?.valueChanges.subscribe(value => {
      this.selectedDeleteType = value;
      
      // Make dispute reason required if delete type is 'disputed'
      const disputeReasonControl = this.deleteForm.get('disputeReason');
      if (value === 'disputed') {
        disputeReasonControl?.setValidators([Validators.required, Validators.minLength(10)]);
      } else {
        disputeReasonControl?.clearValidators();
      }
      disputeReasonControl?.updateValueAndValidity();
    });
  }

  /**
   * Calculate impact summaries based on related data
   */
  calculateImpactSummaries(): void {
    this.impactSummaries = [];

    // Lineage impact
    if (this.relatedData.lineageImpact) {
      const ancestorsLost = this.relatedData.lineageImpact.ancestorsLost || 0;
      const descendantsLost = this.relatedData.lineageImpact.descendantsLost || 0;
      
      if (ancestorsLost > 0 || descendantsLost > 0) {
        this.impactSummaries.push({
          description: `${this.childName} will lose connection to ${ancestorsLost} ancestor(s) and ${this.parentName} will lose connection to ${descendantsLost} descendant(s)`,
          severity: ancestorsLost > 10 || descendantsLost > 10 ? 'critical' : 'high',
          icon: 'account_tree',
          color: 'warn'
        });
      }
    }

    // Sibling relationships
    if (this.relatedData.siblings > 0) {
      this.impactSummaries.push({
        description: `${this.relatedData.siblings} sibling relationship(s) may be affected`,
        severity: 'medium',
        icon: 'people',
        color: 'accent'
      });
    }

    // Family tree visualization
    if (this.relatedData.treeNodes > 0) {
      this.impactSummaries.push({
        description: `${this.relatedData.treeNodes} node(s) in the family tree will be disconnected`,
        severity: this.relatedData.treeNodes > 20 ? 'high' : 'medium',
        icon: 'device_hub',
        color: 'accent'
      });
    }

    // Evidence and documentation
    const totalEvidence = (this.relatedData.evidence || 0) + (this.relatedData.photos || 0) + (this.relatedData.stories || 0);
    if (totalEvidence > 0) {
      this.impactSummaries.push({
        description: `${totalEvidence} evidence item(s), photo(s), or story(ies) are attached to this relationship`,
        severity: 'low',
        icon: 'description',
        color: 'primary'
      });
    }
  }

  /**
   * Get the warning message based on selected delete type
   */
  getWarningMessage(): string {
    switch (this.selectedDeleteType) {
      case 'soft':
        return 'This relationship will be marked as deleted but can be restored by an administrator. Related data will be preserved.';
      case 'disputed':
        return 'This relationship will be marked as disputed/uncertain. It will remain visible but with a disputed indicator. You must provide a reason.';
      case 'hard':
        return 'This relationship will be permanently deleted along with all related data. This action cannot be undone.';
      default:
        return '';
    }
  }

  /**
   * Get the button text based on selected delete type
   */
  getDeleteButtonText(): string {
    switch (this.selectedDeleteType) {
      case 'soft':
        return 'Mark as Deleted';
      case 'disputed':
        return 'Mark as Disputed';
      case 'hard':
        return 'Permanently Delete';
      default:
        return 'Delete';
    }
  }

  /**
   * Get the button color based on selected delete type
   */
  getDeleteButtonColor(): string {
    switch (this.selectedDeleteType) {
      case 'soft':
        return 'warn'; // Orange/yellow warning
      case 'disputed':
        return 'accent'; // Blue accent
      case 'hard':
        return 'warn'; // Red danger (Material uses 'warn' for red)
      default:
        return 'warn';
    }
  }

  /**
   * Get the warning class based on selected delete type (safe for ngClass)
   */
  getWarningClass(): string {
    switch (this.selectedDeleteType) {
      case 'soft':
        return 'warning-soft';
      case 'disputed':
        return 'warning-disputed';
      case 'hard':
        return 'warning-hard';
      default:
        return 'warning-soft';
    }
  }

  /**
   * Get parent initials for avatar
   */
  getParentInitials(): string {
    if (!this.parentName) return '?';
    const names = this.parentName.split(' ').filter(n => n.length > 0);
    if (names.length === 0) return '?';
    if (names.length >= 2) {
      return (names[0][0] || '') + (names[names.length - 1][0] || '');
    }
    return this.parentName.substring(0, 2).toUpperCase();
  }

  /**
   * Get child initials for avatar
   */
  getChildInitials(): string {
    if (!this.childName) return '?';
    const names = this.childName.split(' ').filter(n => n.length > 0);
    if (names.length === 0) return '?';
    if (names.length >= 2) {
      return (names[0][0] || '') + (names[names.length - 1][0] || '');
    }
    return this.childName.substring(0, 2).toUpperCase();
  }

  /**
   * Format date for display
   */
  formatDate(date?: Date | string): string {
    if (!date) return '';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }

  /**
   * Get lifespan string (birth - death or birth - present)
   */
  getLifespan(birthDate?: Date | string, deathDate?: Date | string, isDeceased?: boolean): string {
    const birth = birthDate ? this.formatDate(birthDate) : '?';
    if (isDeceased && deathDate) {
      return `${birth} - ${this.formatDate(deathDate)}`;
    }
    return `${birth} - Present`;
  }

  /**
   * Handle confirm button click
   */
  onConfirm(): void {
    if (this.deleteForm.valid) {
      const options: ParentChildDeleteOptions = {
        deleteType: this.selectedDeleteType,
        disputeReason: this.selectedDeleteType === 'disputed' ? this.deleteForm.get('disputeReason')?.value : undefined,
        confirmed: true
      };
      this.deleteConfirmed.emit(options);
    }
  }

  /**
   * Handle cancel button click
   */
  onCancel(): void {
    this.deleteCancelled.emit();
  }
}
