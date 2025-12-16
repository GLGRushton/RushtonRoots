import { Injectable, ElementRef } from '@angular/core';

/**
 * Service for managing focus and keyboard navigation
 */
@Injectable({
  providedIn: 'root'
})
export class FocusManagementService {
  private previouslyFocusedElement: HTMLElement | null = null;

  /**
   * Set focus to an element
   */
  setFocus(element: HTMLElement | ElementRef): void {
    const el = element instanceof ElementRef ? element.nativeElement : element;
    
    if (el) {
      // Store previously focused element
      this.previouslyFocusedElement = document.activeElement as HTMLElement;
      
      // Set focus
      el.focus();
    }
  }

  /**
   * Restore focus to previously focused element
   */
  restoreFocus(): void {
    if (this.previouslyFocusedElement) {
      this.previouslyFocusedElement.focus();
      this.previouslyFocusedElement = null;
    }
  }

  /**
   * Set focus to first focusable element within a container
   */
  setFocusToFirstElement(container: HTMLElement): void {
    const focusable = this.getFocusableElements(container);
    if (focusable.length > 0) {
      (focusable[0] as HTMLElement).focus();
    }
  }

  /**
   * Get all focusable elements within a container
   */
  getFocusableElements(container: HTMLElement): HTMLElement[] {
    const selector = [
      'a[href]',
      'button:not([disabled])',
      'input:not([disabled])',
      'select:not([disabled])',
      'textarea:not([disabled])',
      '[tabindex]:not([tabindex="-1"])'
    ].join(', ');

    return Array.from(container.querySelectorAll(selector)) as HTMLElement[];
  }

  /**
   * Trap focus within a container (for modals, dialogs)
   */
  trapFocus(container: HTMLElement): () => void {
    const focusable = this.getFocusableElements(container);
    
    if (focusable.length === 0) return () => {};

    const firstFocusable = focusable[0];
    const lastFocusable = focusable[focusable.length - 1];

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key !== 'Tab') return;

      if (event.shiftKey) {
        // Shift + Tab
        if (document.activeElement === firstFocusable) {
          event.preventDefault();
          lastFocusable.focus();
        }
      } else {
        // Tab
        if (document.activeElement === lastFocusable) {
          event.preventDefault();
          firstFocusable.focus();
        }
      }
    };

    container.addEventListener('keydown', handleKeyDown);

    // Focus first element
    firstFocusable.focus();

    // Return cleanup function
    return () => {
      container.removeEventListener('keydown', handleKeyDown);
    };
  }

  /**
   * Check if an element is focusable
   */
  isFocusable(element: HTMLElement): boolean {
    const focusable = this.getFocusableElements(element.parentElement || document.body);
    return focusable.includes(element);
  }

  /**
   * Get the next focusable element
   */
  getNextFocusable(currentElement: HTMLElement, container?: HTMLElement): HTMLElement | null {
    const focusable = this.getFocusableElements(container || document.body);
    const currentIndex = focusable.indexOf(currentElement);
    
    if (currentIndex === -1 || currentIndex === focusable.length - 1) {
      return null;
    }
    
    return focusable[currentIndex + 1];
  }

  /**
   * Get the previous focusable element
   */
  getPreviousFocusable(currentElement: HTMLElement, container?: HTMLElement): HTMLElement | null {
    const focusable = this.getFocusableElements(container || document.body);
    const currentIndex = focusable.indexOf(currentElement);
    
    if (currentIndex <= 0) {
      return null;
    }
    
    return focusable[currentIndex - 1];
  }
}
