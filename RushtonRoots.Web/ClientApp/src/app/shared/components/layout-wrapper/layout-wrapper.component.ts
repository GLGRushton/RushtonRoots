import { Component, Input, Output, EventEmitter } from '@angular/core';
import { UserInfo } from '../header/header.component';
import { BreadcrumbItem } from '../breadcrumb/breadcrumb.component';

/**
 * LayoutWrapperComponent - Main layout orchestrator component
 * 
 * Features:
 * - Integrates HeaderComponent with navigation and user menu
 * - Integrates BreadcrumbComponent for contextual navigation
 * - Content projection for main content area
 * - Integrates FooterComponent
 * - Manages responsive layout state
 * - Handles authentication state display
 * 
 * Usage in _Layout.cshtml:
 * <app-layout-wrapper
 *   [userInfo]="userInfoJson"
 *   [breadcrumbItems]="breadcrumbsJson"
 *   [showSearch]="true"
 *   [showNotifications]="true"
 *   [showBreadcrumbs]="true">
 *   <!-- Main content via @RenderBody() -->
 * </app-layout-wrapper>
 */
@Component({
  selector: 'app-layout-wrapper',
  standalone: false,
  templateUrl: './layout-wrapper.component.html',
  styleUrls: ['./layout-wrapper.component.scss']
})
export class LayoutWrapperComponent {
  // User information for header and authentication state
  private _userInfo: UserInfo = {
    name: '',
    role: '',
    isAuthenticated: false,
    isAdmin: false,
    isHouseholdAdmin: false
  };

  @Input()
  set userinfo(value: string | UserInfo) {
    if (typeof value === 'string') {
      try {
        this._userInfo = JSON.parse(value);
      } catch (e) {
        console.error('Failed to parse userinfo in LayoutWrapperComponent:', e);
      }
    } else {
      this._userInfo = value;
    }
  }

  get userInfo(): UserInfo {
    return this._userInfo;
  }

  // Breadcrumb navigation items
  private _breadcrumbItems: BreadcrumbItem[] = [];

  @Input()
  set breadcrumbitems(value: string | BreadcrumbItem[]) {
    if (typeof value === 'string') {
      try {
        this._breadcrumbItems = JSON.parse(value);
      } catch (e) {
        console.error('Failed to parse breadcrumbItems in LayoutWrapperComponent:', e);
        this._breadcrumbItems = [];
      }
    } else {
      this._breadcrumbItems = value || [];
    }
  }

  get breadcrumbItems(): BreadcrumbItem[] {
    return this._breadcrumbItems;
  }

  // Header configuration
  @Input() showsearch = true;
  @Input() shownotifications = true;
  @Input() showbreadcrumbs = false; // Default to false, can be enabled per page

  // Events
  @Output() searchQuery = new EventEmitter<string>();
  @Output() logout = new EventEmitter<void>();

  // Mobile menu state
  isMobileMenuOpen = false;

  /**
   * Handle search query from header
   */
  onSearch(query: string): void {
    this.searchQuery.emit(query);
  }

  /**
   * Handle logout from header
   */
  onLogout(): void {
    // Trigger server-side logout form submission
    const logoutForm = document.getElementById('logoutForm') as HTMLFormElement;
    if (logoutForm) {
      logoutForm.submit();
    }
    this.logout.emit();
  }

  /**
   * Handle mobile menu toggle
   */
  onMenuToggle(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  /**
   * Check if breadcrumbs should be displayed
   */
  get shouldShowBreadcrumbs(): boolean {
    return this.showbreadcrumbs && this.breadcrumbItems.length > 0;
  }
}
