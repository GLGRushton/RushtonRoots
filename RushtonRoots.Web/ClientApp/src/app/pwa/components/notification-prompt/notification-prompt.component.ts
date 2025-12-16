import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PushNotificationService } from '../../services/push-notification.service';
import { PushSubscriptionState } from '../../models/pwa.model';

/**
 * Notification Prompt Component - Prompts users to enable push notifications
 */
@Component({
  selector: 'app-notification-prompt',
  templateUrl: './notification-prompt.component.html',
  styleUrls: ['./notification-prompt.component.scss']
})
export class NotificationPromptComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  subscriptionState: PushSubscriptionState | null = null;
  showPrompt = false;
  loading = false;

  constructor(private pushNotificationService: PushNotificationService) {}

  ngOnInit(): void {
    // Check if notifications are supported
    if (!this.pushNotificationService.isSupported) {
      return;
    }

    // Subscribe to subscription state
    this.pushNotificationService.state
      .pipe(takeUntil(this.destroy$))
      .subscribe((state) => {
        this.subscriptionState = state;

        // Show prompt if not subscribed and permission not denied
        if (!state.subscribed && state.permission === 'default' && !this.hasUserDismissedPrompt()) {
          // Show prompt after a delay to not overwhelm user
          setTimeout(() => {
            this.showPrompt = true;
          }, 5000);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Handle enable button click
   */
  async onEnable(): Promise<void> {
    this.loading = true;

    try {
      const subscription = await this.pushNotificationService.subscribe();

      if (subscription) {
        console.log('Push notifications enabled');
        this.showPrompt = false;
      } else {
        console.log('Failed to enable push notifications');
      }
    } finally {
      this.loading = false;
    }
  }

  /**
   * Handle dismiss button click
   */
  onDismiss(): void {
    this.showPrompt = false;
    this.markUserDismissedPrompt();
  }

  /**
   * Check if user has previously dismissed the prompt
   */
  private hasUserDismissedPrompt(): boolean {
    try {
      return localStorage.getItem('notification_prompt_dismissed') === 'true';
    } catch {
      return false;
    }
  }

  /**
   * Mark that user has dismissed the prompt (expires after 30 days)
   */
  private markUserDismissedPrompt(): void {
    try {
      localStorage.setItem('notification_prompt_dismissed', 'true');
      // Clear after 30 days
      setTimeout(() => {
        localStorage.removeItem('notification_prompt_dismissed');
      }, 30 * 24 * 60 * 60 * 1000);
    } catch (error) {
      console.error('Failed to save dismiss state:', error);
    }
  }

  /**
   * Get permission status message
   */
  getPermissionMessage(): string {
    if (!this.subscriptionState) {
      return '';
    }

    switch (this.subscriptionState.permission) {
      case 'granted':
        return 'Notifications are enabled';
      case 'denied':
        return 'Notifications are blocked. Please enable them in your browser settings.';
      default:
        return 'Stay updated with important family events and notifications.';
    }
  }
}
