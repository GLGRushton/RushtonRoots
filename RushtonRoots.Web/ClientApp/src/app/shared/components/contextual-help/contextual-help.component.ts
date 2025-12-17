import { Component, Input } from '@angular/core';

/**
 * ContextualHelpComponent - Displays help links relevant to current page
 * 
 * Provides contextual help documentation links based on the current page/feature.
 * 
 * Usage:
 * <app-contextual-help
 *   [helpTopic]="'person-management'"
 *   [showIcon]="true">
 * </app-contextual-help>
 */
@Component({
  selector: 'app-contextual-help',
  standalone: false,
  templateUrl: './contextual-help.component.html',
  styleUrls: ['./contextual-help.component.scss']
})
export class ContextualHelpComponent {
  @Input() helpTopic: string = '';
  @Input() showIcon: boolean = true;
  @Input() iconOnly: boolean = false;

  private helpTopicMap: { [key: string]: { title: string; url: string } } = {
    'person-management': {
      title: 'Managing People',
      url: '/Help/PersonManagement'
    },
    'household-management': {
      title: 'Managing Households',
      url: '/Help/HouseholdManagement'
    },
    'relationship-management': {
      title: 'Managing Relationships',
      url: '/Help/RelationshipManagement'
    },
    'wiki': {
      title: 'Using the Wiki',
      url: '/Help/Wiki'
    },
    'recipes': {
      title: 'Recipe Management',
      url: '/Help/Recipes'
    },
    'stories': {
      title: 'Sharing Stories',
      url: '/Help/Stories'
    },
    'traditions': {
      title: 'Family Traditions',
      url: '/Help/Traditions'
    },
    'calendar': {
      title: 'Calendar & Events',
      url: '/Help/Calendar'
    },
    'account': {
      title: 'Account Settings',
      url: '/Help/Account'
    },
    'getting-started': {
      title: 'Getting Started Guide',
      url: '/Help/GettingStarted'
    }
  };

  /**
   * Get help info for current topic
   */
  get helpInfo(): { title: string; url: string } | null {
    return this.helpTopicMap[this.helpTopic] || null;
  }

  /**
   * Navigate to help page
   */
  navigateToHelp(): void {
    if (this.helpInfo) {
      window.open(this.helpInfo.url, '_blank');
    }
  }
}
