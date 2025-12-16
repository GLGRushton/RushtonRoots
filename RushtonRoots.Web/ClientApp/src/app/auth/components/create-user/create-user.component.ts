import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors, AsyncValidatorFn } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { map, catchError, debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';

/**
 * CreateUserComponent - Admin-only user creation form with Material Design
 * 
 * Features:
 * - Admin-only user creation form
 * - Role selection dropdown (Admin, HouseholdAdmin, FamilyMember)
 * - Household assignment field
 * - Email invitation option
 * - Person linkage (PersonId)
 * - Form validation with reactive forms
 * - Email uniqueness check (async validator)
 * - Password requirements (min 8 chars)
 * - Password confirmation match
 * - Role-based field visibility
 * 
 * Usage:
 * <app-create-user 
 *   (userCreateSubmit)="handleUserCreate($event)">
 * </app-create-user>
 */
@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.scss'],
  standalone: false
})
export class CreateUserComponent implements OnInit {
  /** Success message to display */
  @Input() successMessage: string | null = null;
  
  /** Error message to display */
  @Input() errorMessage: string | null = null;
  
  /** Event emitted when user creation form is submitted */
  @Output() userCreateSubmit = new EventEmitter<CreateUserFormData>();

  createUserForm!: FormGroup;
  hidePassword = true;
  hideConfirmPassword = true;
  isLoading = false;
  
  // Role options for the dropdown
  roleOptions = [
    { value: '', label: 'Select Role (Optional)' },
    { value: 'FamilyMember', label: 'Family Member' },
    { value: 'HouseholdAdmin', label: 'Household Admin' },
    { value: 'Admin', label: 'Administrator' }
  ];

  // Password strength tracking
  passwordStrength: PasswordStrengthResult | null = null;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.createForm();
  }

  /**
   * Initialize the create user form with validators
   */
  private createForm(): void {
    this.createUserForm = this.fb.group({
      email: ['', 
        [Validators.required, Validators.email],
        [this.emailUniqueValidator()]
      ],
      personId: ['', [Validators.required, Validators.min(1)]],
      password: ['', 
        [Validators.required, Validators.minLength(8), this.passwordStrengthValidator.bind(this)]
      ],
      confirmPassword: ['', [Validators.required]],
      role: [''],
      householdId: [null],
      sendInvitationEmail: [true]
    }, {
      validators: this.passwordMatchValidator
    });

    // Update password strength when password changes
    this.createUserForm.get('password')?.valueChanges.subscribe(password => {
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
   * Async validator to check email uniqueness
   * In a real implementation, this would call an API
   */
  private emailUniqueValidator(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      if (!control.value) {
        return of(null);
      }

      // Simulate API call to check email uniqueness
      // In production, replace with actual HTTP call to backend
      return of(control.value).pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap(email => {
          // Placeholder - replace with actual API call
          // return this.accountService.checkEmailExists(email);
          return of(false); // Assume email is unique for now
        }),
        map(emailExists => emailExists ? { emailTaken: true } : null),
        catchError(() => of(null))
      );
    };
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
      score += 25;
    } else {
      feedback.push('At least 8 characters');
    }

    if (password.length >= 12) {
      score += 15;
    }

    // Uppercase check
    if (/[A-Z]/.test(password)) {
      score += 20;
    } else {
      feedback.push('At least one uppercase letter');
    }

    // Lowercase check
    if (/[a-z]/.test(password)) {
      score += 20;
    } else {
      feedback.push('At least one lowercase letter');
    }

    // Number check
    if (/[0-9]/.test(password)) {
      score += 20;
    } else {
      feedback.push('At least one number');
    }

    // Special character check
    if (/[^A-Za-z0-9]/.test(password)) {
      score += 20;
    } else {
      feedback.push('At least one special character');
    }

    // Determine strength level
    let strength: PasswordStrength;
    let color: string;

    if (score < 40) {
      strength = PasswordStrength.Weak;
      color = '#d32f2f'; // Red
    } else if (score < 70) {
      strength = PasswordStrength.Medium;
      color = '#ff9800'; // Orange
    } else {
      strength = PasswordStrength.Strong;
      color = '#4caf50'; // Green
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
   * Check if a form control has an error
   */
  hasError(controlName: string, errorType?: string): boolean {
    const control = this.createUserForm.get(controlName);
    if (!control) return false;
    
    if (errorType) {
      return control.hasError(errorType) && (control.dirty || control.touched);
    }
    return control.invalid && (control.dirty || control.touched);
  }

  /**
   * Get error message for a form control
   */
  getErrorMessage(controlName: string): string {
    const control = this.createUserForm.get(controlName);
    if (!control || !control.errors) return '';

    if (control.hasError('required')) {
      return `${this.getFieldLabel(controlName)} is required`;
    }
    if (control.hasError('email')) {
      return 'Please enter a valid email address';
    }
    if (control.hasError('emailTaken')) {
      return 'This email is already in use';
    }
    if (control.hasError('minlength')) {
      const minLength = control.errors['minlength'].requiredLength;
      return `Must be at least ${minLength} characters`;
    }
    if (control.hasError('min')) {
      return 'Please select a valid person';
    }
    if (control.hasError('weakPassword')) {
      return 'Password is too weak. Please meet the requirements below.';
    }

    return 'Invalid value';
  }

  /**
   * Get form-level error message
   */
  getFormError(): string | null {
    if (this.createUserForm.hasError('passwordMismatch')) {
      return 'Passwords do not match';
    }
    return null;
  }

  /**
   * Get user-friendly field label
   */
  private getFieldLabel(controlName: string): string {
    const labels: { [key: string]: string } = {
      email: 'Email',
      personId: 'Person ID',
      password: 'Password',
      confirmPassword: 'Confirm Password',
      role: 'Role',
      householdId: 'Household ID'
    };
    return labels[controlName] || controlName;
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.createUserForm.valid) {
      this.isLoading = true;
      const formData: CreateUserFormData = this.createUserForm.value;
      this.userCreateSubmit.emit(formData);
      
      // Reset loading state after a delay (the parent component should handle this)
      setTimeout(() => {
        this.isLoading = false;
      }, 3000);
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.createUserForm.controls).forEach(key => {
        this.createUserForm.get(key)?.markAsTouched();
      });
    }
  }

  /**
   * Navigate to home page (emit event for parent to handle)
   */
  onCancel(): void {
    // In a real app, this would be handled by routing
    // For now, we'll just reset the form
    this.createUserForm.reset({
      email: '',
      personId: '',
      password: '',
      confirmPassword: '',
      role: '',
      householdId: null,
      sendInvitationEmail: true
    });
  }

  /**
   * Get password strength class for progress bar
   */
  getPasswordStrengthClass(): string {
    if (!this.passwordStrength) return '';
    
    switch (this.passwordStrength.strength) {
      case PasswordStrength.Weak:
        return 'weak';
      case PasswordStrength.Medium:
        return 'medium';
      case PasswordStrength.Strong:
        return 'strong';
      default:
        return '';
    }
  }

  /**
   * Get password strength label
   */
  getPasswordStrengthLabel(): string {
    if (!this.passwordStrength) return '';
    
    switch (this.passwordStrength.strength) {
      case PasswordStrength.Weak:
        return 'Weak';
      case PasswordStrength.Medium:
        return 'Medium';
      case PasswordStrength.Strong:
        return 'Strong';
      default:
        return '';
    }
  }
}

// Type definitions
export interface CreateUserFormData {
  email: string;
  personId: number;
  password: string;
  confirmPassword: string;
  role?: string;
  householdId?: number | null;
  sendInvitationEmail: boolean;
}

export enum PasswordStrength {
  Weak = 'weak',
  Medium = 'medium',
  Strong = 'strong'
}

export interface PasswordStrengthResult {
  strength: PasswordStrength;
  score: number;
  feedback: string[];
  color: string;
}
