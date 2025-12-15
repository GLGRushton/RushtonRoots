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
      { name: 'Primary Dark', value: '#1b5e20' },
      { name: 'Primary', value: '#2e7d32' },
      { name: 'Primary Light', value: '#4caf50' },
      { name: 'Accent', value: '#66bb6a' }
    ],
    neutral: [
      { name: 'Text Primary', value: '#212121' },
      { name: 'Text Secondary', value: '#757575' },
      { name: 'Background', value: '#f5f5f5' },
      { name: 'Surface', value: '#ffffff' }
    ],
    semantic: [
      { name: 'Warning', value: '#d32f2f' }
    ]
  };

  spacingSizes = [
    { name: 'XS', value: '4px', class: 'spacing-xs' },
    { name: 'SM', value: '8px', class: 'spacing-sm' },
    { name: 'MD', value: '16px', class: 'spacing-md' },
    { name: 'LG', value: '24px', class: 'spacing-lg' },
    { name: 'XL', value: '32px', class: 'spacing-xl' },
    { name: 'XXL', value: '48px', class: 'spacing-xxl' }
  ];

  typographyExamples = [
    { tag: 'h1', text: 'Heading 1 - Page Title' },
    { tag: 'h2', text: 'Heading 2 - Section Title' },
    { tag: 'h3', text: 'Heading 3 - Subsection Title' },
    { tag: 'h4', text: 'Heading 4 - Card Title' },
    { tag: 'h5', text: 'Heading 5 - Small Heading' },
    { tag: 'h6', text: 'Heading 6 - Label Heading' },
    { tag: 'p', text: 'Paragraph - Body text with normal line height and spacing for comfortable reading.' }
  ];

  iconExamples = [
    'home', 'person', 'family_restroom', 'search', 'edit', 'delete',
    'add', 'close', 'menu', 'settings', 'favorite', 'star',
    'calendar_today', 'location_on', 'photo', 'email'
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
}
