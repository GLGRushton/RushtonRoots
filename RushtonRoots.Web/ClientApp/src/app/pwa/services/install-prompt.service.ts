import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, fromEvent } from 'rxjs';
import { InstallPromptEvent, InstallPromptState, InstallInstructions } from '../models/pwa.model';

/**
 * Install Prompt Service for handling PWA installation
 */
@Injectable({
  providedIn: 'root'
})
export class InstallPromptService {
  private promptState$ = new BehaviorSubject<InstallPromptState>({
    canInstall: false,
    isInstalled: false,
    deferredPrompt: null
  });

  constructor() {
    this.initializeInstallPrompt();
    this.detectInstalledState();
  }

  /**
   * Get observable for install prompt state
   */
  get state(): Observable<InstallPromptState> {
    return this.promptState$.asObservable();
  }

  /**
   * Get current state value
   */
  get currentState(): InstallPromptState {
    return this.promptState$.value;
  }

  /**
   * Initialize install prompt event listener
   */
  private initializeInstallPrompt(): void {
    // Listen for beforeinstallprompt event
    fromEvent<InstallPromptEvent>(window, 'beforeinstallprompt').subscribe((event) => {
      // Prevent the mini-infobar from appearing on mobile
      event.preventDefault();

      // Stash the event so it can be triggered later
      this.promptState$.next({
        ...this.promptState$.value,
        canInstall: true,
        deferredPrompt: event
      });

      console.log('Install prompt event captured');
    });

    // Listen for app installed event
    fromEvent(window, 'appinstalled').subscribe(() => {
      console.log('PWA was installed');
      this.promptState$.next({
        canInstall: false,
        isInstalled: true,
        deferredPrompt: null
      });
    });
  }

  /**
   * Detect if app is already installed
   */
  private detectInstalledState(): void {
    // Check if running in standalone mode (installed PWA)
    const isStandalone = window.matchMedia('(display-mode: standalone)').matches;
    const isIosStandalone = (window.navigator as any).standalone === true;

    if (isStandalone || isIosStandalone) {
      this.promptState$.next({
        ...this.promptState$.value,
        isInstalled: true
      });
    }
  }

  /**
   * Show the install prompt
   */
  async showInstallPrompt(): Promise<'accepted' | 'dismissed' | 'not-available'> {
    const state = this.promptState$.value;

    if (!state.deferredPrompt) {
      console.warn('Install prompt is not available');
      return 'not-available';
    }

    try {
      // Show the install prompt
      await state.deferredPrompt.prompt();

      // Wait for the user to respond to the prompt
      const { outcome } = await state.deferredPrompt.userChoice;

      console.log(`User response to install prompt: ${outcome}`);

      // Clear the deferred prompt
      this.promptState$.next({
        ...this.promptState$.value,
        canInstall: false,
        deferredPrompt: null
      });

      return outcome;
    } catch (error) {
      console.error('Error showing install prompt:', error);
      return 'dismissed';
    }
  }

  /**
   * Get platform-specific installation instructions
   */
  getInstallInstructions(): InstallInstructions | null {
    const userAgent = navigator.userAgent.toLowerCase();

    // iOS Safari
    if (/iphone|ipad|ipod/.test(userAgent) && /safari/.test(userAgent) && !/crios/.test(userAgent)) {
      return {
        platform: 'ios',
        browser: 'Safari',
        steps: [
          'Tap the Share button',
          'Scroll down and tap "Add to Home Screen"',
          'Tap "Add" in the top right corner'
        ],
        icon: 'ios_share'
      };
    }

    // Android Chrome
    if (/android/.test(userAgent) && /chrome/.test(userAgent)) {
      return {
        platform: 'android',
        browser: 'Chrome',
        steps: [
          'Tap the menu button (three dots)',
          'Tap "Add to Home screen"',
          'Tap "Add" to confirm'
        ],
        icon: 'more_vert'
      };
    }

    // Desktop Chrome/Edge
    if (/chrome|edg/.test(userAgent) && !/mobile/.test(userAgent)) {
      return {
        platform: 'desktop',
        browser: 'Chrome/Edge',
        steps: [
          'Click the install icon in the address bar',
          'Or open the menu and select "Install RushtonRoots"',
          'Click "Install" in the dialog'
        ],
        icon: 'get_app'
      };
    }

    return null;
  }

  /**
   * Check if the app can be installed
   */
  canInstall(): boolean {
    return this.promptState$.value.canInstall;
  }

  /**
   * Check if the app is already installed
   */
  isInstalled(): boolean {
    return this.promptState$.value.isInstalled;
  }

  /**
   * Manually set installed state (for testing)
   */
  setInstalledState(installed: boolean): void {
    this.promptState$.next({
      ...this.promptState$.value,
      isInstalled: installed
    });
  }
}
