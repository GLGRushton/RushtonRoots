import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface KeyboardShortcut {
  key: string;
  ctrlKey?: boolean;
  shiftKey?: boolean;
  altKey?: boolean;
  metaKey?: boolean;
  description: string;
  action: () => void;
}

/**
 * Service for managing keyboard shortcuts and navigation
 */
@Injectable({
  providedIn: 'root'
})
export class KeyboardNavigationService {
  private shortcuts: Map<string, KeyboardShortcut> = new Map();
  private shortcutTriggered$ = new Subject<KeyboardShortcut>();

  constructor() {
    this.registerDefaultShortcuts();
    this.initializeKeyboardListener();
  }

  /**
   * Register a keyboard shortcut
   */
  registerShortcut(id: string, shortcut: KeyboardShortcut): void {
    this.shortcuts.set(id, shortcut);
  }

  /**
   * Unregister a keyboard shortcut
   */
  unregisterShortcut(id: string): void {
    this.shortcuts.delete(id);
  }

  /**
   * Get all registered shortcuts
   */
  getAllShortcuts(): KeyboardShortcut[] {
    return Array.from(this.shortcuts.values());
  }

  /**
   * Get observable for shortcut triggers
   */
  getShortcutTriggered$() {
    return this.shortcutTriggered$.asObservable();
  }

  /**
   * Register default application shortcuts
   */
  private registerDefaultShortcuts(): void {
    // Skip to main content
    this.registerShortcut('skip-main', {
      key: 's',
      altKey: true,
      description: 'Skip to main content',
      action: () => this.skipToMain()
    });

    // Skip to navigation
    this.registerShortcut('skip-nav', {
      key: 'n',
      altKey: true,
      description: 'Skip to navigation',
      action: () => this.skipToNavigation()
    });

    // Open accessibility menu
    this.registerShortcut('accessibility-menu', {
      key: 'a',
      altKey: true,
      shiftKey: true,
      description: 'Open accessibility menu',
      action: () => this.openAccessibilityMenu()
    });

    // Focus search
    this.registerShortcut('focus-search', {
      key: '/',
      description: 'Focus search box',
      action: () => this.focusSearch()
    });

    // Show keyboard shortcuts help
    this.registerShortcut('show-help', {
      key: '?',
      shiftKey: true,
      description: 'Show keyboard shortcuts help',
      action: () => this.showKeyboardHelp()
    });
  }

  /**
   * Initialize keyboard event listener
   */
  private initializeKeyboardListener(): void {
    document.addEventListener('keydown', (event: KeyboardEvent) => {
      // Don't trigger shortcuts when typing in inputs
      const target = event.target as HTMLElement;
      if (target.tagName === 'INPUT' || target.tagName === 'TEXTAREA' || target.isContentEditable) {
        // Allow specific shortcuts like search (/)
        if (event.key === '/' && !event.ctrlKey && !event.altKey && !event.metaKey) {
          this.handleShortcut(event);
        }
        return;
      }

      this.handleShortcut(event);
    });
  }

  /**
   * Handle keyboard shortcut
   */
  private handleShortcut(event: KeyboardEvent): void {
    for (const shortcut of this.shortcuts.values()) {
      if (this.matchesShortcut(event, shortcut)) {
        event.preventDefault();
        shortcut.action();
        this.shortcutTriggered$.next(shortcut);
        break;
      }
    }
  }

  /**
   * Check if event matches shortcut
   */
  private matchesShortcut(event: KeyboardEvent, shortcut: KeyboardShortcut): boolean {
    return (
      event.key.toLowerCase() === shortcut.key.toLowerCase() &&
      !!event.ctrlKey === !!shortcut.ctrlKey &&
      !!event.shiftKey === !!shortcut.shiftKey &&
      !!event.altKey === !!shortcut.altKey &&
      !!event.metaKey === !!shortcut.metaKey
    );
  }

  /**
   * Skip to main content
   */
  private skipToMain(): void {
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
  private skipToNavigation(): void {
    const nav = document.querySelector('nav') || document.querySelector('[role="navigation"]');
    if (nav) {
      const firstLink = nav.querySelector('a');
      if (firstLink) {
        (firstLink as HTMLElement).focus();
      }
    }
  }

  /**
   * Open accessibility menu
   */
  private openAccessibilityMenu(): void {
    // This would trigger an event that the app component listens to
    window.dispatchEvent(new CustomEvent('open-accessibility-menu'));
  }

  /**
   * Focus search box
   */
  private focusSearch(): void {
    const search = document.querySelector('input[type="search"]') || 
                   document.querySelector('[role="search"] input') ||
                   document.querySelector('[aria-label*="search" i]');
    if (search) {
      (search as HTMLElement).focus();
    }
  }

  /**
   * Show keyboard shortcuts help
   */
  private showKeyboardHelp(): void {
    // This would trigger an event that shows the keyboard shortcuts dialog
    window.dispatchEvent(new CustomEvent('show-keyboard-shortcuts'));
  }
}
