import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { BreadcrumbItem } from '../components/breadcrumb/breadcrumb.component';

/**
 * Service for managing application breadcrumbs
 * Provides dynamic breadcrumb management across the application
 */
@Injectable({
  providedIn: 'root'
})
export class BreadcrumbService {
  private breadcrumbs$ = new BehaviorSubject<BreadcrumbItem[]>([]);

  /**
   * Get breadcrumbs as observable
   */
  getBreadcrumbs(): Observable<BreadcrumbItem[]> {
    return this.breadcrumbs$.asObservable();
  }

  /**
   * Set breadcrumbs
   */
  setBreadcrumbs(breadcrumbs: BreadcrumbItem[]): void {
    this.breadcrumbs$.next(breadcrumbs);
  }

  /**
   * Clear breadcrumbs
   */
  clearBreadcrumbs(): void {
    this.breadcrumbs$.next([]);
  }

  /**
   * Add breadcrumb item
   */
  addBreadcrumb(breadcrumb: BreadcrumbItem): void {
    const current = this.breadcrumbs$.value;
    this.breadcrumbs$.next([...current, breadcrumb]);
  }

  /**
   * Build breadcrumbs from route segments
   * Examples:
   * - /Person -> Home > People
   * - /Person/Details/1 -> Home > People > John Doe
   * - /Person/Edit/1 -> Home > People > John Doe > Edit
   */
  buildBreadcrumbsFromRoute(
    segments: string[],
    dynamicData?: Map<string, string>
  ): BreadcrumbItem[] {
    const breadcrumbs: BreadcrumbItem[] = [
      { label: 'Home', url: '/', icon: 'home' }
    ];

    let currentPath = '';
    
    for (let i = 0; i < segments.length; i++) {
      const segment = segments[i];
      currentPath += `/${segment}`;

      // Skip numeric IDs
      if (!isNaN(Number(segment))) {
        continue;
      }

      const breadcrumb = this.mapSegmentToBreadcrumb(
        segment,
        currentPath,
        i < segments.length - 1,
        dynamicData
      );

      if (breadcrumb) {
        breadcrumbs.push(breadcrumb);
      }
    }

    return breadcrumbs;
  }

  /**
   * Map route segment to breadcrumb item
   */
  private mapSegmentToBreadcrumb(
    segment: string,
    path: string,
    hasNext: boolean,
    dynamicData?: Map<string, string>
  ): BreadcrumbItem | null {
    const labelMap: { [key: string]: { label: string; icon?: string } } = {
      // Main sections
      'Person': { label: 'People', icon: 'people' },
      'Household': { label: 'Households', icon: 'home' },
      'Partnership': { label: 'Partnerships', icon: 'favorite' },
      'ParentChild': { label: 'Relationships', icon: 'family_restroom' },
      'Wiki': { label: 'Wiki', icon: 'menu_book' },
      'Recipe': { label: 'Recipes', icon: 'restaurant' },
      'StoryView': { label: 'Stories', icon: 'auto_stories' },
      'Tradition': { label: 'Traditions', icon: 'celebration' },
      'Calendar': { label: 'Calendar', icon: 'event' },
      'Account': { label: 'Account', icon: 'account_circle' },

      // Actions
      'Create': { label: 'Add New', icon: 'add' },
      'Edit': { label: 'Edit', icon: 'edit' },
      'Delete': { label: 'Delete', icon: 'delete' },
      'Details': { label: 'Details', icon: 'info' },
      'Members': { label: 'Members', icon: 'group' },
      'Index': { label: 'Browse', icon: 'list' }
    };

    const config = labelMap[segment];
    if (!config) {
      return null;
    }

    // Check for dynamic data (e.g., person name, household name)
    const dynamicLabel = dynamicData?.get(segment);
    
    return {
      label: dynamicLabel || config.label,
      url: hasNext ? path : undefined,
      icon: config.icon
    };
  }

  /**
   * Build breadcrumbs for specific pages
   */
  buildPersonBreadcrumbs(personName?: string, action?: 'Details' | 'Edit' | 'Delete'): BreadcrumbItem[] {
    const breadcrumbs: BreadcrumbItem[] = [
      { label: 'Home', url: '/', icon: 'home' },
      { label: 'People', url: '/Person', icon: 'people' }
    ];

    if (personName) {
      breadcrumbs.push({
        label: personName,
        url: action ? `/Person/Details` : undefined,
        icon: 'person'
      });

      if (action) {
        const actionMap = {
          'Details': { label: 'Details', icon: 'info' },
          'Edit': { label: 'Edit', icon: 'edit' },
          'Delete': { label: 'Delete', icon: 'delete' }
        };
        breadcrumbs.push({
          label: actionMap[action].label,
          icon: actionMap[action].icon
        });
      }
    }

    return breadcrumbs;
  }

  buildHouseholdBreadcrumbs(householdName?: string, action?: 'Details' | 'Edit' | 'Delete' | 'Members'): BreadcrumbItem[] {
    const breadcrumbs: BreadcrumbItem[] = [
      { label: 'Home', url: '/', icon: 'home' },
      { label: 'Households', url: '/Household', icon: 'home' }
    ];

    if (householdName) {
      breadcrumbs.push({
        label: householdName,
        url: action ? `/Household/Details` : undefined,
        icon: 'house'
      });

      if (action) {
        const actionMap = {
          'Details': { label: 'Details', icon: 'info' },
          'Edit': { label: 'Edit', icon: 'edit' },
          'Delete': { label: 'Delete', icon: 'delete' },
          'Members': { label: 'Members', icon: 'group' }
        };
        breadcrumbs.push({
          label: actionMap[action].label,
          icon: actionMap[action].icon
        });
      }
    }

    return breadcrumbs;
  }

  buildWikiBreadcrumbs(category?: string, articleTitle?: string): BreadcrumbItem[] {
    const breadcrumbs: BreadcrumbItem[] = [
      { label: 'Home', url: '/', icon: 'home' },
      { label: 'Wiki', url: '/Wiki', icon: 'menu_book' }
    ];

    if (category) {
      breadcrumbs.push({
        label: category,
        url: articleTitle ? `/Wiki?category=${category}` : undefined,
        icon: 'folder'
      });

      if (articleTitle) {
        breadcrumbs.push({
          label: articleTitle,
          icon: 'article'
        });
      }
    }

    return breadcrumbs;
  }
}
