/**
 * Authentication Models
 * TypeScript interfaces and types for authentication components
 */

/**
 * Login form data
 */
export interface LoginFormData {
  email: string;
  password: string;
  rememberMe: boolean;
}

/**
 * Forgot password form data
 */
export interface ForgotPasswordFormData {
  email: string;
}

/**
 * Reset password form data
 */
export interface ResetPasswordFormData {
  email: string;
  password: string;
  confirmPassword: string;
  code: string;
}

/**
 * Social login provider configuration
 */
export interface SocialLoginProvider {
  id: string;
  name: string;
  icon: string;
  color: string;
  enabled: boolean;
}

/**
 * Available social login providers
 */
export const SOCIAL_LOGIN_PROVIDERS: SocialLoginProvider[] = [
  {
    id: 'google',
    name: 'Google',
    icon: 'google',
    color: '#4285F4',
    enabled: false // For future use
  },
  {
    id: 'facebook',
    name: 'Facebook',
    icon: 'facebook',
    color: '#1877F2',
    enabled: false // For future use
  },
  {
    id: 'microsoft',
    name: 'Microsoft',
    icon: 'microsoft',
    color: '#00A4EF',
    enabled: false // For future use
  }
];

/**
 * Password strength levels
 */
export enum PasswordStrength {
  Weak = 'weak',
  Fair = 'fair',
  Good = 'good',
  Strong = 'strong'
}

/**
 * Password strength result
 */
export interface PasswordStrengthResult {
  strength: PasswordStrength;
  score: number; // 0-100
  feedback: string[];
  color: string;
}

/**
 * Auth action state for loading indicators
 */
export interface AuthActionState {
  loading: boolean;
  error: string | null;
  success: boolean;
}
