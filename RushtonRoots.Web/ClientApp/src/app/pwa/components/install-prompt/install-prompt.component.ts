import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { InstallPromptService } from '../../services/install-prompt.service';
import { InstallPromptState, InstallInstructions } from '../../models/pwa.model';

/**
 * Install Prompt Component - Prompts users to install the PWA
 */
@Component({
  selector: 'app-install-prompt',
  templateUrl: './install-prompt.component.html',
  styleUrls: ['./install-prompt.component.scss']
})
export class InstallPromptComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  promptState: InstallPromptState | null = null;
  showBanner = false;
  showInstructions = false;
  instructions: InstallInstructions | null = null;
  installing = false;

  constructor(private installPromptService: InstallPromptService) {}

  ngOnInit(): void {
    // Subscribe to install prompt state
    this.installPromptService.state
      .pipe(takeUntil(this.destroy$))
      .subscribe((state) => {
        this.promptState = state;
        
        // Show banner if can install and not already shown
        if (state.canInstall && !this.hasUserDismissedPrompt()) {
          this.showBanner = true;
        }

        // Hide banner if installed
        if (state.isInstalled) {
          this.showBanner = false;
        }
      });

    // Get platform-specific instructions
    this.instructions = this.installPromptService.getInstallInstructions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Handle install button click
   */
  async onInstall(): Promise<void> {
    if (!this.promptState?.canInstall) {
      // Show manual instructions if prompt not available
      this.showInstructions = true;
      return;
    }

    this.installing = true;

    try {
      const outcome = await this.installPromptService.showInstallPrompt();

      if (outcome === 'accepted') {
        console.log('User accepted the install prompt');
        this.showBanner = false;
      } else if (outcome === 'dismissed') {
        console.log('User dismissed the install prompt');
        this.showBanner = false;
        this.markUserDismissedPrompt();
      } else {
        // Prompt not available, show instructions
        this.showInstructions = true;
      }
    } finally {
      this.installing = false;
    }
  }

  /**
   * Handle dismiss button click
   */
  onDismiss(): void {
    this.showBanner = false;
    this.markUserDismissedPrompt();
  }

  /**
   * Toggle instructions display
   */
  toggleInstructions(): void {
    this.showInstructions = !this.showInstructions;
  }

  /**
   * Check if user has previously dismissed the prompt
   */
  private hasUserDismissedPrompt(): boolean {
    try {
      return localStorage.getItem('install_prompt_dismissed') === 'true';
    } catch {
      return false;
    }
  }

  /**
   * Mark that user has dismissed the prompt (expires after 7 days)
   */
  private markUserDismissedPrompt(): void {
    try {
      localStorage.setItem('install_prompt_dismissed', 'true');
      // Clear after 7 days
      setTimeout(() => {
        localStorage.removeItem('install_prompt_dismissed');
      }, 7 * 24 * 60 * 60 * 1000);
    } catch (error) {
      console.error('Failed to save dismiss state:', error);
    }
  }
}
