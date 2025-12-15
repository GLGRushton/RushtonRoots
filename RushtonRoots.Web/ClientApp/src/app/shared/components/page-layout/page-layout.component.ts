import { Component, Input } from '@angular/core';

/**
 * PageLayoutComponent - Wrapper component for consistent page layout
 * 
 * Features:
 * - Consistent page container widths
 * - Standardized spacing and rhythm
 * - Optional background patterns
 * - Page transition animations
 * - Responsive design
 * 
 * Usage:
 * <app-page-layout 
 *   [maxWidth]="'1400px'"
 *   [padding]="'large'"
 *   [background]="'gradient'">
 *   <ng-content></ng-content>
 * </app-page-layout>
 */
@Component({
  selector: 'app-page-layout',
  standalone: false,
  templateUrl: './page-layout.component.html',
  styleUrls: ['./page-layout.component.scss']
})
export class PageLayoutComponent {
  /**
   * Maximum width of the content container
   * Options: 'narrow' (800px), 'medium' (1200px), 'wide' (1400px), 'full' (100%)
   * Default: 'wide'
   */
  @Input() maxWidth: 'narrow' | 'medium' | 'wide' | 'full' = 'wide';

  /**
   * Padding size for the container
   * Options: 'small', 'medium', 'large'
   * Default: 'large'
   */
  @Input() padding: 'small' | 'medium' | 'large' = 'large';

  /**
   * Background style
   * Options: 'none', 'gradient', 'pattern'
   * Default: 'gradient'
   */
  @Input() background: 'none' | 'gradient' | 'pattern' = 'gradient';

  /**
   * Enable page transition animation
   * Default: true
   */
  @Input() animate = true;

  get containerClass(): string {
    const classes = ['page-container'];
    
    // Add max-width class
    classes.push(`max-width-${this.maxWidth}`);
    
    // Add padding class
    classes.push(`padding-${this.padding}`);
    
    // Add background class
    if (this.background !== 'none') {
      classes.push(`background-${this.background}`);
    }
    
    // Add animation class
    if (this.animate) {
      classes.push('animate-page-enter');
    }
    
    return classes.join(' ');
  }
}
