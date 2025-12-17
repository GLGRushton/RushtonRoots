import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';

/**
 * RoleGuard - Protects routes that require specific user roles
 * 
 * This guard checks if the user has the required role(s) before allowing access to a route.
 * If the user doesn't have the required role, they are redirected to an access denied page.
 * 
 * Usage:
 * ```typescript
 * {
 *   path: 'admin',
 *   component: AdminComponent,
 *   canActivate: [RoleGuard],
 *   data: { roles: ['Admin'] }
 * }
 * ```
 * 
 * Multiple roles (user must have at least one):
 * ```typescript
 * {
 *   path: 'manage-household',
 *   component: ManageHouseholdComponent,
 *   canActivate: [RoleGuard],
 *   data: { roles: ['Admin', 'HouseholdAdmin'] }
 * }
 * ```
 */
@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    
    // Get required roles from route data
    const requiredRoles = route.data['roles'] as string[] | undefined;

    if (!requiredRoles || requiredRoles.length === 0) {
      // No roles required, allow access
      return true;
    }

    // Check if user has required role
    const hasRole = this.checkUserRole(requiredRoles);

    if (hasRole) {
      return true;
    }

    // User doesn't have required role - redirect to access denied
    // Using window.location for compatibility with ASP.NET Core MVC routing
    window.location.href = '/Account/AccessDenied';
    
    return false;
  }

  /**
   * Check if the user has any of the required roles
   * 
   * NOTE: This is a simple implementation that checks role information from DOM or cookies.
   * In a full SPA implementation, this should be replaced with a proper AuthService
   * that manages user roles and permissions.
   */
  private checkUserRole(requiredRoles: string[]): boolean {
    // Admin has access to everything
    if (this.isAdmin()) {
      return true;
    }

    // Try to get user role from meta tag (if set by server)
    const roleMetaTag = document.querySelector('meta[name="user-role"]');
    if (roleMetaTag) {
      const userRole = roleMetaTag.getAttribute('content');
      if (userRole && requiredRoles.includes(userRole)) {
        return true;
      }
    }

    // Try to get from data attribute on body (alternative method)
    const bodyRole = document.body.getAttribute('data-user-role');
    if (bodyRole && requiredRoles.includes(bodyRole)) {
      return true;
    }

    // HouseholdAdmin role check
    if (this.isHouseholdAdmin() && requiredRoles.includes('HouseholdAdmin')) {
      return true;
    }

    return false;
  }

  /**
   * Check if user is an Admin
   */
  private isAdmin(): boolean {
    const adminMetaTag = document.querySelector('meta[name="user-is-admin"]');
    return adminMetaTag?.getAttribute('content') === 'true';
  }

  /**
   * Check if user is a HouseholdAdmin
   */
  private isHouseholdAdmin(): boolean {
    const householdAdminMetaTag = document.querySelector('meta[name="user-is-household-admin"]');
    return householdAdminMetaTag?.getAttribute('content') === 'true';
  }
}
