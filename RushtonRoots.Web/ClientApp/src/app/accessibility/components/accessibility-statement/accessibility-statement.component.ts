import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { AccessibilityTestingService } from '../../services/accessibility-testing.service';

/**
 * Accessibility statement component
 * Documents the application's accessibility features and compliance
 */
@Component({
  selector: 'app-accessibility-statement',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatDividerModule,
    MatButtonModule,
    MatExpansionModule
  ],
  templateUrl: './accessibility-statement.component.html',
  styleUrls: ['./accessibility-statement.component.scss']
})
export class AccessibilityStatementComponent implements OnInit {
  lastUpdated: Date = new Date();
  
  features = [
    {
      title: 'Keyboard Navigation',
      icon: 'keyboard',
      description: 'Full keyboard support for all interactive elements',
      details: [
        'Tab and Shift+Tab to navigate between elements',
        'Enter and Space to activate buttons and links',
        'Arrow keys for menus and dropdowns',
        'Escape to close dialogs and menus',
        'Custom keyboard shortcuts for common actions'
      ]
    },
    {
      title: 'Screen Reader Support',
      icon: 'record_voice_over',
      description: 'Optimized for screen readers like NVDA, JAWS, and VoiceOver',
      details: [
        'Proper semantic HTML structure',
        'ARIA labels and descriptions',
        'Landmarks for easy navigation',
        'Live regions for dynamic content updates',
        'Alternative text for all images'
      ]
    },
    {
      title: 'Visual Accessibility',
      icon: 'visibility',
      description: 'High contrast and color considerations',
      details: [
        'WCAG 2.1 AA compliant color contrast (4.5:1 for normal text)',
        'Clear focus indicators',
        'Text resizable up to 200% without loss of content',
        'Multiple ways to navigate content',
        'No information conveyed by color alone'
      ]
    },
    {
      title: 'Skip Navigation',
      icon: 'skip_next',
      description: 'Quick access to main content',
      details: [
        'Skip to main content link',
        'Skip to navigation link',
        'Skip to footer link',
        'Visible on keyboard focus',
        'Accessible via keyboard shortcuts (Alt+S)'
      ]
    },
    {
      title: 'Responsive Design',
      icon: 'devices',
      description: 'Works on all devices and screen sizes',
      details: [
        'Mobile-first responsive design',
        'Touch-friendly targets (44x44px minimum)',
        'Orientation support (portrait and landscape)',
        'Zoom support up to 400%',
        'Content reflows at different screen sizes'
      ]
    },
    {
      title: 'Forms & Input',
      icon: 'edit',
      description: 'Accessible forms with clear labels and error messages',
      details: [
        'Visible labels for all form fields',
        'Error messages clearly associated with fields',
        'Required fields clearly marked',
        'Inline validation with feedback',
        'Time limits with warning and extension options'
      ]
    }
  ];

  standards = [
    {
      name: 'WCAG 2.1 Level AA',
      status: 'Compliant',
      icon: 'check_circle',
      description: 'We conform to the Web Content Accessibility Guidelines (WCAG) 2.1 at Level AA'
    },
    {
      name: 'Section 508',
      status: 'Compliant',
      icon: 'check_circle',
      description: 'Compliant with Section 508 of the Rehabilitation Act'
    },
    {
      name: 'ADA',
      status: 'Compliant',
      icon: 'check_circle',
      description: 'Compliant with the Americans with Disabilities Act (ADA)'
    }
  ];

  knownIssues = [
    {
      title: 'Third-party Content',
      description: 'Some embedded third-party content may not be fully accessible',
      workaround: 'We provide alternative ways to access this information',
      priority: 'Medium'
    }
  ];

  constructor(private a11yTestingService: AccessibilityTestingService) {}

  ngOnInit(): void {
    // Run accessibility audit in development mode
    if (!this.isProduction()) {
      this.runAccessibilityAudit();
    }
  }

  /**
   * Run accessibility audit
   */
  async runAccessibilityAudit(): Promise<void> {
    try {
      const results = await this.a11yTestingService.runAudit();
      this.a11yTestingService.logResults(results);
    } catch (error) {
      console.error('Accessibility audit failed:', error);
    }
  }

  /**
   * Check if running in production
   */
  private isProduction(): boolean {
    return window.location.hostname !== 'localhost' && 
           !window.location.hostname.includes('127.0.0.1');
  }

  /**
   * Report accessibility issue
   */
  reportIssue(): void {
    // This would open a dialog or navigate to a feedback form
    console.log('Report accessibility issue');
  }

  /**
   * View testing results
   */
  viewTestResults(): void {
    this.runAccessibilityAudit();
  }
}
