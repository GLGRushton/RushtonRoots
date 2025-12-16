import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { CreateUserComponent, PasswordStrength } from './create-user.component';
import { SharedModule } from '../../../shared/shared.module';

describe('CreateUserComponent', () => {
  let component: CreateUserComponent;
  let fixture: ComponentFixture<CreateUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CreateUserComponent],
      imports: [
        ReactiveFormsModule,
        NoopAnimationsModule,
        SharedModule
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Form Initialization', () => {
    it('should initialize the form with correct default values', () => {
      expect(component.createUserForm).toBeDefined();
      expect(component.createUserForm.get('email')?.value).toBe('');
      expect(component.createUserForm.get('personId')?.value).toBe('');
      expect(component.createUserForm.get('password')?.value).toBe('');
      expect(component.createUserForm.get('confirmPassword')?.value).toBe('');
      expect(component.createUserForm.get('role')?.value).toBe('');
      expect(component.createUserForm.get('householdId')?.value).toBeNull();
      expect(component.createUserForm.get('sendInvitationEmail')?.value).toBe(true);
    });

    it('should have all required validators configured', () => {
      const emailControl = component.createUserForm.get('email');
      const personIdControl = component.createUserForm.get('personId');
      const passwordControl = component.createUserForm.get('password');
      const confirmPasswordControl = component.createUserForm.get('confirmPassword');

      expect(emailControl?.hasError('required')).toBe(true);
      expect(personIdControl?.hasError('required')).toBe(true);
      expect(passwordControl?.hasError('required')).toBe(true);
      expect(confirmPasswordControl?.hasError('required')).toBe(true);
    });
  });

  describe('Form Validation', () => {
    it('should validate email format', () => {
      const emailControl = component.createUserForm.get('email');
      
      emailControl?.setValue('invalid-email');
      expect(emailControl?.hasError('email')).toBe(true);
      
      emailControl?.setValue('valid@example.com');
      expect(emailControl?.hasError('email')).toBe(false);
    });

    it('should validate password minimum length', () => {
      const passwordControl = component.createUserForm.get('password');
      
      passwordControl?.setValue('short');
      expect(passwordControl?.hasError('minlength')).toBe(true);
      
      passwordControl?.setValue('longenough');
      expect(passwordControl?.hasError('minlength')).toBe(false);
    });

    it('should validate personId is a positive number', () => {
      const personIdControl = component.createUserForm.get('personId');
      
      personIdControl?.setValue(0);
      expect(personIdControl?.hasError('min')).toBe(true);
      
      personIdControl?.setValue(-1);
      expect(personIdControl?.hasError('min')).toBe(true);
      
      personIdControl?.setValue(1);
      expect(personIdControl?.hasError('min')).toBe(false);
    });

    it('should validate passwords match', () => {
      const passwordControl = component.createUserForm.get('password');
      const confirmPasswordControl = component.createUserForm.get('confirmPassword');
      
      passwordControl?.setValue('Password123!');
      confirmPasswordControl?.setValue('DifferentPassword');
      
      expect(component.createUserForm.hasError('passwordMismatch')).toBe(true);
      
      confirmPasswordControl?.setValue('Password123!');
      expect(component.createUserForm.hasError('passwordMismatch')).toBe(false);
    });
  });

  describe('Password Strength Indicator', () => {
    it('should calculate weak password strength correctly', () => {
      const password = 'weak';
      const strength = component['calculatePasswordStrength'](password);
      
      expect(strength.strength).toBe(PasswordStrength.Weak);
      expect(strength.score).toBeLessThan(40);
      expect(strength.feedback.length).toBeGreaterThan(0);
    });

    it('should calculate medium password strength correctly', () => {
      const password = 'Medium123';
      const strength = component['calculatePasswordStrength'](password);
      
      expect(strength.strength).toBe(PasswordStrength.Medium);
      expect(strength.score).toBeGreaterThanOrEqual(40);
      expect(strength.score).toBeLessThan(70);
    });

    it('should calculate strong password strength correctly', () => {
      const password = 'StrongPassword123!';
      const strength = component['calculatePasswordStrength'](password);
      
      expect(strength.strength).toBe(PasswordStrength.Strong);
      expect(strength.score).toBeGreaterThanOrEqual(70);
    });

    it('should update password strength when password changes', () => {
      const passwordControl = component.createUserForm.get('password');
      
      passwordControl?.setValue('weak');
      expect(component.passwordStrength).toBeDefined();
      expect(component.passwordStrength?.strength).toBe(PasswordStrength.Weak);
      
      passwordControl?.setValue('StrongPassword123!');
      expect(component.passwordStrength?.strength).toBe(PasswordStrength.Strong);
    });
  });

  describe('Password Visibility Toggle', () => {
    it('should toggle password visibility', () => {
      expect(component.hidePassword).toBe(true);
      
      component.togglePasswordVisibility();
      expect(component.hidePassword).toBe(false);
      
      component.togglePasswordVisibility();
      expect(component.hidePassword).toBe(true);
    });

    it('should toggle confirm password visibility', () => {
      expect(component.hideConfirmPassword).toBe(true);
      
      component.toggleConfirmPasswordVisibility();
      expect(component.hideConfirmPassword).toBe(false);
      
      component.toggleConfirmPasswordVisibility();
      expect(component.hideConfirmPassword).toBe(true);
    });
  });

  describe('Form Submission', () => {
    it('should emit userCreateSubmit event with form data when form is valid', (done) => {
      // Set up valid form data
      component.createUserForm.patchValue({
        email: 'test@example.com',
        personId: 1,
        password: 'StrongPassword123!',
        confirmPassword: 'StrongPassword123!',
        role: 'FamilyMember',
        householdId: 10,
        sendInvitationEmail: true
      });

      component.userCreateSubmit.subscribe(formData => {
        expect(formData.email).toBe('test@example.com');
        expect(formData.personId).toBe(1);
        expect(formData.password).toBe('StrongPassword123!');
        expect(formData.role).toBe('FamilyMember');
        expect(formData.householdId).toBe(10);
        expect(formData.sendInvitationEmail).toBe(true);
        done();
      });

      component.onSubmit();
    });

    it('should not emit event when form is invalid', () => {
      let eventEmitted = false;
      component.userCreateSubmit.subscribe(() => {
        eventEmitted = true;
      });

      // Leave form empty (invalid)
      component.onSubmit();

      expect(eventEmitted).toBe(false);
    });

    it('should mark all fields as touched when submitting invalid form', () => {
      component.onSubmit();

      expect(component.createUserForm.get('email')?.touched).toBe(true);
      expect(component.createUserForm.get('personId')?.touched).toBe(true);
      expect(component.createUserForm.get('password')?.touched).toBe(true);
      expect(component.createUserForm.get('confirmPassword')?.touched).toBe(true);
    });

    it('should set loading state during submission', () => {
      component.createUserForm.patchValue({
        email: 'test@example.com',
        personId: 1,
        password: 'StrongPassword123!',
        confirmPassword: 'StrongPassword123!',
        role: 'FamilyMember'
      });

      expect(component.isLoading).toBe(false);
      component.onSubmit();
      expect(component.isLoading).toBe(true);
    });
  });

  describe('Error Messages', () => {
    it('should return correct error message for required fields', () => {
      const emailControl = component.createUserForm.get('email');
      emailControl?.markAsTouched();
      emailControl?.setValue('');
      
      expect(component.getErrorMessage('email')).toBe('Email is required');
    });

    it('should return correct error message for invalid email', () => {
      const emailControl = component.createUserForm.get('email');
      emailControl?.markAsTouched();
      emailControl?.setValue('invalid-email');
      
      expect(component.getErrorMessage('email')).toBe('Please enter a valid email address');
    });

    it('should return correct error message for weak password', () => {
      const passwordControl = component.createUserForm.get('password');
      passwordControl?.markAsTouched();
      passwordControl?.setValue('weak');
      
      expect(component.getErrorMessage('password')).toBe('Password is too weak. Please meet the requirements below.');
    });

    it('should return form-level error for password mismatch', () => {
      component.createUserForm.patchValue({
        password: 'Password123!',
        confirmPassword: 'DifferentPassword'
      });
      
      expect(component.getFormError()).toBe('Passwords do not match');
    });
  });

  describe('Cancel Action', () => {
    it('should reset form to initial values when cancelled', () => {
      // Set some values
      component.createUserForm.patchValue({
        email: 'test@example.com',
        personId: 1,
        password: 'password',
        role: 'Admin'
      });

      // Cancel
      component.onCancel();

      // Verify reset
      expect(component.createUserForm.get('email')?.value).toBe('');
      expect(component.createUserForm.get('personId')?.value).toBe('');
      expect(component.createUserForm.get('password')?.value).toBe('');
      expect(component.createUserForm.get('role')?.value).toBe('');
      expect(component.createUserForm.get('sendInvitationEmail')?.value).toBe(true);
    });
  });

  describe('Password Strength Display', () => {
    it('should return correct strength class', () => {
      component.passwordStrength = {
        strength: PasswordStrength.Weak,
        score: 30,
        feedback: [],
        color: '#d32f2f'
      };
      expect(component.getPasswordStrengthClass()).toBe('weak');

      component.passwordStrength = {
        strength: PasswordStrength.Medium,
        score: 50,
        feedback: [],
        color: '#ff9800'
      };
      expect(component.getPasswordStrengthClass()).toBe('medium');

      component.passwordStrength = {
        strength: PasswordStrength.Strong,
        score: 80,
        feedback: [],
        color: '#4caf50'
      };
      expect(component.getPasswordStrengthClass()).toBe('strong');
    });

    it('should return correct strength label', () => {
      component.passwordStrength = {
        strength: PasswordStrength.Weak,
        score: 30,
        feedback: [],
        color: '#d32f2f'
      };
      expect(component.getPasswordStrengthLabel()).toBe('Weak');

      component.passwordStrength = {
        strength: PasswordStrength.Strong,
        score: 80,
        feedback: [],
        color: '#4caf50'
      };
      expect(component.getPasswordStrengthLabel()).toBe('Strong');
    });
  });
});
