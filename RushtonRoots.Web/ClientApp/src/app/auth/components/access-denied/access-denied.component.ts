import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Location } from '@angular/common';

/**
 * AccessDeniedComponent - Display clear access denied message with options
 * 
 * Features:
 * - Clear access denied message
 * - Reason for denial (if available)
 * - Link to request access
 * - Link back to safe page (home)
 * - Contact administrator option
 * 
 * Usage:
 * <app-access-denied 
 *   [reason]="denialReason"
 *   [contactEmail]="adminEmail"
 *   (requestAccess)="handleRequestAccess()">
 * </app-access-denied>
 */
@Component({
  selector: 'app-access-denied',
  templateUrl: './access-denied.component.html',
  styleUrls: ['./access-denied.component.scss'],
  standalone: false
})
export class AccessDeniedComponent {
  constructor(private location: Location) {}

  /** Optional reason for access denial */
  @Input() reason: string = '';
  
  /** Optional administrator contact email */
  @Input() contactEmail: string = '';
  
  /** Optional resource name that was denied */
  @Input() resourceName: string = '';
  
  /** Event emitted when user requests access */
  @Output() requestAccess = new EventEmitter<void>();

  requestingAccess = false;
  requestSuccess = false;

  /**
   * Handle request access action
   * Emits event to parent component to handle the actual request
   */
  onRequestAccess(): void {
    if (!this.requestingAccess) {
      this.requestingAccess = true;
      this.requestSuccess = false;
      this.requestAccess.emit();
      
      // Simulate success feedback after a delay
      // TODO: Replace with actual API response handling
      setTimeout(() => {
        this.requestingAccess = false;
        this.requestSuccess = true;
        
        // Clear success message after 5 seconds
        setTimeout(() => {
          this.requestSuccess = false;
        }, 5000);
      }, 1000);
    }
  }

  /**
   * Get display reason for denial
   * Returns custom reason if provided, otherwise default message
   */
  get displayReason(): string {
    return this.reason || 'You do not have permission to access this resource.';
  }

  /**
   * Check if contact information is available
   */
  get hasContactInfo(): boolean {
    return !!this.contactEmail;
  }

  /**
   * Get mailto link for contacting administrator
   */
  get mailtoLink(): string {
    const subject = encodeURIComponent('Access Request');
    const body = this.resourceName 
      ? encodeURIComponent(`I would like to request access to: ${this.resourceName}`)
      : encodeURIComponent('I would like to request access to a resource.');
    return `mailto:${this.contactEmail}?subject=${subject}&body=${body}`;
  }

  /**
   * Navigate back to previous page
   */
  goBack(): void {
    this.location.back();
  }
}
