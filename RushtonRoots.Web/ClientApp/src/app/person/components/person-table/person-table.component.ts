import { Component, Input, Output, EventEmitter, ViewChild, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { SelectionModel } from '@angular/cdk/collections';

export interface PersonTableRow {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  householdName: string;
  dateOfBirth?: Date | string;
  dateOfDeath?: Date | string;
  isDeceased: boolean;
  photoUrl?: string;
}

export interface PersonAction {
  type: 'view' | 'edit' | 'delete';
  personId: number;
  person: PersonTableRow;
}

/**
 * PersonTableComponent - Advanced table for displaying people with sorting, pagination, and actions
 * 
 * Features:
 * - Material table with sorting and pagination
 * - Row selection (optional)
 * - Quick action buttons (view, edit, delete)
 * - Responsive design (converts to cards on mobile)
 * - Export functionality
 */
@Component({
  selector: 'app-person-table',
  standalone: false,
  templateUrl: './person-table.component.html',
  styleUrls: ['./person-table.component.scss']
})
export class PersonTableComponent implements OnInit {
  @Input() set people(value: PersonTableRow[]) {
    this.dataSource.data = value;
  }
  @Input() showPagination = true;
  @Input() pageSize = 10;
  @Input() pageSizeOptions = [5, 10, 25, 50, 100];
  @Input() showActions = true;
  @Input() showSelection = false;
  @Input() canEdit = false;
  @Input() canDelete = false;
  
  @Output() actionTriggered = new EventEmitter<PersonAction>();
  @Output() pageChanged = new EventEmitter<PageEvent>();
  @Output() sortChanged = new EventEmitter<Sort>();
  @Output() selectionChanged = new EventEmitter<PersonTableRow[]>();

  displayedColumns: string[] = [];
  dataSource = new MatTableDataSource<PersonTableRow>([]);
  selection = new SelectionModel<PersonTableRow>(true, []);

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

  ngOnInit(): void {
    this.updateDisplayedColumns();
  }

  updateDisplayedColumns(): void {
    const columns = [];
    if (this.showSelection) columns.push('select');
    columns.push('fullName', 'householdName', 'dateOfBirth', 'status');
    if (this.showActions) columns.push('actions');
    this.displayedColumns = columns;
  }

  formatDate(date?: Date | string): string {
    if (!date) return 'N/A';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString();
  }

  onView(person: PersonTableRow): void {
    this.actionTriggered.emit({ type: 'view', personId: person.id, person });
    // Using window.location.href because this component is used as an Angular Element
    // within a traditional MVC application (not a full SPA)
    window.location.href = `/Person/Details/${person.id}`;
  }

  onEdit(person: PersonTableRow): void {
    this.actionTriggered.emit({ type: 'edit', personId: person.id, person });
    // Using window.location.href because this component is used as an Angular Element
    // within a traditional MVC application (not a full SPA)
    window.location.href = `/Person/Edit/${person.id}`;
  }

  onDelete(person: PersonTableRow): void {
    this.actionTriggered.emit({ type: 'delete', personId: person.id, person });
  }

  onPageChange(event: PageEvent): void {
    this.pageChanged.emit(event);
  }

  onSortChange(event: Sort): void {
    this.sortChanged.emit(event);
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  toggleAllRows(): void {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.dataSource.data.forEach(row => this.selection.select(row));
    }
    this.selectionChanged.emit(this.selection.selected);
  }

  /** The label for the checkbox on the passed row */
  checkboxLabel(row?: PersonTableRow): string {
    if (!row) {
      return `${this.isAllSelected() ? 'deselect' : 'select'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.id}`;
  }

  onSelectionToggle(row: PersonTableRow): void {
    this.selection.toggle(row);
    this.selectionChanged.emit(this.selection.selected);
  }

  exportToCSV(): void {
    const data = this.dataSource.data;
    const csvContent = this.convertToCSV(data);
    this.downloadFile(csvContent, 'people.csv', 'text/csv');
  }

  private convertToCSV(data: PersonTableRow[]): string {
    const headers = ['ID', 'First Name', 'Last Name', 'Household', 'Date of Birth', 'Date of Death', 'Status'];
    const rows = data.map(person => [
      person.id,
      person.firstName,
      person.lastName,
      person.householdName,
      this.formatDate(person.dateOfBirth),
      person.isDeceased ? this.formatDate(person.dateOfDeath) : '',
      person.isDeceased ? 'Deceased' : 'Living'
    ]);

    const csvRows = [headers, ...rows];
    return csvRows.map(row => row.map(cell => `"${cell}"`).join(',')).join('\n');
  }

  private downloadFile(content: string, filename: string, mimeType: string): void {
    const blob = new Blob([content], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}
