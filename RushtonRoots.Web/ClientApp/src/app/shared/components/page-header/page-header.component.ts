import { Component, Input } from '@angular/core';

export interface BreadcrumbItem {
  label: string;
  url?: string;
}

/**
 * PageHeaderComponent - Consistent page headers with title, breadcrumbs, and actions
 * 
 * Usage:
 * <app-page-header 
 *   [title]="'People'"
 *   [subtitle]="'Manage family members'"
 *   [breadcrumbs]="[{label: 'Home', url: '/'}, {label: 'People'}]"
 *   [showBackButton]="true">
 *   <div actions>
 *     <button mat-raised-button color="primary">Add Person</button>
 *   </div>
 * </app-page-header>
 */
@Component({
  selector: 'app-page-header',
  standalone: false,
  templateUrl: './page-header.component.html',
  styleUrls: ['./page-header.component.scss']
})
export class PageHeaderComponent {
  @Input() title: string = '';
  @Input() subtitle?: string;
  @Input() breadcrumbs: BreadcrumbItem[] = [];
  @Input() showBackButton: boolean = false;

  goBack(): void {
    window.history.back();
  }
}
