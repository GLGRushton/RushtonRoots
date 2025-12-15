import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

export interface PersonListItem {
  id: number;
  firstName: string;
  lastName: string;
  birthDate?: string;
  deathDate?: string;
}

/**
 * PersonListComponent - Displays a list of people in a table format
 * 
 * Usage:
 * <app-person-list 
 *   [people]="personArray"
 *   [showPagination]="true"
 *   (personSelected)="onPersonSelected($event)">
 * </app-person-list>
 */
@Component({
  selector: 'app-person-list',
  standalone: false,
  templateUrl: './person-list.component.html',
  styleUrls: ['./person-list.component.scss']
})
export class PersonListComponent {
  @Input() set people(value: PersonListItem[]) {
    this.dataSource.data = value;
  }
  @Input() showPagination: boolean = true;
  @Output() personSelected = new EventEmitter<number>();

  displayedColumns: string[] = ['firstName', 'lastName', 'birthDate', 'deathDate', 'actions'];
  dataSource = new MatTableDataSource<PersonListItem>([]);

  @ViewChild(MatPaginator) set paginator(value: MatPaginator) {
    if (value) {
      this.dataSource.paginator = value;
    }
  }
  @ViewChild(MatSort) set sort(value: MatSort) {
    if (value) {
      this.dataSource.sort = value;
    }
  }

  formatDate(dateString?: string): string {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString();
  }

  onView(personId: number): void {
    this.personSelected.emit(personId);
    window.location.href = `/Person/Details/${personId}`;
  }

  onEdit(personId: number): void {
    window.location.href = `/Person/Edit/${personId}`;
  }
}
