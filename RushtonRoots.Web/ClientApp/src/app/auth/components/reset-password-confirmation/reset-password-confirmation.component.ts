import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';

/**
 * ResetPasswordConfirmationComponent - Confirmation screen after successful password reset
 * 
 * Features:
 * - Password reset success message
 * - Auto-redirect to login after 5 seconds
 * - Manual login link
 * - Countdown timer display
 * 
 * Usage:
 * <app-reset-password-confirmation></app-reset-password-confirmation>
 */
@Component({
  selector: 'app-reset-password-confirmation',
  templateUrl: './reset-password-confirmation.component.html',
  styleUrls: ['./reset-password-confirmation.component.scss'],
  standalone: false
})
export class ResetPasswordConfirmationComponent implements OnInit, OnDestroy {
  countdown = 5;
  private countdownInterval?: number;

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.startCountdown();
  }

  ngOnDestroy(): void {
    this.clearCountdown();
  }

  /**
   * Start countdown timer for auto-redirect
   */
  private startCountdown(): void {
    this.countdownInterval = window.setInterval(() => {
      this.countdown--;
      
      if (this.countdown <= 0) {
        this.clearCountdown();
        this.redirectToLogin();
      }
    }, 1000);
  }

  /**
   * Clear countdown timer
   */
  private clearCountdown(): void {
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
      this.countdownInterval = undefined;
    }
  }

  /**
   * Navigate to login page
   */
  redirectToLogin(): void {
    this.router.navigate(['/Account/Login']);
  }

  /**
   * Cancel auto-redirect and go to login manually
   */
  goToLoginNow(): void {
    this.clearCountdown();
    this.redirectToLogin();
  }
}
