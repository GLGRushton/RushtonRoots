import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ForgotPasswordFormData, AuthActionState } from '../../models/auth.model';

/**
 * ForgotPasswordComponent - Modern forgot password form with Material Design
 * 
 * Features:
 * - Email input with validation
 * - Loading states
 * - Success/error message handling
 * - Link back to login
 * 
 * Usage:
 * <app-forgot-password 
 *   (forgotPasswordSubmit)="handleForgotPassword($event)">
 * </app-forgot-password>
 */
@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
  standalone: false
})
export class ForgotPasswordComponent implements OnInit {
  /** Success message to display */
  @Input() successMessage: string | null = null;
  
  /** Error message to display */
  @Input() errorMessage: string | null = null;
  
  /** Event emitted when form is submitted */
  @Output() forgotPasswordSubmit = new EventEmitter<ForgotPasswordFormData>();

  forgotPasswordForm!: FormGroup;
  authState: AuthActionState = {
    loading: false,
    error: null,
    success: false
  };

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.createForm();
  }

  /**
   * Initialize the forgot password form with validators
   */
  private createForm(): void {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.forgotPasswordForm.valid) {
      this.authState.loading = true;
      this.authState.error = null;
      this.authState.success = false;
      
      const formData: ForgotPasswordFormData = this.forgotPasswordForm.value;
      this.forgotPasswordSubmit.emit(formData);
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.forgotPasswordForm.controls).forEach(key => {
        this.forgotPasswordForm.get(key)?.markAsTouched();
      });
    }
  }

  /**
   * Get error message for email field
   */
  getErrorMessage(): string {
    const email = this.forgotPasswordForm.get('email');
    
    if (email?.hasError('required')) {
      return 'Email is required';
    }
    
    if (email?.hasError('email')) {
      return 'Please enter a valid email address';
    }
    
    return '';
  }

  /**
   * Check if email field has an error
   */
  hasError(): boolean {
    const email = this.forgotPasswordForm.get('email');
    return !!(email && email.invalid && (email.dirty || email.touched));
  }
}
