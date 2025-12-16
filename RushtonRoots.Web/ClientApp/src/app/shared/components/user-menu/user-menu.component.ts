import { Component, Input, Output, EventEmitter } from '@angular/core';
import { UserInfo } from '../header/header.component';

/**
 * UserMenuComponent - User profile dropdown menu
 * 
 * Features:
 * - User avatar and name display
 * - Role badge
 * - Profile link
 * - Admin actions (for admins)
 * - Logout action
 * 
 * Usage:
 * <app-user-menu 
 *   [userInfo]="userInfo"
 *   (logout)="onLogout()">
 * </app-user-menu>
 */
@Component({
  selector: 'app-user-menu',
  standalone: false,
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.scss']
})
export class UserMenuComponent {
  @Input() userInfo!: UserInfo;
  @Output() logout = new EventEmitter<void>();

  onLogout(): void {
    // Submit the logout form using the form ID
    const logoutForm = document.getElementById('logoutForm') as HTMLFormElement;
    if (logoutForm) {
      logoutForm.submit();
    } else {
      this.logout.emit();
    }
  }

  navigateTo(url: string): void {
    window.location.href = url;
  }

  getUserInitial(): string {
    return this.userInfo?.name ? this.userInfo.name.charAt(0).toUpperCase() : '?';
  }

  getRoleBadgeColor(): string {
    if (this.userInfo?.isAdmin) {
      return 'warn';
    } else if (this.userInfo?.isHouseholdAdmin) {
      return 'accent';
    }
    return 'primary';
  }
}
