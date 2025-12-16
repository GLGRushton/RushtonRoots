import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { PushNotificationPayload, PushSubscriptionState } from '../models/pwa.model';

/**
 * Push Notification Service for managing push notifications
 */
@Injectable({
  providedIn: 'root'
})
export class PushNotificationService {
  private subscriptionState$ = new BehaviorSubject<PushSubscriptionState>({
    subscribed: false,
    subscription: null,
    permission: 'default'
  });

  // VAPID public key - should be generated and stored securely
  // This is a placeholder - replace with actual VAPID public key from server
  private readonly VAPID_PUBLIC_KEY = 'YOUR_VAPID_PUBLIC_KEY_HERE';

  constructor() {
    this.initializeNotifications();
  }

  /**
   * Get observable for subscription state
   */
  get state(): Observable<PushSubscriptionState> {
    return this.subscriptionState$.asObservable();
  }

  /**
   * Get current subscription state
   */
  get currentState(): PushSubscriptionState {
    return this.subscriptionState$.value;
  }

  /**
   * Check if push notifications are supported
   */
  get isSupported(): boolean {
    return 'PushManager' in window && 'Notification' in window && 'serviceWorker' in navigator;
  }

  /**
   * Initialize push notifications
   */
  private async initializeNotifications(): Promise<void> {
    if (!this.isSupported) {
      console.log('Push notifications are not supported');
      return;
    }

    // Get current permission status
    const permission = Notification.permission;
    this.updatePermission(permission);

    // Check existing subscription
    await this.checkExistingSubscription();
  }

  /**
   * Check for existing push subscription
   */
  private async checkExistingSubscription(): Promise<void> {
    if (!('serviceWorker' in navigator)) {
      return;
    }

    try {
      const registration = await navigator.serviceWorker.ready;
      const subscription = await registration.pushManager.getSubscription();

      if (subscription) {
        this.subscriptionState$.next({
          subscribed: true,
          subscription,
          permission: Notification.permission
        });
      }
    } catch (error) {
      console.error('Error checking subscription:', error);
    }
  }

  /**
   * Request notification permission
   */
  async requestPermission(): Promise<NotificationPermission> {
    if (!this.isSupported) {
      return 'denied';
    }

    const permission = await Notification.requestPermission();
    this.updatePermission(permission);
    return permission;
  }

  /**
   * Subscribe to push notifications
   */
  async subscribe(): Promise<PushSubscription | null> {
    if (!this.isSupported) {
      console.error('Push notifications are not supported');
      return null;
    }

    // Request permission first if not granted
    if (Notification.permission !== 'granted') {
      const permission = await this.requestPermission();
      if (permission !== 'granted') {
        console.log('Notification permission denied');
        return null;
      }
    }

    try {
      const registration = await navigator.serviceWorker.ready;

      // Convert VAPID key to Uint8Array
      const convertedVapidKey = this.urlBase64ToUint8Array(this.VAPID_PUBLIC_KEY);

      const subscription = await registration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: convertedVapidKey
      });

      this.subscriptionState$.next({
        subscribed: true,
        subscription,
        permission: Notification.permission
      });

      console.log('Push notification subscription successful:', subscription);

      // Send subscription to server
      await this.sendSubscriptionToServer(subscription);

      return subscription;
    } catch (error) {
      console.error('Failed to subscribe to push notifications:', error);
      return null;
    }
  }

  /**
   * Unsubscribe from push notifications
   */
  async unsubscribe(): Promise<boolean> {
    const currentState = this.subscriptionState$.value;

    if (!currentState.subscription) {
      return false;
    }

    try {
      const success = await currentState.subscription.unsubscribe();

      if (success) {
        this.subscriptionState$.next({
          subscribed: false,
          subscription: null,
          permission: Notification.permission
        });

        // Notify server about unsubscription
        await this.removeSubscriptionFromServer(currentState.subscription);

        console.log('Unsubscribed from push notifications');
      }

      return success;
    } catch (error) {
      console.error('Failed to unsubscribe from push notifications:', error);
      return false;
    }
  }

  /**
   * Show a local notification (not push)
   */
  async showNotification(payload: PushNotificationPayload): Promise<void> {
    if (!this.isSupported) {
      return;
    }

    if (Notification.permission !== 'granted') {
      console.warn('Notification permission not granted');
      return;
    }

    try {
      const registration = await navigator.serviceWorker.ready;

      await registration.showNotification(payload.title, {
        body: payload.body,
        icon: payload.icon || '/assets/icons/icon-192x192.png',
        badge: payload.badge || '/assets/icons/icon-72x72.png',
        tag: payload.tag,
        data: payload.data,
        requireInteraction: payload.requireInteraction,
        actions: payload.actions
      } as any); // Use 'as any' to bypass TypeScript image property issue

      console.log('Notification shown:', payload.title);
    } catch (error) {
      console.error('Failed to show notification:', error);
    }
  }

  /**
   * Update permission state
   */
  private updatePermission(permission: NotificationPermission): void {
    this.subscriptionState$.next({
      ...this.subscriptionState$.value,
      permission
    });
  }

  /**
   * Convert VAPID key from base64 to Uint8Array
   */
  private urlBase64ToUint8Array(base64String: string): Uint8Array {
    const padding = '='.repeat((4 - (base64String.length % 4)) % 4);
    const base64 = (base64String + padding).replace(/-/g, '+').replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
      outputArray[i] = rawData.charCodeAt(i);
    }

    return outputArray;
  }

  /**
   * Send subscription to server
   * This should be implemented based on your backend API
   */
  private async sendSubscriptionToServer(subscription: PushSubscription): Promise<void> {
    try {
      // TODO: Implement actual API call to save subscription on server
      console.log('Subscription to be sent to server:', subscription.toJSON());
      
      // Example:
      // await fetch('/api/notifications/subscribe', {
      //   method: 'POST',
      //   headers: { 'Content-Type': 'application/json' },
      //   body: JSON.stringify(subscription.toJSON())
      // });
    } catch (error) {
      console.error('Failed to send subscription to server:', error);
    }
  }

  /**
   * Remove subscription from server
   */
  private async removeSubscriptionFromServer(subscription: PushSubscription): Promise<void> {
    try {
      // TODO: Implement actual API call to remove subscription from server
      console.log('Subscription to be removed from server:', subscription.toJSON());

      // Example:
      // await fetch('/api/notifications/unsubscribe', {
      //   method: 'POST',
      //   headers: { 'Content-Type': 'application/json' },
      //   body: JSON.stringify(subscription.toJSON())
      // });
    } catch (error) {
      console.error('Failed to remove subscription from server:', error);
    }
  }

  /**
   * Check if user has granted notification permission
   */
  hasPermission(): boolean {
    return this.subscriptionState$.value.permission === 'granted';
  }

  /**
   * Check if user is subscribed to push notifications
   */
  isSubscribed(): boolean {
    return this.subscriptionState$.value.subscribed;
  }
}
