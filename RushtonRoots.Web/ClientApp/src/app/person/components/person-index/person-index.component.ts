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
 * - Parses JSON string inputs from Angular Elements
 * - Handles query parameters for initial search
 */
@Component({
  selector: 'app-person-index',
  standalone: false,
  templateUrl: './person-index.component.html',
  styleUrls: ['./person-index.component.scss']
})
export class PersonIndexComponent implements OnInit {
  // Inputs can be either objects/arrays (when used in Angular) or JSON strings (when used as Angular Elements)
  @Input() initialPeople: PersonTableRow[] | string = [];
  @Input() households: HouseholdOption[] | string = [];
  @Input() canEdit: boolean | string = false;
  @Input() canDelete: boolean | string = false;
  @Input() initialFilters?: PersonSearchFilters | string;

  filteredPeople: PersonTableRow[] = [];
  allPeople: PersonTableRow[] = [];
  parsedHouseholds: HouseholdOption[] = [];
  parsedFilters?: PersonSearchFilters;
  parsedCanEdit = false;
  parsedCanDelete = false;
  isLoading = false;
  errorMessage?: string;

  ngOnInit(): void {
    // Initialize data - parse inputs that may come as JSON strings from Angular Elements
    this.allPeople = this.parseArrayInput<PersonTableRow>(this.initialPeople);
    this.parsedHouseholds = this.parseArrayInput<HouseholdOption>(this.households);
    this.parsedCanEdit = this.parseBooleanInput(this.canEdit);
    this.parsedCanDelete = this.parseBooleanInput(this.canDelete);
    
    // Parse filters (might be JSON string or object)
    if (this.initialFilters) {
      if (typeof this.initialFilters === 'string') {
        try {
          this.parsedFilters = JSON.parse(this.initialFilters);
        } catch (e) {
          console.error('Error parsing initial filters:', e);
          this.parsedFilters = undefined;
        }
      } else {
        this.parsedFilters = this.initialFilters;
      }
    }
    
    // Check for URL query parameters
    // Note: Using window.location for Angular Elements compatibility in MVC views
    // This component is embedded as a custom element, not in a full Angular router context
    const urlParams = new URLSearchParams(window.location.search);
    const searchParam = urlParams.get('search');
    
    // If there's a search query parameter, merge it with existing filters
    // URL parameter takes precedence over initial filters for searchTerm
    if (searchParam) {
      this.parsedFilters = {
        ...this.parsedFilters,
        searchTerm: searchParam // Intentionally overwrites any existing searchTerm
      };
    }
    
    // Set initial filtered people
    if (this.hasActiveFilters(this.parsedFilters)) {
      // If we have filters, apply them
      this.filteredPeople = this.filterPeople(this.allPeople, this.parsedFilters!);
    } else {
      // Otherwise show all people
      this.filteredPeople = this.allPeople;
    }
  }

  /**
   * Check if any filters have active values
   */
  private hasActiveFilters(filters?: PersonSearchFilters): boolean {
    if (!filters) return false;
    return Object.keys(filters).some(key => {
      const value = filters[key as keyof PersonSearchFilters];
      return value !== undefined && value !== null && value !== '';
    });
  }

  /**
   * Parse array input that might be a JSON string or already an array
   */
  private parseArrayInput<T>(input: T[] | string): T[] {
    if (typeof input === 'string') {
      if (!input || input.trim() === '') {
        return [];
      }
      try {
        const parsed = JSON.parse(input);
        return Array.isArray(parsed) ? parsed : [];
      } catch (e) {
        console.error('Error parsing array input:', e, 'Input:', input);
        return [];
      }
    }
    return Array.isArray(input) ? input : [];
  }

  /**
   * Parse boolean input that might be a string or already a boolean
   */
  private parseBooleanInput(input: boolean | string): boolean {
    if (typeof input === 'string') {
      return input.toLowerCase() === 'true';
    }
    return Boolean(input);
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

      // Household filter - now properly checks by ID
      if (filters.householdId !== undefined && filters.householdId !== null) {
        if (person.householdId !== filters.householdId) return false;
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
        // Using window.location.href because this component is used as an Angular Element
        // within a traditional MVC application (not a full SPA)
        window.location.href = `/Person/Delete/${action.personId}`;
      }
    }
  }

  exportToCSV(): void {
    const csvContent = this.convertToCSV(this.filteredPeople);
    this.downloadFile(csvContent, 'people.csv', 'text/csv');
  }

  navigateToCreate(): void {
    // Using window.location.href because this component is used as an Angular Element
    // within a traditional MVC application (not a full SPA)
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
