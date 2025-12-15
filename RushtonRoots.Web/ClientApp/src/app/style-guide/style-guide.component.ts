import { Component } from '@angular/core';

@Component({
  selector: 'app-style-guide',
  templateUrl: './style-guide.component.html',
  styleUrls: ['./style-guide.component.scss'],
  standalone: false
})
export class StyleGuideComponent {
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
}
