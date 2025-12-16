import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

/**
 * AdminOnlyDirective - Structural directive to show/hide content based on user's admin role
 * 
 * This directive conditionally displays content only for users with admin privileges.
 * It can check for specific roles like Admin or HouseholdAdmin.
 * 
 * Usage:
 * <div *appAdminOnly>
 *   This content is only visible to admins
 * </div>
 * 
 * <div *appAdminOnly="'HouseholdAdmin'">
 *   This content is only visible to household admins
 * </div>
 * 
 * <div *appAdminOnly="['Admin', 'HouseholdAdmin']">
 *   This content is visible to both admin types
 * </div>
 */
@Directive({
  selector: '[appAdminOnly]',
  standalone: false
})
export class AdminOnlyDirective implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private hasView = false;
  private allowedRoles: string[] = ['Admin', 'HouseholdAdmin'];

  @Input() set appAdminOnly(roles: string | string[] | undefined) {
    if (roles) {
      this.allowedRoles = Array.isArray(roles) ? roles : [roles];
    }
    this.updateView();
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}

  ngOnInit(): void {
    this.updateView();
    
    // In a real implementation, you would subscribe to an auth service
    // to reactively update the view when user roles change
    // Example:
    // this.authService.userRoles$
    //   .pipe(takeUntil(this.destroy$))
    //   .subscribe(() => this.updateView());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private updateView(): void {
    const hasRequiredRole = this.checkUserRole();

    if (hasRequiredRole && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!hasRequiredRole && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }

  /**
   * Check if the current user has one of the required roles
   * In a real implementation, this would check against actual user roles
   */
  private checkUserRole(): boolean {
    // Placeholder implementation
    // In production, replace with actual role checking logic:
    // return this.authService.hasAnyRole(this.allowedRoles);
    
    // For now, we'll assume the user is authorized
    // This should be replaced with actual authentication service
    return true;
  }
}

/**
 * RoleGuardDirective - More flexible role-based directive
 * 
 * This directive can show content based on specific role requirements
 * and supports both "any" and "all" matching strategies.
 * 
 * Usage:
 * <div *appRoleGuard="'Admin'">Admin only content</div>
 * <div *appRoleGuard="['Admin', 'HouseholdAdmin']; strategy: 'any'">
 *   Content for Admin OR HouseholdAdmin
 * </div>
 * <div *appRoleGuard="['Admin', 'Editor']; strategy: 'all'">
 *   Content for users with BOTH Admin AND Editor roles
 * </div>
 */
@Directive({
  selector: '[appRoleGuard]',
  standalone: false
})
export class RoleGuardDirective implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private hasView = false;
  private requiredRoles: string[] = [];
  private matchStrategy: 'any' | 'all' = 'any';

  @Input() set appRoleGuard(roles: string | string[]) {
    this.requiredRoles = Array.isArray(roles) ? roles : [roles];
    this.updateView();
  }

  @Input() set appRoleGuardStrategy(strategy: 'any' | 'all') {
    this.matchStrategy = strategy;
    this.updateView();
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}

  ngOnInit(): void {
    this.updateView();
    
    // In a real implementation, subscribe to auth service
    // this.authService.userRoles$
    //   .pipe(takeUntil(this.destroy$))
    //   .subscribe(() => this.updateView());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private updateView(): void {
    const isAuthorized = this.checkAuthorization();

    if (isAuthorized && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!isAuthorized && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }

  /**
   * Check if user is authorized based on role requirements and strategy
   */
  private checkAuthorization(): boolean {
    if (this.requiredRoles.length === 0) {
      return false;
    }

    // Placeholder implementation
    // In production, replace with actual role checking:
    // const userRoles = this.authService.getUserRoles();
    const userRoles = ['Admin']; // Placeholder

    if (this.matchStrategy === 'all') {
      return this.requiredRoles.every(role => userRoles.includes(role));
    } else {
      return this.requiredRoles.some(role => userRoles.includes(role));
    }
  }
}
