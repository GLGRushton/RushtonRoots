import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PwaService } from '../../services/pwa.service';
import { ServiceWorkerUpdate, UpdatePromptOptions } from '../../models/pwa.model';

/**
 * Update Prompt Component - Prompts users to update to the latest version
 */
@Component({
  selector: 'app-update-prompt',
  standalone: false,
  templateUrl: './update-prompt.component.html',
  styleUrls: ['./update-prompt.component.scss']
})
export class UpdatePromptComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  @Input() options: UpdatePromptOptions = {
    message: 'A new version of RushtonRoots is available!',
    updateButtonText: 'Update Now',
    dismissButtonText: 'Later',
    force: false
  };

  updateAvailable: ServiceWorkerUpdate | null = null;
  showPrompt = false;
  updating = false;

  constructor(private pwaService: PwaService) {}

  ngOnInit(): void {
    this.pwaService.onUpdateAvailable
      .pipe(takeUntil(this.destroy$))
      .subscribe((update) => {
        if (update && update.type === 'available') {
          this.updateAvailable = update;
          this.showPrompt = true;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Handle update button click
   */
  async onUpdate(): Promise<void> {
    this.updating = true;

    try {
      await this.pwaService.activateUpdate();
      // Page will reload automatically after activation
    } catch (error) {
      console.error('Failed to activate update:', error);
      this.updating = false;
    }
  }

  /**
   * Handle dismiss button click
   */
  onDismiss(): void {
    if (!this.options.force) {
      this.showPrompt = false;
    }
  }
}
