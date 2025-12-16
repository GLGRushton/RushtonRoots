import { Injectable, ApplicationRef } from '@angular/core';
import { SwUpdate, VersionReadyEvent } from '@angular/service-worker';
import { BehaviorSubject, Observable, concat, interval } from 'rxjs';
import { first, filter } from 'rxjs/operators';
import { ServiceWorkerUpdate, PWAFeatureSupport } from '../models/pwa.model';

/**
 * PWA Service for managing Progressive Web App features
 * Handles service worker registration, updates, and PWA capabilities
 */
@Injectable({
  providedIn: 'root'
})
export class PwaService {
  private updateAvailable$ = new BehaviorSubject<ServiceWorkerUpdate | null>(null);
  private featureSupport: PWAFeatureSupport;

  constructor(
    private swUpdate: SwUpdate,
    private appRef: ApplicationRef
  ) {
    this.featureSupport = this.detectFeatureSupport();
    this.initializeServiceWorker();
  }

  /**
   * Get observable for service worker updates
   */
  get onUpdateAvailable(): Observable<ServiceWorkerUpdate | null> {
    return this.updateAvailable$.asObservable();
  }

  /**
   * Check if service workers are supported
   */
  get isServiceWorkerSupported(): boolean {
    return this.featureSupport.serviceWorker;
  }

  /**
   * Get PWA feature support
   */
  get features(): PWAFeatureSupport {
    return this.featureSupport;
  }

  /**
   * Initialize service worker and check for updates
   */
  private initializeServiceWorker(): void {
    if (!this.swUpdate.isEnabled) {
      console.log('Service Worker is not enabled');
      return;
    }

    // Check for updates on startup
    this.checkForUpdates();

    // Allow the app to stabilize first, before starting
    // periodic checks for updates with `interval()`.
    const appIsStable$ = this.appRef.isStable.pipe(
      first(isStable => isStable === true)
    );

    // Check for updates every 6 hours once app is stable
    const everySixHours$ = interval(6 * 60 * 60 * 1000);
    const everySixHoursOnceAppIsStable$ = concat(appIsStable$, everySixHours$);

    everySixHoursOnceAppIsStable$.subscribe(async () => {
      try {
        await this.checkForUpdates();
      } catch (err) {
        console.error('Failed to check for updates:', err);
      }
    });

    // Listen for version updates
    this.swUpdate.versionUpdates
      .pipe(filter((evt): evt is VersionReadyEvent => evt.type === 'VERSION_READY'))
      .subscribe(evt => {
        const update: ServiceWorkerUpdate = {
          type: 'available',
          current: evt.currentVersion.hash,
          available: evt.latestVersion.hash
        };
        this.updateAvailable$.next(update);
        console.log('Update available:', update);
      });
  }

  /**
   * Manually check for service worker updates
   */
  async checkForUpdates(): Promise<boolean> {
    if (!this.swUpdate.isEnabled) {
      return false;
    }

    try {
      const updateFound = await this.swUpdate.checkForUpdate();
      console.log(updateFound ? 'A new version is available.' : 'Already on the latest version.');
      return updateFound;
    } catch (err) {
      console.error('Failed to check for updates:', err);
      return false;
    }
  }

  /**
   * Activate the latest version of the service worker
   */
  async activateUpdate(): Promise<void> {
    if (!this.swUpdate.isEnabled) {
      return;
    }

    try {
      await this.swUpdate.activateUpdate();
      this.updateAvailable$.next({
        type: 'activated',
        current: 'latest'
      });
      // Reload the page to apply the update
      window.location.reload();
    } catch (err) {
      console.error('Failed to activate update:', err);
      throw err;
    }
  }

  /**
   * Detect PWA feature support
   */
  private detectFeatureSupport(): PWAFeatureSupport {
    return {
      serviceWorker: 'serviceWorker' in navigator,
      pushNotifications: 'PushManager' in window && 'Notification' in window,
      backgroundSync: 'sync' in (self as any).registration || false,
      installPrompt: true, // Will be detected via beforeinstallprompt event
      periodicBackgroundSync: 'periodicSync' in (self as any).registration || false,
      badging: 'setAppBadge' in navigator || false,
      webShare: 'share' in navigator || false
    };
  }

  /**
   * Unregister the service worker (for debugging)
   */
  async unregisterServiceWorker(): Promise<void> {
    if ('serviceWorker' in navigator) {
      const registrations = await navigator.serviceWorker.getRegistrations();
      for (const registration of registrations) {
        await registration.unregister();
      }
      console.log('Service worker unregistered');
    }
  }

  /**
   * Get the current service worker registration
   */
  async getRegistration(): Promise<ServiceWorkerRegistration | null> {
    if ('serviceWorker' in navigator) {
      return await navigator.serviceWorker.getRegistration();
    }
    return null;
  }
}
