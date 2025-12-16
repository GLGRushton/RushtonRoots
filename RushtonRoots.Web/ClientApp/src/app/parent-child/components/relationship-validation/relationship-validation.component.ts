import { Component, Input } from '@angular/core';
import { ValidationResult } from '../../models/parent-child.model';

/**
 * RelationshipValidationComponent - Displays relationship validation results
 * Phase 5.2: Parent-Child Relationships
 */
@Component({
  selector: 'app-relationship-validation',
  standalone: false,
  templateUrl: './relationship-validation.component.html',
  styleUrls: ['./relationship-validation.component.scss']
})
export class RelationshipValidationComponent {
  /**
   * Validation result to display
   */
  @Input() validationResult!: ValidationResult;

  /**
   * Show details expanded by default
   */
  @Input() expanded: boolean = true;

  /**
   * Get overall validation status
   */
  get validationStatus(): 'success' | 'warning' | 'error' {
    if (this.validationResult.errors.length > 0) {
      return 'error';
    }
    if (this.validationResult.warnings.length > 0) {
      return 'warning';
    }
    return 'success';
  }

  /**
   * Get status icon
   */
  get statusIcon(): string {
    switch (this.validationStatus) {
      case 'success':
        return 'check_circle';
      case 'warning':
        return 'warning';
      case 'error':
        return 'error';
    }
  }

  /**
   * Get status color
   */
  get statusColor(): string {
    switch (this.validationStatus) {
      case 'success':
        return 'primary';
      case 'warning':
        return 'accent';
      case 'error':
        return 'warn';
    }
  }

  /**
   * Get status message
   */
  get statusMessage(): string {
    if (this.validationResult.errors.length > 0) {
      return `${this.validationResult.errors.length} error(s) found`;
    }
    if (this.validationResult.warnings.length > 0) {
      return `${this.validationResult.warnings.length} warning(s) found`;
    }
    return 'All validation checks passed';
  }
}
