import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { BackgroundSyncRegistration, SyncableFormData } from '../models/pwa.model';
import { NetworkStatusService } from './network-status.service';

/**
 * Background Sync Service for offline form submissions
 */
@Injectable({
  providedIn: 'root'
})
export class BackgroundSyncService {
  private readonly STORAGE_KEY = 'rushtonroots_background_sync';
  private syncQueue$ = new BehaviorSubject<BackgroundSyncRegistration[]>([]);
  private maxRetries = 3;

  constructor(private networkStatus: NetworkStatusService) {
    this.loadQueue();
    this.initializeBackgroundSync();
    this.monitorNetworkStatus();
  }

  /**
   * Get observable for sync queue
   */
  get queue(): Observable<BackgroundSyncRegistration[]> {
    return this.syncQueue$.asObservable();
  }

  /**
   * Get current sync queue
   */
  get currentQueue(): BackgroundSyncRegistration[] {
    return this.syncQueue$.value;
  }

  /**
   * Register a form for background sync
   */
  async registerSync(formData: SyncableFormData): Promise<void> {
    const registration: BackgroundSyncRegistration = {
      tag: `form-sync-${formData.formId}-${Date.now()}`,
      data: formData,
      timestamp: Date.now(),
      status: 'pending',
      retryCount: 0
    };

    // Add to queue
    const queue = [...this.syncQueue$.value, registration];
    this.syncQueue$.next(queue);
    this.saveQueue();

    console.log('Registered background sync:', registration.tag);

    // Try to sync immediately if online
    if (this.networkStatus.isOnline) {
      await this.syncItem(registration);
    } else {
      // Register with service worker if supported
      await this.registerServiceWorkerSync(registration.tag);
    }
  }

  /**
   * Initialize background sync capability
   */
  private initializeBackgroundSync(): void {
    // Listen for sync events from service worker
    if ('serviceWorker' in navigator && 'sync' in (self as any).registration) {
      navigator.serviceWorker.ready.then((registration) => {
        // Service worker is ready
        console.log('Background sync is available');
      });
    } else {
      console.log('Background sync is not supported, falling back to manual retry');
    }
  }

  /**
   * Monitor network status and retry pending syncs when online
   */
  private monitorNetworkStatus(): void {
    this.networkStatus.status.subscribe((status) => {
      if (status.online) {
        this.retryPendingSyncs();
      }
    });
  }

  /**
   * Register sync tag with service worker
   */
  private async registerServiceWorkerSync(tag: string): Promise<void> {
    if (!('serviceWorker' in navigator) || !('sync' in (self as any).registration)) {
      return;
    }

    try {
      const registration = await navigator.serviceWorker.ready;
      await (registration as any).sync.register(tag);
      console.log('Registered sync tag with service worker:', tag);
    } catch (error) {
      console.error('Failed to register sync tag:', error);
    }
  }

  /**
   * Sync a specific item
   */
  private async syncItem(item: BackgroundSyncRegistration): Promise<void> {
    const formData = item.data as SyncableFormData;

    // Update status to syncing
    this.updateItemStatus(item.tag, 'syncing');

    try {
      // Perform the actual HTTP request
      const response = await fetch(formData.url, {
        method: formData.method,
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData.data)
      });

      if (response.ok) {
        // Success - remove from queue
        this.removeItem(item.tag);
        console.log('Background sync completed:', item.tag);
      } else {
        // Failed - mark as failed and retry
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Background sync failed:', error);

      // Increment retry count
      item.retryCount++;

      if (item.retryCount >= this.maxRetries) {
        // Max retries reached - mark as failed
        this.updateItemStatus(item.tag, 'failed');
      } else {
        // Retry later
        this.updateItemStatus(item.tag, 'pending');
      }
    }
  }

  /**
   * Retry all pending syncs
   */
  private async retryPendingSyncs(): Promise<void> {
    const pending = this.syncQueue$.value.filter(
      (item) => item.status === 'pending' && item.retryCount < this.maxRetries
    );

    for (const item of pending) {
      await this.syncItem(item);
    }
  }

  /**
   * Update item status in queue
   */
  private updateItemStatus(tag: string, status: BackgroundSyncRegistration['status']): void {
    const queue = this.syncQueue$.value.map((item) =>
      item.tag === tag ? { ...item, status } : item
    );
    this.syncQueue$.next(queue);
    this.saveQueue();
  }

  /**
   * Remove item from queue
   */
  private removeItem(tag: string): void {
    const queue = this.syncQueue$.value.filter((item) => item.tag !== tag);
    this.syncQueue$.next(queue);
    this.saveQueue();
  }

  /**
   * Clear all completed and failed items
   */
  clearCompleted(): void {
    const queue = this.syncQueue$.value.filter(
      (item) => item.status === 'pending' || item.status === 'syncing'
    );
    this.syncQueue$.next(queue);
    this.saveQueue();
  }

  /**
   * Manually retry a failed item
   */
  async retryItem(tag: string): Promise<void> {
    const item = this.syncQueue$.value.find((i) => i.tag === tag);
    if (item) {
      item.retryCount = 0; // Reset retry count
      await this.syncItem(item);
    }
  }

  /**
   * Save queue to local storage
   */
  private saveQueue(): void {
    try {
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(this.syncQueue$.value));
    } catch (error) {
      console.error('Failed to save sync queue:', error);
    }
  }

  /**
   * Load queue from local storage
   */
  private loadQueue(): void {
    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      if (stored) {
        const queue = JSON.parse(stored) as BackgroundSyncRegistration[];
        this.syncQueue$.next(queue);
      }
    } catch (error) {
      console.error('Failed to load sync queue:', error);
    }
  }

  /**
   * Get pending sync count
   */
  getPendingCount(): number {
    return this.syncQueue$.value.filter((item) => item.status === 'pending').length;
  }
}
