import { Component } from '@angular/core';

/**
 * Skip navigation links component for accessibility
 * Allows keyboard users to skip to main content or navigation
 */
@Component({
  selector: 'app-skip-navigation',
  standalone: true,
  templateUrl: './skip-navigation.component.html',
  styleUrls: ['./skip-navigation.component.scss']
})
export class SkipNavigationComponent {
  /**
   * Skip to main content
   */
  skipToMain(): void {
    const main = document.querySelector('main') || document.querySelector('[role="main"]');
    if (main) {
      (main as HTMLElement).setAttribute('tabindex', '-1');
      (main as HTMLElement).focus();
      main.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  /**
   * Skip to navigation
   */
  skipToNavigation(): void {
    const nav = document.querySelector('nav') || document.querySelector('[role="navigation"]');
    if (nav) {
      (nav as HTMLElement).setAttribute('tabindex', '-1');
      (nav as HTMLElement).focus();
      nav.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  /**
   * Skip to footer
   */
  skipToFooter(): void {
    const footer = document.querySelector('footer') || document.querySelector('[role="contentinfo"]');
    if (footer) {
      (footer as HTMLElement).setAttribute('tabindex', '-1');
      (footer as HTMLElement).focus();
      footer.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}
