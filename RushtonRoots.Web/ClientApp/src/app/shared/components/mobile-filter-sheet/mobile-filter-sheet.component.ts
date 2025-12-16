// Mobile Filter Sheet Component - Bottom sheet for filters on mobile
import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatBottomSheetRef, MAT_BOTTOM_SHEET_DATA } from '@angular/material/bottom-sheet';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';

export interface FilterOption {
  id: string;
  label: string;
  type: 'checkbox' | 'select' | 'text' | 'date' | 'range';
  value?: any;
  options?: { value: any; label: string }[];
  placeholder?: string;
}

export interface FilterSheetData {
  title: string;
  filters: FilterOption[];
  activeFilters?: { [key: string]: any };
}

@Component({
  selector: 'app-mobile-filter-sheet',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatChipsModule,
    MatIconModule,
    MatDividerModule
  ],
  template: `
    <div class="filter-sheet">
      <div class="filter-sheet__header">
        <h2>{{ data.title }}</h2>
        <button 
          mat-icon-button 
          (click)="close()"
          class="filter-sheet__close">
          <mat-icon>close</mat-icon>
        </button>
      </div>
      
      <mat-divider></mat-divider>
      
      <div class="filter-sheet__content">
        <div class="filter-sheet__active" *ngIf="getActiveFilterCount() > 0">
          <div class="filter-chips">
            <mat-chip-set>
              <mat-chip 
                *ngFor="let key of getActiveFilterKeys()"
                (removed)="removeFilter(key)"
                class="filter-chip">
                {{ getFilterLabel(key) }}: {{ getFilterDisplayValue(key) }}
                <button matChipRemove>
                  <mat-icon>cancel</mat-icon>
                </button>
              </mat-chip>
            </mat-chip-set>
          </div>
          <button 
            mat-button 
            color="warn" 
            (click)="clearAll()"
            class="filter-sheet__clear">
            Clear All
          </button>
        </div>
        
        <div class="filter-sheet__filters">
          <div 
            *ngFor="let filter of data.filters" 
            class="filter-item">
            
            <!-- Checkbox Filter -->
            <mat-checkbox 
              *ngIf="filter.type === 'checkbox'"
              [(ngModel)]="filterValues[filter.id]"
              class="filter-checkbox">
              {{ filter.label }}
            </mat-checkbox>
            
            <!-- Select Filter -->
            <mat-form-field *ngIf="filter.type === 'select'" class="filter-field">
              <mat-label>{{ filter.label }}</mat-label>
              <mat-select [(ngModel)]="filterValues[filter.id]">
                <mat-option [value]="null">All</mat-option>
                <mat-option 
                  *ngFor="let option of filter.options" 
                  [value]="option.value">
                  {{ option.label }}
                </mat-option>
              </mat-select>
            </mat-form-field>
            
            <!-- Text Filter -->
            <mat-form-field *ngIf="filter.type === 'text'" class="filter-field">
              <mat-label>{{ filter.label }}</mat-label>
              <input 
                matInput 
                [(ngModel)]="filterValues[filter.id]"
                [placeholder]="filter.placeholder || ''">
            </mat-form-field>
            
            <!-- Date Filter -->
            <mat-form-field *ngIf="filter.type === 'date'" class="filter-field">
              <mat-label>{{ filter.label }}</mat-label>
              <input 
                matInput 
                type="date"
                [(ngModel)]="filterValues[filter.id]">
            </mat-form-field>
            
            <!-- Range Filter (TODO: implement range slider) -->
            <div *ngIf="filter.type === 'range'" class="filter-range">
              <label>{{ filter.label }}</label>
              <div class="range-inputs">
                <mat-form-field class="range-field">
                  <mat-label>Min</mat-label>
                  <input 
                    matInput 
                    type="number"
                    [(ngModel)]="filterValues[filter.id + '_min']">
                </mat-form-field>
                <span class="range-separator">to</span>
                <mat-form-field class="range-field">
                  <mat-label>Max</mat-label>
                  <input 
                    matInput 
                    type="number"
                    [(ngModel)]="filterValues[filter.id + '_max']">
                </mat-form-field>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="filter-sheet__footer">
        <button 
          mat-stroked-button 
          (click)="reset()"
          class="filter-sheet__reset">
          Reset
        </button>
        <button 
          mat-raised-button 
          color="primary"
          (click)="apply()"
          class="filter-sheet__apply">
          Apply Filters ({{ getActiveFilterCount() }})
        </button>
      </div>
    </div>
  `,
  styles: [`
    .filter-sheet {
      display: flex;
      flex-direction: column;
      max-height: 80vh;
      padding-bottom: env(safe-area-inset-bottom);
    }
    
    .filter-sheet__header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 16px 24px;
      
      h2 {
        margin: 0;
        font-size: 20px;
        font-weight: 500;
      }
    }
    
    .filter-sheet__close {
      margin-right: -12px;
    }
    
    .filter-sheet__content {
      flex: 1;
      overflow-y: auto;
      padding: 16px 24px;
    }
    
    .filter-sheet__active {
      margin-bottom: 16px;
      padding-bottom: 16px;
      border-bottom: 1px solid rgba(0, 0, 0, 0.12);
      
      .filter-chips {
        margin-bottom: 8px;
      }
      
      .filter-chip {
        margin-right: 8px;
        margin-bottom: 8px;
      }
    }
    
    .filter-sheet__clear {
      width: 100%;
    }
    
    .filter-sheet__filters {
      .filter-item {
        margin-bottom: 16px;
      }
      
      .filter-checkbox {
        display: block;
        min-height: 48px;
      }
      
      .filter-field {
        width: 100%;
      }
      
      .filter-range {
        label {
          display: block;
          margin-bottom: 8px;
          font-size: 14px;
          color: rgba(0, 0, 0, 0.6);
        }
        
        .range-inputs {
          display: flex;
          align-items: center;
          gap: 8px;
          
          .range-field {
            flex: 1;
          }
          
          .range-separator {
            color: rgba(0, 0, 0, 0.6);
          }
        }
      }
    }
    
    .filter-sheet__footer {
      display: flex;
      gap: 12px;
      padding: 16px 24px;
      border-top: 1px solid rgba(0, 0, 0, 0.12);
      
      button {
        flex: 1;
        min-height: 48px;
      }
    }
  `]
})
export class MobileFilterSheetComponent {
  
  filterValues: { [key: string]: any } = {};
  
  constructor(
    private bottomSheetRef: MatBottomSheetRef<MobileFilterSheetComponent>,
    @Inject(MAT_BOTTOM_SHEET_DATA) public data: FilterSheetData
  ) {
    // Initialize filter values from active filters
    this.filterValues = { ...data.activeFilters } || {};
  }
  
  getActiveFilterKeys(): string[] {
    return Object.keys(this.filterValues).filter(key => {
      const value = this.filterValues[key];
      return value !== null && value !== undefined && value !== '' && value !== false;
    });
  }
  
  getActiveFilterCount(): number {
    return this.getActiveFilterKeys().length;
  }
  
  getFilterLabel(key: string): string {
    const filter = this.data.filters.find(f => 
      f.id === key || key.startsWith(f.id)
    );
    return filter?.label || key;
  }
  
  getFilterDisplayValue(key: string): string {
    const value = this.filterValues[key];
    const filter = this.data.filters.find(f => f.id === key);
    
    if (filter?.type === 'select' && filter.options) {
      const option = filter.options.find(o => o.value === value);
      return option?.label || value;
    }
    
    if (filter?.type === 'checkbox') {
      return value ? 'Yes' : 'No';
    }
    
    return value?.toString() || '';
  }
  
  removeFilter(key: string): void {
    delete this.filterValues[key];
  }
  
  clearAll(): void {
    this.filterValues = {};
  }
  
  reset(): void {
    this.clearAll();
  }
  
  apply(): void {
    // Filter out null/undefined/empty values
    const activeFilters = Object.keys(this.filterValues).reduce((acc, key) => {
      const value = this.filterValues[key];
      if (value !== null && value !== undefined && value !== '' && value !== false) {
        acc[key] = value;
      }
      return acc;
    }, {} as { [key: string]: any });
    
    this.bottomSheetRef.dismiss(activeFilters);
  }
  
  close(): void {
    this.bottomSheetRef.dismiss();
  }
}
