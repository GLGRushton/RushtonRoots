import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { NetworkStatusService } from '../../services/network-status.service';
import { NetworkStatus, OfflineIndicatorConfig } from '../../models/pwa.model';

/**
 * Offline Indicator Component - Shows online/offline status
 */
@Component({
  selector: 'app-offline-indicator',
  templateUrl: './offline-indicator.component.html',
  styleUrls: ['./offline-indicator.component.scss']
})
export class OfflineIndicatorComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  @Input() config: OfflineIndicatorConfig = {
    position: 'top',
    message: 'You are offline. Some features may be unavailable.',
    showRetry: true,
    autoHide: false,
    hideDelay: 5000
  };

  networkStatus: NetworkStatus | null = null;
  show = false;
  retrying = false;

  constructor(private networkStatusService: NetworkStatusService) {}

  ngOnInit(): void {
    this.networkStatusService.status
      .pipe(takeUntil(this.destroy$))
      .subscribe((status) => {
        this.networkStatus = status;
        this.updateVisibility(status);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Update indicator visibility based on network status
   */
  private updateVisibility(status: NetworkStatus): void {
    if (!status.online) {
      // Offline - show indicator
      this.show = true;
    } else if (this.show) {
      // Just came back online
      if (this.config.autoHide) {
        // Hide after delay
        setTimeout(() => {
          this.show = false;
        }, this.config.hideDelay);
      } else {
        // Hide immediately
        this.show = false;
      }
    }
  }

  /**
   * Retry connection
   */
  async onRetry(): Promise<void> {
    this.retrying = true;

    try {
      // Try to fetch a small resource to check connectivity
      await fetch('/favicon.ico', { method: 'HEAD', cache: 'no-cache' });
      
      // If successful, we're online
      console.log('Connection restored');
      this.show = false;
    } catch (error) {
      // Still offline
      console.log('Still offline');
    } finally {
      this.retrying = false;
    }
  }

  /**
   * Manually dismiss the indicator
   */
  onDismiss(): void {
    this.show = false;
  }

  /**
   * Get connection quality message
   */
  getConnectionMessage(): string {
    if (!this.networkStatus) {
      return this.config.message;
    }

    if (!this.networkStatus.online) {
      return this.config.message;
    }

    if (this.networkStatus.effectiveType === 'slow-2g' || this.networkStatus.effectiveType === '2g') {
      return 'Slow connection detected. Some features may load slowly.';
    }

    if (this.networkStatus.saveData) {
      return 'Data saver mode is active. Some content may not load automatically.';
    }

    return 'Connection restored!';
  }

  /**
   * Get indicator color based on status
   */
  getIndicatorClass(): string {
    if (!this.networkStatus?.online) {
      return 'offline';
    }

    if (this.networkStatus.effectiveType === 'slow-2g' || this.networkStatus.effectiveType === '2g') {
      return 'slow';
    }

    return 'online';
  }
}
