import { Component, Input } from '@angular/core';

/**
 * LoadingSpinnerComponent - Consistent loading states
 * 
 * Usage:
 * <app-loading-spinner 
 *   [size]="'medium'"
 *   [message]="'Loading...'"
 *   [overlay]="true">
 * </app-loading-spinner>
 */
@Component({
  selector: 'app-loading-spinner',
  standalone: false,
  templateUrl: './loading-spinner.component.html',
  styleUrls: ['./loading-spinner.component.scss']
})
export class LoadingSpinnerComponent {
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() message?: string;
  @Input() overlay: boolean = false;

  get diameter(): number {
    switch (this.size) {
      case 'small': return 30;
      case 'large': return 80;
      default: return 50;
    }
  }
}
