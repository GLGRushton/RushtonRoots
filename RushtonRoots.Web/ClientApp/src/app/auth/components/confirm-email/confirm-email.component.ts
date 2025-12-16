import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';

/**
 * Email confirmation status
 */
export interface EmailConfirmationStatus {
  success: boolean;
  message: string;
  tokenValid?: boolean;
}

/**
 * ConfirmEmailComponent - Email verification confirmation screen
 * 
 * Features:
 * - Email verification status display
 * - Success/error states
 * - Token validation feedback
 * - Resend confirmation email option
 * 
 * Usage:
 * <app-confirm-email 
 *   [status]="confirmationStatus"
 *   [email]="userEmail"
 *   (resendEmail)="handleResendConfirmation($event)">
 * </app-confirm-email>
 */
@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss'],
  standalone: false
})
export class ConfirmEmailComponent implements OnInit {
  /** Confirmation status from server */
  @Input() status: EmailConfirmationStatus | null = null;
  
  /** User's email address */
  @Input() email: string = '';
  
  /** Event emitted when resend confirmation email is requested */
  @Output() resendEmail = new EventEmitter<string>();

  resendLoading = false;
  resendSuccess = false;
  isSuccess = false;
  isError = false;
  isTokenInvalid = false;

  ngOnInit(): void {
    this.updateStatus();
  }

  ngOnChanges(): void {
    this.updateStatus();
  }

  /**
   * Update component state based on status
   */
  private updateStatus(): void {
    if (this.status) {
      this.isSuccess = this.status.success;
      this.isError = !this.status.success;
      this.isTokenInvalid = this.status.tokenValid === false;
    }
  }

  /**
   * Handle resend confirmation email request
   */
  onResendEmail(): void {
    if (!this.resendLoading && this.email) {
      this.resendLoading = true;
      this.resendSuccess = false;
      this.resendEmail.emit(this.email);
      
      // Simulate success feedback after a delay
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

  /**
   * Get icon based on status
   */
  get statusIcon(): string {
    if (this.isSuccess) return 'check_circle';
    if (this.isTokenInvalid) return 'error_outline';
    return 'warning';
  }

  /**
   * Get icon color class based on status
   */
  get iconColorClass(): string {
    if (this.isSuccess) return 'success-icon';
    if (this.isTokenInvalid) return 'error-icon';
    return 'warning-icon';
  }
}
