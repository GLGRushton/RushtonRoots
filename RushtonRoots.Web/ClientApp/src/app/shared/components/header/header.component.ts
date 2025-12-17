import { Component, Input, Output, EventEmitter } from '@angular/core';

export interface UserInfo {
  name: string;
  role: string;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isHouseholdAdmin: boolean;
}

/**
 * HeaderComponent - Main application header with navigation and user menu
 * 
 * Features:
 * - Responsive navigation with mobile hamburger menu
 * - User profile dropdown
 * - Global search
 * - Notification bell icon
 * - Breadcrumb navigation
 * 
 * Usage:
 * <app-header 
 *   [userInfo]="userInfo"
 *   [showSearch]="true"
 *   [showNotifications]="true"
 *   (searchQuery)="onSearch($event)"
 *   (logout)="onLogout()">
 * </app-header>
 */
@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  private _userInfo: UserInfo | null = null;

  @Input() 
  set userinfo(value: string | UserInfo | null) {
    if (!value) {
      this._userInfo = null;
      return;
    }
    
    if (typeof value === 'string') {
      try {
        this._userInfo = JSON.parse(value);
      } catch (e) {
        console.error('Failed to parse userinfo:', e);
        this._userInfo = null;
      }
    } else {
      this._userInfo = value;
    }
  }
  
  get userInfo(): UserInfo | null {
    return this._userInfo;
  }

  @Input() showSearch = true;
  @Input() showNotifications = true;
  @Input() showBreadcrumbs = false;

  @Output() searchQuery = new EventEmitter<string>();
  @Output() logout = new EventEmitter<void>();
  @Output() menuToggle = new EventEmitter<void>();

  isMobileMenuOpen = false;

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
    this.menuToggle.emit();
  }

  onSearch(query: string): void {
    this.searchQuery.emit(query);
  }

  onLogout(): void {
    this.logout.emit();
  }

  navigateTo(url: string): void {
    window.location.href = url;
  }
}
