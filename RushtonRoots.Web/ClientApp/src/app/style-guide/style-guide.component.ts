import { Component } from '@angular/core';
import { BreadcrumbItem } from '../shared/components/breadcrumb/breadcrumb.component';
import { PersonListItem } from '../shared/components/person-list/person-list.component';
import { ConfirmDialogService } from '../shared/components/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-style-guide',
  templateUrl: './style-guide.component.html',
  styleUrls: ['./style-guide.component.scss'],
  standalone: false
})
export class StyleGuideComponent {
  searchResult: string = '';
  dialogResult: string = '';
  activeSection: string = 'foundation';

  // Navigation sections
  sections = [
    { id: 'foundation', label: 'Foundation', icon: 'palette' },
    { id: 'core-components', label: 'Core Components', icon: 'widgets' },
    { id: 'person', label: 'Person Management', icon: 'person' },
    { id: 'household', label: 'Household Management', icon: 'home' },
    { id: 'relationship', label: 'Relationships', icon: 'family_restroom' },
    { id: 'auth', label: 'Authentication', icon: 'lock' },
    { id: 'content', label: 'Content Pages', icon: 'article' },
    { id: 'advanced', label: 'Advanced Features', icon: 'star' },
    { id: 'mobile', label: 'Mobile & PWA', icon: 'phone_android' },
    { id: 'accessibility', label: 'Accessibility', icon: 'accessibility' },
    { id: 'theme', label: 'Theme Customization', icon: 'color_lens' },
    { id: 'code-examples', label: 'Code Examples', icon: 'code' }
  ];

  sampleBreadcrumbs: BreadcrumbItem[] = [
    { label: 'Home', url: '/', icon: 'home' },
    { label: 'People', url: '/Person' },
    { label: 'John Doe' }
  ];

  samplePeople: PersonListItem[] = [
    { id: 1, firstName: 'John', lastName: 'Doe', birthDate: '1980-01-15', deathDate: '2050-12-31' },
    { id: 2, firstName: 'Jane', lastName: 'Smith', birthDate: '1985-05-20' },
    { id: 3, firstName: 'Robert', lastName: 'Johnson', birthDate: '1975-08-10', deathDate: '2045-03-25' },
    { id: 4, firstName: 'Emily', lastName: 'Williams', birthDate: '1990-11-30' },
    { id: 5, firstName: 'Michael', lastName: 'Brown', birthDate: '1972-02-14' },
    { id: 6, firstName: 'Sarah', lastName: 'Davis', birthDate: '1988-07-22' },
  ];

  colors = {
    primary: [
      { name: 'Primary Dark', value: '#1b5e20', variable: '$primary-dark' },
      { name: 'Primary', value: '#2e7d32', variable: '$primary' },
      { name: 'Primary Light', value: '#4caf50', variable: '$primary-light' },
      { name: 'Accent', value: '#66bb6a', variable: '$accent' }
    ],
    neutral: [
      { name: 'Text Primary', value: '#212121', variable: '$text-primary' },
      { name: 'Text Secondary', value: '#757575', variable: '$text-secondary' },
      { name: 'Background', value: '#f5f5f5', variable: '$background' },
      { name: 'Surface', value: '#ffffff', variable: '$surface' }
    ],
    semantic: [
      { name: 'Success', value: '#4caf50', variable: '$success' },
      { name: 'Warning', value: '#ff9800', variable: '$warning' },
      { name: 'Error', value: '#d32f2f', variable: '$error' },
      { name: 'Info', value: '#2196f3', variable: '$info' }
    ]
  };

  spacingSizes = [
    { name: 'XS', value: '4px', class: 'spacing-xs', variable: '$spacing-xs' },
    { name: 'SM', value: '8px', class: 'spacing-sm', variable: '$spacing-sm' },
    { name: 'MD', value: '16px', class: 'spacing-md', variable: '$spacing-md' },
    { name: 'LG', value: '24px', class: 'spacing-lg', variable: '$spacing-lg' },
    { name: 'XL', value: '32px', class: 'spacing-xl', variable: '$spacing-xl' },
    { name: 'XXL', value: '48px', class: 'spacing-xxl', variable: '$spacing-xxl' }
  ];

  typographyExamples = [
    { tag: 'h1', text: 'Heading 1 - Page Title', size: '32px', weight: '700' },
    { tag: 'h2', text: 'Heading 2 - Section Title', size: '24px', weight: '600' },
    { tag: 'h3', text: 'Heading 3 - Subsection Title', size: '20px', weight: '600' },
    { tag: 'h4', text: 'Heading 4 - Card Title', size: '18px', weight: '500' },
    { tag: 'h5', text: 'Heading 5 - Small Heading', size: '16px', weight: '500' },
    { tag: 'h6', text: 'Heading 6 - Label Heading', size: '14px', weight: '500' },
    { tag: 'p', text: 'Paragraph - Body text with normal line height and spacing for comfortable reading.', size: '16px', weight: '400' }
  ];

  iconExamples = [
    'home', 'person', 'family_restroom', 'search', 'edit', 'delete',
    'add', 'close', 'menu', 'settings', 'favorite', 'star',
    'calendar_today', 'location_on', 'photo', 'email', 'lock',
    'palette', 'widgets', 'article', 'phone_android', 'accessibility'
  ];

  // Component phase organization
  phases = [
    {
      name: 'Phase 1-2: Foundation & Layout',
      status: 'Complete',
      components: [
        'PersonCardComponent', 'PersonListComponent', 'SearchBarComponent', 
        'PageHeaderComponent', 'EmptyStateComponent', 'ConfirmDialogComponent', 
        'LoadingSpinnerComponent', 'BreadcrumbComponent', 'HeaderComponent',
        'NavigationComponent', 'UserMenuComponent', 'FooterComponent', 'PageLayoutComponent'
      ]
    },
    {
      name: 'Phase 3: Person Management',
      status: 'Complete',
      components: [
        'PersonIndexComponent', 'PersonSearchComponent', 'PersonTableComponent',
        'PersonDetailsComponent', 'PersonTimelineComponent', 'RelationshipVisualizerComponent',
        'PhotoGalleryComponent', 'PersonFormComponent', 'DatePickerComponent',
        'LocationAutocompleteComponent', 'PersonDeleteDialogComponent'
      ]
    },
    {
      name: 'Phase 4: Household Management',
      status: 'Complete',
      components: [
        'HouseholdIndexComponent', 'HouseholdCardComponent', 'HouseholdDetailsComponent',
        'HouseholdMembersComponent', 'MemberInviteDialogComponent', 
        'HouseholdSettingsComponent', 'HouseholdActivityTimelineComponent'
      ]
    },
    {
      name: 'Phase 5: Relationship Management',
      status: 'Complete',
      components: [
        'PartnershipIndexComponent', 'PartnershipCardComponent', 'PartnershipFormComponent',
        'PartnershipTimelineComponent', 'ParentChildIndexComponent', 'ParentChildCardComponent',
        'ParentChildFormComponent', 'FamilyTreeMiniComponent', 'RelationshipValidationComponent',
        'RelationshipSuggestionsComponent', 'BulkRelationshipImportComponent'
      ]
    },
    {
      name: 'Phase 6: Authentication',
      status: 'Complete',
      components: [
        'LoginComponent', 'ForgotPasswordComponent', 'ResetPasswordComponent',
        'UserProfileComponent', 'NotificationPreferencesComponent', 'PrivacySettingsComponent',
        'ConnectedAccountsComponent', 'AccountDeletionComponent'
      ]
    },
    {
      name: 'Phase 7: Content Pages',
      status: 'Complete',
      components: [
        'WikiIndexComponent', 'WikiArticleComponent', 'MarkdownEditorComponent',
        'RecipeCardComponent', 'RecipeDetailsComponent', 'StoryCardComponent',
        'TraditionCardComponent', 'ContentGridComponent'
      ]
    },
    {
      name: 'Phase 8: Advanced Features',
      status: 'Complete',
      components: [
        'MediaGalleryComponent', 'PhotoLightboxComponent', 'PhotoTaggingComponent',
        'AlbumManagerComponent', 'PhotoUploadComponent', 'PhotoEditorComponent',
        'VideoPlayerComponent', 'CalendarComponent', 'EventCardComponent',
        'EventFormDialogComponent', 'MessageThreadComponent', 'ChatInterfaceComponent',
        'NotificationPanelComponent'
      ]
    },
    {
      name: 'Phase 9: Mobile & PWA',
      status: 'Complete',
      components: [
        'MobileActionSheetComponent', 'MobileFilterSheetComponent',
        'InstallPromptComponent', 'OfflineIndicatorComponent', 'UpdatePromptComponent',
        'NotificationPromptComponent', 'PullToRefreshDirective', 'SwipeActionsDirective'
      ]
    },
    {
      name: 'Phase 10: Accessibility',
      status: 'Complete',
      components: [
        'SkipNavigationComponent', 'KeyboardShortcutsDialogComponent',
        'AccessibilityStatementComponent'
      ]
    }
  ];

  constructor(private confirmDialogService: ConfirmDialogService) {}

  onSearchChanged(searchTerm: string): void {
    this.searchResult = searchTerm;
  }

  onEmptyStateAction(): void {
    alert('Add Person action clicked!');
  }

  showConfirmDialog(): void {
    this.confirmDialogService.confirm({
      title: 'Confirm Action',
      message: 'Are you sure you want to proceed with this action?',
      confirmText: 'Yes, Proceed',
      cancelText: 'Cancel'
    }).subscribe(result => {
      this.dialogResult = result ? 'Confirmed' : 'Cancelled';
    });
  }

  showDeleteDialog(): void {
    this.confirmDialogService.confirmDelete('John Doe', 'Person')
      .subscribe(result => {
        this.dialogResult = result ? 'Delete confirmed' : 'Delete cancelled';
      });
  }

  scrollToSection(sectionId: string): void {
    this.activeSection = sectionId;
    const element = document.getElementById(sectionId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}
