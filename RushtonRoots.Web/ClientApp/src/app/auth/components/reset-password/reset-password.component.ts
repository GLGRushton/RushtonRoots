import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { ResetPasswordFormData, AuthActionState, PasswordStrength, PasswordStrengthResult } from '../../models/auth.model';

/**
 * ResetPasswordComponent - Modern password reset form with Material Design
 * 
 * Features:
 * - Email and password input with validation
 * - Password confirmation matching
 * - Password visibility toggles
 * - Password strength indicator
 * - Loading states
 * - Success/error message handling
 * 
 * Usage:
 * <app-reset-password 
 *   [code]="resetCode"
 *   (resetPasswordSubmit)="handleResetPassword($event)">
 * </app-reset-password>
 */
@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
  standalone: false
})
export class ResetPasswordComponent implements OnInit {
  /** Reset code from email link */
  @Input() code: string = '';
  
  /** Error message to display */
  @Input() errorMessage: string | null = null;
  
  /** Event emitted when form is submitted */
  @Output() resetPasswordSubmit = new EventEmitter<ResetPasswordFormData>();

  resetPasswordForm!: FormGroup;
  hidePassword = true;
  hideConfirmPassword = true;
  passwordStrength: PasswordStrengthResult | null = null;
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
   * Initialize the reset password form with validators
   */
  private createForm(): void {
    this.resetPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8), this.passwordStrengthValidator.bind(this)]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });

    // Update password strength when password changes
    this.resetPasswordForm.get('password')?.valueChanges.subscribe(password => {
      this.passwordStrength = this.calculatePasswordStrength(password);
    });
  }

  /**
   * Custom validator for password strength
   */
  private passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.value;
    if (!password) {
      return null;
    }

    const strength = this.calculatePasswordStrength(password);
    if (strength.score < 40) {
      return { weakPassword: true };
    }

    return null;
  }

  /**
   * Custom validator to check if passwords match
   */
  private passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;

    if (password && confirmPassword && password !== confirmPassword) {
      return { passwordMismatch: true };
    }

    return null;
  }

  /**
   * Calculate password strength
   */
  private calculatePasswordStrength(password: string): PasswordStrengthResult {
    let score = 0;
    const feedback: string[] = [];

    if (!password) {
      return {
        strength: PasswordStrength.Weak,
        score: 0,
        feedback: ['Password is required'],
        color: '#d32f2f'
      };
    }

    // Length check
    if (password.length >= 8) {
      score += 20;
    } else {
      feedback.push('Use at least 8 characters');
    }

    if (password.length >= 12) {
      score += 10;
    }

    // Uppercase check
    if (/[A-Z]/.test(password)) {
      score += 20;
    } else {
      feedback.push('Include uppercase letters');
    }

    // Lowercase check
    if (/[a-z]/.test(password)) {
      score += 20;
    } else {
      feedback.push('Include lowercase letters');
    }

    // Number check
    if (/\d/.test(password)) {
      score += 20;
    } else {
      feedback.push('Include numbers');
    }

    // Special character check
    if (/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
      score += 10;
    } else {
      feedback.push('Include special characters');
    }

    // Determine strength level
    let strength: PasswordStrength;
    let color: string;

    if (score >= 80) {
      strength = PasswordStrength.Strong;
      color = '#4caf50';
    } else if (score >= 60) {
      strength = PasswordStrength.Good;
      color = '#66bb6a';
    } else if (score >= 40) {
      strength = PasswordStrength.Fair;
      color = '#ffa726';
    } else {
      strength = PasswordStrength.Weak;
      color = '#d32f2f';
    }

    return { strength, score, feedback, color };
  }

  /**
   * Toggle password visibility
   */
  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  /**
   * Toggle confirm password visibility
   */
  toggleConfirmPasswordVisibility(): void {
    this.hideConfirmPassword = !this.hideConfirmPassword;
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.resetPasswordForm.valid) {
      this.authState.loading = true;
      this.authState.error = null;
      
      const formData: ResetPasswordFormData = {
        ...this.resetPasswordForm.value,
        code: this.code
      };
      this.resetPasswordSubmit.emit(formData);
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.resetPasswordForm.controls).forEach(key => {
        this.resetPasswordForm.get(key)?.markAsTouched();
      });
    }
  }

  /**
   * Get error message for a form field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.resetPasswordForm.get(fieldName);
    
    if (field?.hasError('required')) {
      return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
    }
    
    if (field?.hasError('email')) {
      return 'Please enter a valid email address';
    }
    
    if (field?.hasError('minlength')) {
      const minLength = field.errors?.['minlength'].requiredLength;
      return `Password must be at least ${minLength} characters`;
    }

    if (field?.hasError('weakPassword')) {
      return 'Password is too weak';
    }
    
    if (fieldName === 'confirmPassword' && this.resetPasswordForm.hasError('passwordMismatch')) {
      return 'Passwords do not match';
    }
    
    return '';
  }

  /**
   * Check if a form field has an error and should display it
   */
  hasError(fieldName: string): boolean {
    const field = this.resetPasswordForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched)) || 
           (fieldName === 'confirmPassword' && this.resetPasswordForm.hasError('passwordMismatch'));
  }

  /**
   * Get password strength label
   */
  getStrengthLabel(): string {
    if (!this.passwordStrength) return '';
    
    switch (this.passwordStrength.strength) {
      case PasswordStrength.Weak: return 'Weak';
      case PasswordStrength.Fair: return 'Fair';
      case PasswordStrength.Good: return 'Good';
      case PasswordStrength.Strong: return 'Strong';
      default: return '';
    }
  }
}
