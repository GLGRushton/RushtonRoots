import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LoginFormData, SocialLoginProvider, SOCIAL_LOGIN_PROVIDERS, AuthActionState } from '../../models/auth.model';

/**
 * LoginComponent - Modern login form with Material Design
 * 
 * Features:
 * - Email and password input with validation
 * - "Remember Me" toggle
 * - Password visibility toggle
 * - Social login buttons (for future use)
 * - Loading states for auth actions
 * - Error handling and display
 * 
 * Usage:
 * <app-login 
 *   [returnUrl]="returnUrl" 
 *   (loginSubmit)="handleLogin($event)"
 *   (socialLogin)="handleSocialLogin($event)">
 * </app-login>
 */
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: false
})
export class LoginComponent implements OnInit {
  /** Return URL after successful login */
  @Input() returnUrl: string = '/';
  
  /** Error message to display */
  @Input() errorMessage: string | null = null;
  
  /** Event emitted when login form is submitted */
  @Output() loginSubmit = new EventEmitter<LoginFormData>();
  
  /** Event emitted when social login is clicked */
  @Output() socialLogin = new EventEmitter<string>();

  loginForm!: FormGroup;
  hidePassword = true;
  socialProviders = SOCIAL_LOGIN_PROVIDERS;
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
   * Initialize the login form with validators
   */
  private createForm(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  /**
   * Toggle password visibility
   */
  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.loginForm.valid) {
      this.authState.loading = true;
      this.authState.error = null;
      
      const formData: LoginFormData = this.loginForm.value;
      this.loginSubmit.emit(formData);
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.loginForm.controls).forEach(key => {
        this.loginForm.get(key)?.markAsTouched();
      });
    }
  }

  /**
   * Handle social login button click
   */
  onSocialLogin(providerId: string): void {
    if (!this.authState.loading) {
      this.socialLogin.emit(providerId);
    }
  }

  /**
   * Get error message for a form field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    
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
    
    return '';
  }

  /**
   * Check if a form field has an error and should display it
   */
  hasError(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
