import { Injectable } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

/**
 * Interface that components must implement to work with UnsavedChangesGuard
 */
export interface CanComponentDeactivate {
  canDeactivate: () => Observable<boolean> | Promise<boolean> | boolean;
}

/**
 * UnsavedChangesGuard - Warns users before leaving a page with unsaved changes
 * 
 * This guard checks if the component has unsaved changes before allowing navigation away.
 * If there are unsaved changes, the user is prompted to confirm before leaving.
 * 
 * Components must implement the CanComponentDeactivate interface to work with this guard.
 * 
 * Usage in component:
 * ```typescript
 * export class PersonFormComponent implements CanComponentDeactivate {
 *   form: FormGroup;
 * 
 *   canDeactivate(): boolean {
 *     if (this.form.dirty) {
 *       return confirm('You have unsaved changes. Do you really want to leave?');
 *     }
 *     return true;
 *   }
 * }
 * ```
 * 
 * Usage in route:
 * ```typescript
 * {
 *   path: 'edit/:id',
 *   component: PersonFormComponent,
 *   canDeactivate: [UnsavedChangesGuard]
 * }
 * ```
 */
@Injectable({
  providedIn: 'root'
})
export class UnsavedChangesGuard implements CanDeactivate<CanComponentDeactivate> {
  
  canDeactivate(
    component: CanComponentDeactivate,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    
    // If component doesn't implement canDeactivate, allow navigation
    if (!component.canDeactivate) {
      return true;
    }

    // Call the component's canDeactivate method
    return component.canDeactivate();
  }
}
