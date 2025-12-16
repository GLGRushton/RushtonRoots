// Mobile Action Sheet Component - Bottom sheet for mobile actions
import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatBottomSheetRef, MAT_BOTTOM_SHEET_DATA } from '@angular/material/bottom-sheet';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';

export interface ActionSheetAction {
  icon: string;
  label: string;
  value: string;
  color?: 'primary' | 'accent' | 'warn';
  disabled?: boolean;
  divider?: boolean; // Show divider after this item
}

export interface ActionSheetData {
  title?: string;
  actions: ActionSheetAction[];
  cancelLabel?: string;
}

@Component({
  selector: 'app-mobile-action-sheet',
  standalone: true,
  imports: [
    CommonModule,
    MatListModule,
    MatIconModule,
    MatDividerModule
  ],
  template: `
    <div class="action-sheet">
      <div class="action-sheet__header" *ngIf="data.title">
        <h3>{{ data.title }}</h3>
      </div>
      
      <mat-nav-list class="action-sheet__actions">
        <ng-container *ngFor="let action of data.actions">
          <button 
            mat-list-item 
            (click)="selectAction(action)"
            [disabled]="action.disabled"
            [class.action-primary]="action.color === 'primary'"
            [class.action-accent]="action.color === 'accent'"
            [class.action-warn]="action.color === 'warn'"
            class="action-sheet__item">
            <mat-icon matListItemIcon>{{ action.icon }}</mat-icon>
            <span matListItemTitle>{{ action.label }}</span>
          </button>
          <mat-divider *ngIf="action.divider"></mat-divider>
        </ng-container>
      </mat-nav-list>
      
      <div class="action-sheet__cancel">
        <button 
          mat-list-item 
          (click)="cancel()"
          class="action-sheet__cancel-btn">
          <span matListItemTitle>{{ data.cancelLabel || 'Cancel' }}</span>
        </button>
      </div>
    </div>
  `,
  styles: [`
    .action-sheet {
      padding-bottom: env(safe-area-inset-bottom);
    }
    
    .action-sheet__header {
      padding: 16px 24px 8px;
      text-align: center;
      
      h3 {
        margin: 0;
        font-size: 14px;
        font-weight: 500;
        color: rgba(0, 0, 0, 0.6);
        text-transform: uppercase;
        letter-spacing: 0.5px;
      }
    }
    
    .action-sheet__actions {
      padding: 0 !important;
    }
    
    .action-sheet__item {
      min-height: 56px !important;
      padding: 0 24px !important;
      transition: background-color 0.2s;
      
      &:active {
        background-color: rgba(0, 0, 0, 0.05);
      }
      
      mat-icon {
        color: rgba(0, 0, 0, 0.54);
        margin-right: 16px;
      }
      
      &.action-primary mat-icon {
        color: #2e7d32;
      }
      
      &.action-accent mat-icon {
        color: #66bb6a;
      }
      
      &.action-warn mat-icon {
        color: #d32f2f;
      }
      
      &[disabled] {
        opacity: 0.5;
        cursor: not-allowed;
      }
    }
    
    .action-sheet__cancel {
      margin-top: 8px;
      padding: 0;
      border-top: 1px solid rgba(0, 0, 0, 0.12);
    }
    
    .action-sheet__cancel-btn {
      min-height: 56px !important;
      padding: 0 24px !important;
      font-weight: 500;
      
      &:active {
        background-color: rgba(0, 0, 0, 0.05);
      }
    }
    
    mat-divider {
      margin: 0 24px;
    }
  `]
})
export class MobileActionSheetComponent {
  
  constructor(
    private bottomSheetRef: MatBottomSheetRef<MobileActionSheetComponent>,
    @Inject(MAT_BOTTOM_SHEET_DATA) public data: ActionSheetData
  ) {}
  
  selectAction(action: ActionSheetAction): void {
    if (!action.disabled) {
      this.bottomSheetRef.dismiss(action.value);
    }
  }
  
  cancel(): void {
    this.bottomSheetRef.dismiss();
  }
}
