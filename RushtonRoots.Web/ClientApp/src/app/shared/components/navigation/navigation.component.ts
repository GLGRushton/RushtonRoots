import { Component, Input, HostListener } from '@angular/core';

export interface NavigationItem {
  label: string;
  url?: string;
  icon?: string;
  requireAuth?: boolean;
  requireRole?: string[];
  children?: NavigationItem[];
  divider?: boolean;
}

export interface UserInfo {
  name: string;
  role: string;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isHouseholdAdmin: boolean;
}

/**
 * NavigationComponent - Responsive navigation menu
 * 
 * Features:
 * - Desktop horizontal navigation with dropdowns
 * - Mobile vertical navigation
 * - Role-based menu items
 * - Active route highlighting
 * - Keyboard navigation support
 * 
 * Usage:
 * <app-navigation 
 *   [userInfo]="userInfo"
 *   [isMobile]="false">
 * </app-navigation>
 */
@Component({
  selector: 'app-navigation',
  standalone: false,
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss']
})
export class NavigationComponent {
  private _userInfo: UserInfo = {
    name: '',
    role: '',
    isAuthenticated: false,
    isAdmin: false,
    isHouseholdAdmin: false
  };

  @Input() 
  set userInfo(value: UserInfo) {
    this._userInfo = value;
  }
  
  get userInfo(): UserInfo {
    return this._userInfo;
  }

  @Input() 
  set isAuthenticated(value: boolean) {
    this._userInfo.isAuthenticated = value;
  }
  
  get isAuthenticated(): boolean {
    return this._userInfo.isAuthenticated;
  }

  @Input() isMobile = false;

  openMenuIndex: number | null = null;
  focusedIndex = 0;

  navigationItems: NavigationItem[] = [
    { 
      label: 'Home', 
      url: '/', 
      icon: 'home' 
    },
    { 
      label: 'People', 
      icon: 'people', 
      requireAuth: true,
      children: [
        { label: 'Browse People', url: '/Person', icon: 'list' },
        { label: 'Add Person', url: '/Person/Create', icon: 'person_add', requireRole: ['Admin', 'HouseholdAdmin'] },
        { label: 'Search People', url: '/Person?search=true', icon: 'search' }
      ]
    },
    { 
      label: 'Households', 
      icon: 'house', 
      requireAuth: true,
      children: [
        { label: 'View Households', url: '/Household', icon: 'view_list' },
        { label: 'Create Household', url: '/Household/Create', icon: 'add_home', requireRole: ['Admin', 'HouseholdAdmin'] }
      ]
    },
    { 
      label: 'Relationships', 
      icon: 'family_restroom', 
      requireAuth: true,
      children: [
        { label: 'Partnerships', url: '/Partnership', icon: 'favorite' },
        { label: 'Parent-Child', url: '/ParentChild', icon: 'escalator_warning' },
        { label: 'Add Relationship', url: '/Partnership/Create', icon: 'group_add', requireRole: ['Admin', 'HouseholdAdmin'] }
      ]
    },
    { 
      label: 'Media', 
      icon: 'photo_library', 
      requireAuth: true,
      children: [
        { label: 'Photo Gallery', url: '/MediaGallery', icon: 'photo' },
        { label: 'Upload Photos', url: '/MediaGallery/Upload', icon: 'upload', requireRole: ['Admin', 'HouseholdAdmin'] },
        { label: 'Videos', url: '/MediaGallery?type=video', icon: 'videocam' }
      ]
    },
    { 
      label: 'Content', 
      icon: 'article', 
      requireAuth: true,
      children: [
        { label: 'Wiki', url: '/Wiki', icon: 'description' },
        { label: 'Recipes', url: '/Recipe', icon: 'restaurant' },
        { label: 'Stories', url: '/StoryView', icon: 'book' },
        { label: 'Traditions', url: '/Tradition', icon: 'celebration' }
      ]
    },
    { 
      label: 'Calendar', 
      icon: 'event', 
      requireAuth: true,
      children: [
        { label: 'View Events', url: '/Calendar', icon: 'calendar_today' },
        { label: 'Create Event', url: '/Calendar/Create', icon: 'event_note', requireRole: ['Admin', 'HouseholdAdmin'] }
      ]
    },
    { 
      label: 'Account', 
      icon: 'account_circle', 
      requireAuth: true,
      children: [
        { label: 'Profile', url: '/Account/Profile', icon: 'person' },
        { label: 'Settings', url: '/Account/Settings', icon: 'settings' },
        { label: 'Notifications', url: '/Account/Notifications', icon: 'notifications' },
        { divider: true },
        { label: 'Logout', url: '/Account/Logout', icon: 'logout' }
      ]
    },
    { 
      label: 'Admin', 
      icon: 'admin_panel_settings', 
      requireAuth: true,
      requireRole: ['Admin'],
      children: [
        { label: 'User Management', url: '/Account/CreateUser', icon: 'manage_accounts' },
        { label: 'System Settings', url: '/Admin/Settings', icon: 'settings_applications' },
        { label: 'Style Guide', url: '/Home/StyleGuide', icon: 'palette' }
      ]
    }
  ];

  get visibleItems(): NavigationItem[] {
    return this.navigationItems.filter(item => this.isItemVisible(item));
  }

  isItemVisible(item: NavigationItem): boolean {
    // Check authentication requirement
    if (item.requireAuth && !this._userInfo.isAuthenticated) {
      return false;
    }
    
    // Check role requirement
    if (item.requireRole && item.requireRole.length > 0) {
      const hasRole = item.requireRole.some(role => {
        if (role === 'Admin') return this._userInfo.isAdmin;
        if (role === 'HouseholdAdmin') return this._userInfo.isHouseholdAdmin || this._userInfo.isAdmin;
        return false;
      });
      if (!hasRole) return false;
    }
    
    return true;
  }

  getVisibleChildren(item: NavigationItem): NavigationItem[] {
    if (!item.children) return [];
    return item.children.filter(child => !child.divider && this.isItemVisible(child));
  }

  toggleMenu(index: number): void {
    if (this.openMenuIndex === index) {
      this.openMenuIndex = null;
    } else {
      this.openMenuIndex = index;
    }
  }

  closeMenu(): void {
    this.openMenuIndex = null;
  }

  isMenuOpen(index: number): boolean {
    return this.openMenuIndex === index;
  }

  navigateTo(url: string | undefined): void {
    if (url) {
      window.location.href = url;
    }
  }

  isActive(url: string | undefined): boolean {
    if (!url) return false;
    if (url === '/') {
      return window.location.pathname === '/';
    }
    return window.location.pathname.startsWith(url);
  }

  // Keyboard navigation support
  @HostListener('document:keydown.escape')
  onEscape(): void {
    this.closeMenu();
  }

  @HostListener('document:keydown.arrowdown', ['$event'])
  onArrowDown(event: KeyboardEvent): void {
    if (this.isMobile && this.openMenuIndex === null) {
      event.preventDefault();
      this.focusedIndex = Math.min(this.focusedIndex + 1, this.visibleItems.length - 1);
    }
  }

  @HostListener('document:keydown.arrowup', ['$event'])
  onArrowUp(event: KeyboardEvent): void {
    if (this.isMobile && this.openMenuIndex === null) {
      event.preventDefault();
      this.focusedIndex = Math.max(this.focusedIndex - 1, 0);
    }
  }

  @HostListener('document:keydown.enter', ['$event'])
  onEnter(event: KeyboardEvent): void {
    if (this.isMobile && this.openMenuIndex === null) {
      event.preventDefault();
      const item = this.visibleItems[this.focusedIndex];
      if (item) {
        if (item.children && item.children.length > 0) {
          this.toggleMenu(this.focusedIndex);
        } else if (item.url) {
          this.navigateTo(item.url);
        }
      }
    }
  }
}
