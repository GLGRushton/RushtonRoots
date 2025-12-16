import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Import shared module for Material components
import { SharedModule } from '../shared/shared.module';

// Import auth components
import { LoginComponent } from './components/login/login.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';

/**
 * AuthModule - Authentication and account management components
 * 
 * This module contains all authentication-related components:
 * - LoginComponent: Modern login form with social login buttons
 * - ForgotPasswordComponent: Password reset request form
 * - ResetPasswordComponent: Password reset form with strength indicator
 * 
 * All components are built with Angular Material and follow the design system.
 */
@NgModule({
  declarations: [
    LoginComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent
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
    ResetPasswordComponent
  ]
})
export class AuthModule { }
