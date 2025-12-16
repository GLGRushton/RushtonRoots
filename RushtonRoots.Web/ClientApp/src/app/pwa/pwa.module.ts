import { NgModule, isDevMode } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServiceWorkerModule } from '@angular/service-worker';

// Material imports
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

// Components
import { InstallPromptComponent } from './components/install-prompt/install-prompt.component';
import { OfflineIndicatorComponent } from './components/offline-indicator/offline-indicator.component';
import { UpdatePromptComponent } from './components/update-prompt/update-prompt.component';
import { NotificationPromptComponent } from './components/notification-prompt/notification-prompt.component';

// Services
import { PwaService } from './services/pwa.service';
import { InstallPromptService } from './services/install-prompt.service';
import { NetworkStatusService } from './services/network-status.service';
import { BackgroundSyncService } from './services/background-sync.service';
import { PushNotificationService } from './services/push-notification.service';

/**
 * PWA Module - Progressive Web App Features
 * Phase 9.2: Offline support, install prompts, push notifications
 */
@NgModule({
  declarations: [
    InstallPromptComponent,
    OfflineIndicatorComponent,
    UpdatePromptComponent,
    NotificationPromptComponent
  ],
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: !isDevMode(), // Only enable in production builds
      // Register the ServiceWorker as soon as the application is stable
      // or after 30 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:30000'
    })
  ],
  providers: [
    PwaService,
    InstallPromptService,
    NetworkStatusService,
    BackgroundSyncService,
    PushNotificationService
  ],
  exports: [
    InstallPromptComponent,
    OfflineIndicatorComponent,
    UpdatePromptComponent,
    NotificationPromptComponent
  ]
})
export class PwaModule { }
