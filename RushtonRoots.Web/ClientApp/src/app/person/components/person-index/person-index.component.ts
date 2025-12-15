import { Component, Input, OnInit } from '@angular/core';
import { PersonSearchFilters, HouseholdOption } from '../person-search/person-search.component';
import { PersonTableRow, PersonAction } from '../person-table/person-table.component';

/**
 * PersonIndexComponent - Main component for the Person Index page
 * 
 * Features:
 * - Integrates PersonSearchComponent and PersonTableComponent
 * - Handles search and filtering
 * - Export functionality
 * - Quick actions
 */
@Component({
  selector: 'app-person-index',
  standalone: false,
  templateUrl: './person-index.component.html',
  styleUrls: ['./person-index.component.scss']
})
export class PersonIndexComponent implements OnInit {
  @Input() initialPeople: PersonTableRow[] = [];
  @Input() households: HouseholdOption[] = [];
  @Input() canEdit = false;
  @Input() canDelete = false;
  @Input() initialFilters?: PersonSearchFilters;

  filteredPeople: PersonTableRow[] = [];
  allPeople: PersonTableRow[] = [];
  isLoading = false;
  errorMessage?: string;

  ngOnInit(): void {
    this.allPeople = this.initialPeople;
    this.filteredPeople = this.initialPeople;
  }

  onSearch(filters: PersonSearchFilters): void {
    // For now, we'll use client-side filtering
    // In a real app, you'd call an API with these filters
    this.filteredPeople = this.filterPeople(this.allPeople, filters);
  }

  private filterPeople(people: PersonTableRow[], filters: PersonSearchFilters): PersonTableRow[] {
    return people.filter(person => {
      // Search term filter (name)
      if (filters.searchTerm) {
        const searchLower = filters.searchTerm.toLowerCase();
        const nameMatch = person.fullName.toLowerCase().includes(searchLower) ||
                         person.firstName.toLowerCase().includes(searchLower) ||
                         person.lastName.toLowerCase().includes(searchLower);
        if (!nameMatch) return false;
      }

      // Household filter
      if (filters.householdId && person.householdName) {
        // This is a simplified check - in reality, you'd match by ID
        // For now, we don't have the household ID in PersonTableRow
        // This would need to be added or filtered server-side
      }

      // Deceased status filter
      if (filters.isDeceased !== undefined && filters.isDeceased !== null) {
        if (person.isDeceased !== filters.isDeceased) return false;
      }

      // Surname filter
      if (filters.surname) {
        const surnameLower = filters.surname.toLowerCase();
        if (!person.lastName.toLowerCase().includes(surnameLower)) return false;
      }

      // Birth date range filters
      if (filters.birthDateFrom && person.dateOfBirth) {
        const birthDate = new Date(person.dateOfBirth);
        const fromDate = new Date(filters.birthDateFrom);
        if (birthDate < fromDate) return false;
      }

      if (filters.birthDateTo && person.dateOfBirth) {
        const birthDate = new Date(person.dateOfBirth);
        const toDate = new Date(filters.birthDateTo);
        if (birthDate > toDate) return false;
      }

      // Death date range filters
      if (filters.deathDateFrom && person.dateOfDeath) {
        const deathDate = new Date(person.dateOfDeath);
        const fromDate = new Date(filters.deathDateFrom);
        if (deathDate < fromDate) return false;
      }

      if (filters.deathDateTo && person.dateOfDeath) {
        const deathDate = new Date(person.dateOfDeath);
        const toDate = new Date(filters.deathDateTo);
        if (deathDate > toDate) return false;
      }

      return true;
    });
  }

  onAction(action: PersonAction): void {
    if (action.type === 'delete') {
      // Handle delete confirmation
      if (confirm(`Are you sure you want to delete ${action.person.fullName}?`)) {
        // In a real app, you'd call an API to delete
        console.log('Delete person:', action.personId);
        window.location.href = `/Person/Delete/${action.personId}`;
      }
    }
  }

  onExportCSV(): void {
    // This will be handled by the PersonTableComponent
  }

  exportToCSV(): void {
    const csvContent = this.convertToCSV(this.filteredPeople);
    this.downloadFile(csvContent, 'people.csv', 'text/csv');
  }

  navigateToCreate(): void {
    window.location.href = '/Person/Create';
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

  private formatDate(date?: Date | string): string {
    if (!date) return 'N/A';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString();
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
