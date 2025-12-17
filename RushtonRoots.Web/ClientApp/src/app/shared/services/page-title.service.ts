import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { filter, map } from 'rxjs/operators';

/**
 * Service for managing page titles
 * Automatically updates browser tab title based on current route
 */
@Injectable({
  providedIn: 'root'
})
export class PageTitleService {
  private readonly appName = 'RushtonRoots';
  private readonly separator = ' - ';

  constructor(
    private titleService: Title,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {
    this.initializeTitleUpdates();
  }

  /**
   * Set page title
   * Automatically appends app name
   */
  setTitle(title: string): void {
    const fullTitle = title ? `${title}${this.separator}${this.appName}` : this.appName;
    this.titleService.setTitle(fullTitle);
  }

  /**
   * Set title without app name
   */
  setTitleRaw(title: string): void {
    this.titleService.setTitle(title);
  }

  /**
   * Get current title
   */
  getTitle(): string {
    return this.titleService.getTitle();
  }

  /**
   * Initialize automatic title updates based on routing
   */
  private initializeTitleUpdates(): void {
    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        map(() => {
          let child = this.activatedRoute.firstChild;
          while (child) {
            if (child.firstChild) {
              child = child.firstChild;
            } else if (child.snapshot.data && child.snapshot.data['title']) {
              return child.snapshot.data['title'];
            } else {
              return null;
            }
          }
          return null;
        })
      )
      .subscribe((title: string | null) => {
        if (title) {
          this.setTitle(title);
        }
      });
  }

  /**
   * Build title from route segments
   */
  buildTitleFromRoute(segments: string[], dynamicData?: Map<string, string>): string {
    const titleMap: { [key: string]: string } = {
      // Main sections
      'Person': 'People',
      'Household': 'Households',
      'Partnership': 'Partnerships',
      'ParentChild': 'Relationships',
      'Wiki': 'Wiki',
      'Recipe': 'Recipes',
      'StoryView': 'Stories',
      'Tradition': 'Traditions',
      'Calendar': 'Calendar',
      'Account': 'Account',

      // Actions
      'Create': 'Add New',
      'Edit': 'Edit',
      'Delete': 'Delete',
      'Details': 'Details',
      'Members': 'Members',
      'Index': 'Browse'
    };

    const titleParts: string[] = [];

    for (const segment of segments) {
      // Skip numeric IDs
      if (!isNaN(Number(segment))) {
        continue;
      }

      // Check for dynamic data
      const dynamicTitle = dynamicData?.get(segment);
      if (dynamicTitle) {
        titleParts.push(dynamicTitle);
        continue;
      }

      // Use mapped title or capitalize segment
      const mappedTitle = titleMap[segment];
      if (mappedTitle) {
        titleParts.push(mappedTitle);
      }
    }

    return titleParts.reverse().join(' - ');
  }

  /**
   * Build specific page titles
   */
  buildPersonTitle(personName: string, action?: 'Details' | 'Edit' | 'Delete'): string {
    if (action) {
      return `${action} ${personName}`;
    }
    return personName;
  }

  buildHouseholdTitle(householdName: string, action?: 'Details' | 'Edit' | 'Delete' | 'Members'): string {
    if (action) {
      return `${action} ${householdName}`;
    }
    return householdName;
  }

  buildWikiTitle(articleTitle: string, category?: string): string {
    if (category) {
      return `${articleTitle} - ${category}`;
    }
    return articleTitle;
  }
}
