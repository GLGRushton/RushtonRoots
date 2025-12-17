import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';

/**
 * AuthGuard - Protects routes that require authentication
 * 
 * This guard checks if the user is authenticated before allowing access to a route.
 * If not authenticated, the user is redirected to the login page.
 * 
 * Usage:
 * ```typescript
 * {
 *   path: 'profile',
 *   component: UserProfileComponent,
 *   canActivate: [AuthGuard]
 * }
 * ```
 */
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    
    // Check if user is authenticated
    // In the current implementation, we check if there's user data available
    // This can be replaced with a proper authentication service in the future
    const isAuthenticated = this.checkAuthentication();

    if (isAuthenticated) {
      return true;
    }

    // Store the attempted URL for redirecting after login
    const returnUrl = state.url;

    // Redirect to login page
    // Using window.location for compatibility with ASP.NET Core MVC routing
    window.location.href = `/Account/Login?returnUrl=${encodeURIComponent(returnUrl)}`;
    
    return false;
  }

  /**
   * Check if the user is authenticated
   * 
   * NOTE: This is a simple implementation that checks for authentication markers.
   * In a full SPA implementation, this should be replaced with a proper AuthService
   * that manages authentication state, tokens, and user information.
   */
  private checkAuthentication(): boolean {
    // Check for authentication cookie or token
    // This is a placeholder implementation
    const cookies = document.cookie.split(';');
    const authCookie = cookies.find(c => c.trim().startsWith('.AspNetCore.Identity.Application='));
    
    return !!authCookie;
  }
}
