import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, fromEvent, merge } from 'rxjs';
import { map, startWith, debounceTime } from 'rxjs/operators';
import { NetworkStatus } from '../models/pwa.model';

/**
 * Network Status Service for detecting online/offline state
 */
@Injectable({
  providedIn: 'root'
})
export class NetworkStatusService {
  private networkStatus$ = new BehaviorSubject<NetworkStatus>(this.getCurrentNetworkStatus());

  constructor() {
    this.initializeNetworkMonitoring();
  }

  /**
   * Get observable for network status
   */
  get status(): Observable<NetworkStatus> {
    return this.networkStatus$.asObservable();
  }

  /**
   * Get current network status
   */
  get currentStatus(): NetworkStatus {
    return this.networkStatus$.value;
  }

  /**
   * Check if currently online
   */
  get isOnline(): boolean {
    return this.networkStatus$.value.online;
  }

  /**
   * Check if currently offline
   */
  get isOffline(): boolean {
    return !this.networkStatus$.value.online;
  }

  /**
   * Initialize network status monitoring
   */
  private initializeNetworkMonitoring(): void {
    // Listen to online/offline events
    const online$ = fromEvent(window, 'online').pipe(map(() => true));
    const offline$ = fromEvent(window, 'offline').pipe(map(() => false));

    merge(online$, offline$)
      .pipe(
        startWith(navigator.onLine),
        debounceTime(100)
      )
      .subscribe(() => {
        const status = this.getCurrentNetworkStatus();
        this.networkStatus$.next(status);
        console.log('Network status changed:', status);
      });

    // Monitor connection quality if supported
    if ('connection' in navigator) {
      const connection = (navigator as any).connection;
      if (connection) {
        fromEvent(connection, 'change')
          .pipe(debounceTime(500))
          .subscribe(() => {
            const status = this.getCurrentNetworkStatus();
            this.networkStatus$.next(status);
          });
      }
    }
  }

  /**
   * Get current network status including connection quality
   */
  private getCurrentNetworkStatus(): NetworkStatus {
    const status: NetworkStatus = {
      online: navigator.onLine
    };

    // Add connection quality information if available
    if ('connection' in navigator) {
      const connection = (navigator as any).connection;
      if (connection) {
        status.effectiveType = connection.effectiveType;
        status.downlink = connection.downlink;
        status.rtt = connection.rtt;
        status.saveData = connection.saveData;
      }
    }

    return status;
  }

  /**
   * Check if connection is slow (2G or slow-2G)
   */
  isSlowConnection(): boolean {
    const status = this.networkStatus$.value;
    return status.effectiveType === '2g' || status.effectiveType === 'slow-2g';
  }

  /**
   * Check if data saver mode is enabled
   */
  isDataSaverEnabled(): boolean {
    return this.networkStatus$.value.saveData === true;
  }

  /**
   * Wait for online status
   * Resolves immediately if already online, or waits for next online event
   */
  waitForOnline(): Promise<void> {
    if (this.isOnline) {
      return Promise.resolve();
    }

    return new Promise((resolve) => {
      const subscription = this.status.subscribe((status) => {
        if (status.online) {
          subscription.unsubscribe();
          resolve();
        }
      });
    });
  }

  /**
   * Execute a function when online, queue if offline
   */
  async executeWhenOnline<T>(fn: () => Promise<T>): Promise<T> {
    await this.waitForOnline();
    return fn();
  }
}
