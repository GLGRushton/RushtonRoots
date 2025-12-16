import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Import shared module for Material components
import { SharedModule } from '../shared/shared.module';

// Import auth components
import { LoginComponent } from './components/login/login.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ForgotPasswordConfirmationComponent } from './components/forgot-password-confirmation/forgot-password-confirmation.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ResetPasswordConfirmationComponent } from './components/reset-password-confirmation/reset-password-confirmation.component';
import { ConfirmEmailComponent } from './components/confirm-email/confirm-email.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { NotificationPreferencesComponent } from './components/notification-preferences/notification-preferences.component';
import { PrivacySettingsComponent } from './components/privacy-settings/privacy-settings.component';
import { ConnectedAccountsComponent } from './components/connected-accounts/connected-accounts.component';
import { AccountDeletionComponent } from './components/account-deletion/account-deletion.component';
import { CreateUserComponent } from './components/create-user/create-user.component';
import { AccessDeniedComponent } from './components/access-denied/access-denied.component';

// Import auth directives
import { AdminOnlyDirective, RoleGuardDirective } from './directives/admin-only.directive';

/**
 * AuthModule - Authentication and account management components
 * 
 * This module contains all authentication-related components:
 * - LoginComponent: Modern login form with social login buttons
 * - ForgotPasswordComponent: Password reset request form
 * - ForgotPasswordConfirmationComponent: Confirmation screen after password reset request
 * - ResetPasswordComponent: Password reset form with strength indicator
 * - ResetPasswordConfirmationComponent: Confirmation screen after successful password reset
 * - ConfirmEmailComponent: Email verification confirmation screen
 * - UserProfileComponent: Comprehensive user profile and settings management
 * - NotificationPreferencesComponent: Notification settings
 * - PrivacySettingsComponent: Privacy settings
 * - ConnectedAccountsComponent: Connected social accounts management
 * - AccountDeletionComponent: Account deletion flow
 * - CreateUserComponent: Admin-only user creation form
 * - AccessDeniedComponent: Access denied page with clear messaging and request access option
 * 
 * Directives:
 * - AdminOnlyDirective: Show/hide content for admin users
 * - RoleGuardDirective: Flexible role-based content visibility
 * 
 * All components are built with Angular Material and follow the design system.
 */
@NgModule({
  declarations: [
    LoginComponent,
    ForgotPasswordComponent,
    ForgotPasswordConfirmationComponent,
    ResetPasswordComponent,
    ResetPasswordConfirmationComponent,
    ConfirmEmailComponent,
    UserProfileComponent,
    NotificationPreferencesComponent,
    PrivacySettingsComponent,
    ConnectedAccountsComponent,
    AccountDeletionComponent,
    CreateUserComponent,
    AccessDeniedComponent,
    AdminOnlyDirective,
    RoleGuardDirective
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    SharedModule
  ],
  exports: [
    LoginComponent,
    ForgotPasswordComponent,
    ForgotPasswordConfirmationComponent,
    ResetPasswordComponent,
    ResetPasswordConfirmationComponent,
    ConfirmEmailComponent,
    UserProfileComponent,
    NotificationPreferencesComponent,
    PrivacySettingsComponent,
    ConnectedAccountsComponent,
    AccountDeletionComponent,
    CreateUserComponent,
    AccessDeniedComponent,
    AdminOnlyDirective,
    RoleGuardDirective
  ]
})
export class AuthModule { }
