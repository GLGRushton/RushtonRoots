import { Component, Input } from '@angular/core';

export interface BreadcrumbItem {
  label: string;
  url?: string;
  icon?: string;
}

/**
 * BreadcrumbComponent - Navigation breadcrumbs
 * 
 * Usage:
 * <app-breadcrumb 
 *   [items]="[
 *     {label: 'Home', url: '/', icon: 'home'},
 *     {label: 'People', url: '/Person'},
 *     {label: 'John Doe'}
 *   ]">
 * </app-breadcrumb>
 */
@Component({
  selector: 'app-breadcrumb',
  standalone: false,
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent {
  @Input() items: BreadcrumbItem[] = [];

  isLast(index: number): boolean {
    return index === this.items.length - 1;
  }

  navigate(url?: string): void {
    if (url) {
      window.location.href = url;
    }
  }
}
