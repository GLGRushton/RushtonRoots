import { Component, Input, Output, EventEmitter } from '@angular/core';
import { fadeInOut, slideInOut, rotate } from '../../animations';

export interface QuickAction {
  icon: string;
  label: string;
  action: string;
  color?: 'primary' | 'accent' | 'warn';
  requireRole?: string[];
}

/**
 * QuickActionsComponent - Floating Action Button with quick actions
 * 
 * Displays context-specific quick actions as a floating action button (FAB)
 * with expandable sub-actions.
 * 
 * Usage:
 * <app-quick-actions
 *   [actions]="quickActions"
 *   [visible]="true"
 *   (actionClicked)="handleQuickAction($event)">
 * </app-quick-actions>
 */
@Component({
  selector: 'app-quick-actions',
  standalone: false,
  templateUrl: './quick-actions.component.html',
  styleUrls: ['./quick-actions.component.scss'],
  animations: [fadeInOut, slideInOut, rotate]
})
export class QuickActionsComponent {
  @Input() actions: QuickAction[] = [];
  @Input() visible: boolean = true;
  @Input() userRoles: string[] = [];
  @Output() actionClicked = new EventEmitter<string>();

  isExpanded: boolean = false;

  /**
   * Toggle FAB expansion
   */
  toggleExpand(): void {
    this.isExpanded = !this.isExpanded;
  }

  /**
   * Handle action click
   */
  onActionClick(action: string): void {
    this.actionClicked.emit(action);
    this.isExpanded = false;
  }

  /**
   * Check if action is visible based on user roles
   */
  isActionVisible(action: QuickAction): boolean {
    if (!action.requireRole || action.requireRole.length === 0) {
      return true;
    }

    return action.requireRole.some(role => this.userRoles.includes(role));
  }

  /**
   * Get visible actions
   */
  get visibleActions(): QuickAction[] {
    return this.actions.filter(action => this.isActionVisible(action));
  }

  /**
   * Close FAB when clicking outside
   */
  onBackdropClick(): void {
    this.isExpanded = false;
  }
}
