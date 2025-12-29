import { Component, EventEmitter, Input, OnInit, OnChanges, SimpleChanges, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

export interface PersonSearchFilters {
  searchTerm?: string;
  householdId?: number;
  isDeceased?: boolean;
  birthDateFrom?: Date;
  birthDateTo?: Date;
  deathDateFrom?: Date;
  deathDateTo?: Date;
  surname?: string;
}

export interface HouseholdOption {
  id: number;
  householdName: string;
}

/**
 * PersonSearchComponent - Advanced search interface for filtering people
 * 
 * Features:
 * - Text search with debouncing
 * - Household filter dropdown
 * - Deceased status filter
 * - Date range filters (birth/death)
 * - Surname filter
 * - Active filter chips
 * - Clear all filters
 */
@Component({
  selector: 'app-person-search',
  standalone: false,
  templateUrl: './person-search.component.html',
  styleUrls: ['./person-search.component.scss']
})
export class PersonSearchComponent implements OnInit, OnChanges {
  @Input() households: HouseholdOption[] = [];
  @Input() initialFilters?: PersonSearchFilters;
  @Output() search = new EventEmitter<PersonSearchFilters>();
  @Output() filtersChanged = new EventEmitter<PersonSearchFilters>();

  searchForm!: FormGroup;
  showAdvancedFilters = false;
  activeFilterCount = 0;

  constructor(private fb: FormBuilder) {}

  ngOnChanges(changes: SimpleChanges): void {
    // Handle changes to initialFilters after component initialization
    const initialFiltersChange = changes['initialFilters'];
    if (initialFiltersChange && !initialFiltersChange.firstChange && this.searchForm) {
      const filters = initialFiltersChange.currentValue;
      if (filters) {
        this.searchForm.patchValue({
          searchTerm: filters.searchTerm || '',
          householdId: filters.householdId ?? null,
          isDeceased: filters.isDeceased ?? null,
          birthDateFrom: filters.birthDateFrom || null,
          birthDateTo: filters.birthDateTo || null,
          deathDateFrom: filters.deathDateFrom || null,
          deathDateTo: filters.deathDateTo || null,
          surname: filters.surname || ''
        }, { emitEvent: false });
        this.updateActiveFilterCount();
      }
    }
  }

  ngOnInit(): void {
    this.searchForm = this.fb.group({
      searchTerm: [this.initialFilters?.searchTerm || ''],
      householdId: [this.initialFilters?.householdId ?? null],
      isDeceased: [this.initialFilters?.isDeceased ?? null],
      birthDateFrom: [this.initialFilters?.birthDateFrom || null],
      birthDateTo: [this.initialFilters?.birthDateTo || null],
      deathDateFrom: [this.initialFilters?.deathDateFrom || null],
      deathDateTo: [this.initialFilters?.deathDateTo || null],
      surname: [this.initialFilters?.surname || '']
    });

    // Auto-search on form changes with debouncing
    this.searchForm.valueChanges
      .pipe(
        debounceTime(400),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.updateActiveFilterCount();
        this.onSearch();
      });

    this.updateActiveFilterCount();
  }

  onSearch(): void {
    const filters = this.getActiveFilters();
    this.search.emit(filters);
    this.filtersChanged.emit(filters);
  }

  getActiveFilters(): PersonSearchFilters {
    const formValue = this.searchForm.value;
    const filters: PersonSearchFilters = {};

    if (formValue.searchTerm) filters.searchTerm = formValue.searchTerm;
    if (formValue.householdId !== null && formValue.householdId !== undefined) {
      filters.householdId = formValue.householdId;
    }
    if (formValue.isDeceased !== null && formValue.isDeceased !== '') {
      filters.isDeceased = formValue.isDeceased === 'true' || formValue.isDeceased === true;
    }
    if (formValue.birthDateFrom) filters.birthDateFrom = formValue.birthDateFrom;
    if (formValue.birthDateTo) filters.birthDateTo = formValue.birthDateTo;
    if (formValue.deathDateFrom) filters.deathDateFrom = formValue.deathDateFrom;
    if (formValue.deathDateTo) filters.deathDateTo = formValue.deathDateTo;
    if (formValue.surname) filters.surname = formValue.surname;

    return filters;
  }

  updateActiveFilterCount(): void {
    const filters = this.getActiveFilters();
    this.activeFilterCount = Object.keys(filters).length;
  }

  clearFilters(): void {
    this.searchForm.reset({
      searchTerm: '',
      householdId: null,
      isDeceased: null,
      birthDateFrom: null,
      birthDateTo: null,
      deathDateFrom: null,
      deathDateTo: null,
      surname: ''
    });
    this.showAdvancedFilters = false;
    this.onSearch();
  }

  removeFilter(filterKey: keyof PersonSearchFilters): void {
    this.searchForm.patchValue({ [filterKey]: null });
  }

  toggleAdvancedFilters(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
  }

  getFilterChipLabel(key: keyof PersonSearchFilters, value: any): string {
    switch (key) {
      case 'searchTerm':
        return `Search: ${value}`;
      case 'householdId':
        const household = this.households.find(h => h.id === value);
        return household ? `Household: ${household.householdName}` : `Household: ${value}`;
      case 'isDeceased':
        return value ? 'Status: Deceased' : 'Status: Living';
      case 'surname':
        return `Surname: ${value}`;
      case 'birthDateFrom':
        return `Birth from: ${new Date(value).toLocaleDateString()}`;
      case 'birthDateTo':
        return `Birth to: ${new Date(value).toLocaleDateString()}`;
      case 'deathDateFrom':
        return `Death from: ${new Date(value).toLocaleDateString()}`;
      case 'deathDateTo':
        return `Death to: ${new Date(value).toLocaleDateString()}`;
      default:
        return `${key}: ${value}`;
    }
  }

  getActiveFilterChips(): Array<{ key: keyof PersonSearchFilters; value: any; label: string }> {
    const filters = this.getActiveFilters();
    return Object.entries(filters).map(([key, value]) => ({
      key: key as keyof PersonSearchFilters,
      value,
      label: this.getFilterChipLabel(key as keyof PersonSearchFilters, value)
    }));
  }
}
