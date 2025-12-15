import { Component, Input } from '@angular/core';

export interface NavigationItem {
  label: string;
  url: string;
  icon?: string;
  requireAuth?: boolean;
  requireRole?: string[];
}

/**
 * NavigationComponent - Responsive navigation menu
 * 
 * Features:
 * - Desktop horizontal navigation
 * - Mobile vertical navigation
 * - Role-based menu items
 * - Active route highlighting
 * 
 * Usage:
 * <app-navigation 
 *   [isAuthenticated]="true"
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
  @Input() isAuthenticated = false;
  @Input() isMobile = false;

  navigationItems: NavigationItem[] = [
    { label: 'Home', url: '/', icon: 'home' },
    { label: 'People', url: '/Person', icon: 'people', requireAuth: true },
    { label: 'Households', url: '/Household', icon: 'house', requireAuth: true },
    { label: 'Partnerships', url: '/Partnership', icon: 'favorite', requireAuth: true },
    { label: 'Parent-Child', url: '/ParentChild', icon: 'family_restroom', requireAuth: true },
    { label: 'Recipes', url: '/Recipe', icon: 'restaurant', requireAuth: true },
    { label: 'Stories', url: '/StoryView', icon: 'book', requireAuth: true },
    { label: 'Traditions', url: '/Tradition', icon: 'celebration', requireAuth: true },
    { label: 'Wiki', url: '/Wiki', icon: 'description', requireAuth: true }
  ];

  get visibleItems(): NavigationItem[] {
    return this.navigationItems.filter(item => {
      if (item.requireAuth && !this.isAuthenticated) {
        return false;
      }
      return true;
    });
  }

  navigateTo(url: string): void {
    window.location.href = url;
  }

  isActive(url: string): boolean {
    if (url === '/') {
      return window.location.pathname === '/';
    }
    return window.location.pathname.startsWith(url);
  }
}
