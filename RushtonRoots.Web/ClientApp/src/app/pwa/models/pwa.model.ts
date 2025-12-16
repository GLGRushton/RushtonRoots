/**
 * PWA Models for Progressive Web App Features
 * Phase 9.2: Progressive Web App Features
 */

/**
 * Installation prompt state
 */
export interface InstallPromptEvent extends Event {
  readonly platforms: string[];
  readonly userChoice: Promise<{
    outcome: 'accepted' | 'dismissed';
    platform: string;
  }>;
  prompt(): Promise<void>;
}

/**
 * Install prompt state
 */
export interface InstallPromptState {
  canInstall: boolean;
  isInstalled: boolean;
  deferredPrompt: InstallPromptEvent | null;
}

/**
 * Network connection status
 */
export interface NetworkStatus {
  online: boolean;
  effectiveType?: '2g' | '3g' | '4g' | 'slow-2g';
  downlink?: number;
  rtt?: number;
  saveData?: boolean;
}

/**
 * Offline indicator configuration
 */
export interface OfflineIndicatorConfig {
  position: 'top' | 'bottom';
  message: string;
  showRetry: boolean;
  autoHide: boolean;
  hideDelay: number;
}

/**
 * Background sync registration
 */
export interface BackgroundSyncRegistration {
  tag: string;
  data: any;
  timestamp: number;
  status: 'pending' | 'syncing' | 'completed' | 'failed';
  retryCount: number;
}

/**
 * Push notification payload
 */
export interface PushNotificationPayload {
  title: string;
  body: string;
  icon?: string;
  badge?: string;
  image?: string;
  tag?: string;
  data?: any;
  requireInteraction?: boolean;
  actions?: NotificationAction[];
}

/**
 * Notification action
 */
export interface NotificationAction {
  action: string;
  title: string;
  icon?: string;
}

/**
 * Push subscription state
 */
export interface PushSubscriptionState {
  subscribed: boolean;
  subscription: PushSubscription | null;
  permission: NotificationPermission;
}

/**
 * Service worker update state
 */
export interface ServiceWorkerUpdate {
  type: 'available' | 'activated';
  current: string;
  available?: string;
}

/**
 * PWA installation instructions by platform
 */
export interface InstallInstructions {
  platform: 'ios' | 'android' | 'desktop';
  browser: string;
  steps: string[];
  icon: string;
}

/**
 * Form data for background sync
 */
export interface SyncableFormData {
  formId: string;
  formType: string;
  data: any;
  url: string;
  method: 'POST' | 'PUT' | 'PATCH';
  timestamp: number;
}

/**
 * App update prompt options
 */
export interface UpdatePromptOptions {
  message: string;
  updateButtonText: string;
  dismissButtonText: string;
  force: boolean;
}

/**
 * PWA feature support detection
 */
export interface PWAFeatureSupport {
  serviceWorker: boolean;
  pushNotifications: boolean;
  backgroundSync: boolean;
  installPrompt: boolean;
  periodicBackgroundSync: boolean;
  badging: boolean;
  webShare: boolean;
}
