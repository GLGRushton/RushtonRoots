import { Component, Input, Output, EventEmitter } from '@angular/core';

/**
 * ForgotPasswordConfirmationComponent - Confirmation screen after password reset request
 * 
 * Features:
 * - Success message display
 * - Email sent indicator
 * - Link to login page
 * - Resend email functionality
 * 
 * Usage:
 * <app-forgot-password-confirmation 
 *   [email]="userEmail"
 *   (resendEmail)="handleResendEmail($event)">
 * </app-forgot-password-confirmation>
 */
@Component({
  selector: 'app-forgot-password-confirmation',
  templateUrl: './forgot-password-confirmation.component.html',
  styleUrls: ['./forgot-password-confirmation.component.scss'],
  standalone: false
})
export class ForgotPasswordConfirmationComponent {
  /** Email address where reset link was sent */
  @Input() email: string = '';
  
  /** Event emitted when resend email is requested */
  @Output() resendEmail = new EventEmitter<string>();

  resendLoading = false;
  resendSuccess = false;

  /**
   * Handle resend email request
   * TODO: Integrate with actual API service for resending emails
   */
  onResendEmail(): void {
    if (!this.resendLoading && this.email) {
      this.resendLoading = true;
      this.resendSuccess = false;
      this.resendEmail.emit(this.email);
      
      // Simulate success feedback after a delay
      // TODO: Replace with actual API response handling
      setTimeout(() => {
        this.resendLoading = false;
        this.resendSuccess = true;
        
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.resendSuccess = false;
        }, 3000);
      }, 1000);
    }
  }
}
