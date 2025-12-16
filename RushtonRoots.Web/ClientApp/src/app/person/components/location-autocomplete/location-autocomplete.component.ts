import { Component, Input, Output, EventEmitter, OnInit, forwardRef } from '@angular/core';
import { FormControl, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { map, startWith, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { LocationSuggestion } from '../../models/person-form.model';

/**
 * LocationAutocompleteComponent - Autocomplete component for location selection
 * Provides suggestions for cities, states, and countries
 */
@Component({
  selector: 'app-location-autocomplete',
  templateUrl: './location-autocomplete.component.html',
  styleUrls: ['./location-autocomplete.component.scss'],
  standalone: false,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => LocationAutocompleteComponent),
      multi: true
    }
  ]
})
export class LocationAutocompleteComponent implements OnInit, ControlValueAccessor {
  @Input() label = 'Location';
  @Input() placeholder = 'Enter city, state, or country';
  @Input() hint?: string;
  @Input() required = false;
  @Input() disabled = false;
  @Output() locationSelected = new EventEmitter<string>();

  searchControl = new FormControl('');
  filteredLocations$!: Observable<LocationSuggestion[]>;
  value = '';
  
  onChange: (value: string) => void = () => {};
  onTouched: () => void = () => {};

  // Sample location data - in a real app, this would come from an API
  private locations: LocationSuggestion[] = [
    { id: '1', name: 'New York, NY', city: 'New York', state: 'NY', country: 'USA', fullAddress: 'New York, New York, USA' },
    { id: '2', name: 'Los Angeles, CA', city: 'Los Angeles', state: 'CA', country: 'USA', fullAddress: 'Los Angeles, California, USA' },
    { id: '3', name: 'Chicago, IL', city: 'Chicago', state: 'IL', country: 'USA', fullAddress: 'Chicago, Illinois, USA' },
    { id: '4', name: 'Houston, TX', city: 'Houston', state: 'TX', country: 'USA', fullAddress: 'Houston, Texas, USA' },
    { id: '5', name: 'Phoenix, AZ', city: 'Phoenix', state: 'AZ', country: 'USA', fullAddress: 'Phoenix, Arizona, USA' },
    { id: '6', name: 'Philadelphia, PA', city: 'Philadelphia', state: 'PA', country: 'USA', fullAddress: 'Philadelphia, Pennsylvania, USA' },
    { id: '7', name: 'San Antonio, TX', city: 'San Antonio', state: 'TX', country: 'USA', fullAddress: 'San Antonio, Texas, USA' },
    { id: '8', name: 'San Diego, CA', city: 'San Diego', state: 'CA', country: 'USA', fullAddress: 'San Diego, California, USA' },
    { id: '9', name: 'Dallas, TX', city: 'Dallas', state: 'TX', country: 'USA', fullAddress: 'Dallas, Texas, USA' },
    { id: '10', name: 'San Jose, CA', city: 'San Jose', state: 'CA', country: 'USA', fullAddress: 'San Jose, California, USA' },
    { id: '11', name: 'London, England', city: 'London', state: 'England', country: 'United Kingdom', fullAddress: 'London, England, United Kingdom' },
    { id: '12', name: 'Paris, France', city: 'Paris', country: 'France', fullAddress: 'Paris, France' },
    { id: '13', name: 'Berlin, Germany', city: 'Berlin', country: 'Germany', fullAddress: 'Berlin, Germany' },
    { id: '14', name: 'Tokyo, Japan', city: 'Tokyo', country: 'Japan', fullAddress: 'Tokyo, Japan' },
    { id: '15', name: 'Sydney, Australia', city: 'Sydney', state: 'NSW', country: 'Australia', fullAddress: 'Sydney, New South Wales, Australia' }
  ];

  ngOnInit(): void {
    this.filteredLocations$ = this.searchControl.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      map(value => this._filter(value || ''))
    );
  }

  writeValue(value: string): void {
    this.value = value || '';
    this.searchControl.setValue(value, { emitEvent: false });
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
    if (isDisabled) {
      this.searchControl.disable();
    } else {
      this.searchControl.enable();
    }
  }

  onOptionSelected(location: LocationSuggestion): void {
    const selectedValue = location.fullAddress || location.name;
    this.value = selectedValue;
    this.onChange(selectedValue);
    this.onTouched();
    this.locationSelected.emit(selectedValue);
  }

  onInputChange(value: string): void {
    this.value = value;
    this.onChange(value);
    this.onTouched();
  }

  displayFn(location: LocationSuggestion | string): string {
    if (typeof location === 'string') {
      return location;
    }
    return location?.fullAddress || location?.name || '';
  }

  private _filter(value: string): LocationSuggestion[] {
    if (!value) {
      return this.locations.slice(0, 10); // Show top 10 by default
    }
    
    const filterValue = value.toLowerCase();
    return this.locations.filter(location => 
      location.name.toLowerCase().includes(filterValue) ||
      location.city?.toLowerCase().includes(filterValue) ||
      location.state?.toLowerCase().includes(filterValue) ||
      location.country?.toLowerCase().includes(filterValue) ||
      location.fullAddress?.toLowerCase().includes(filterValue)
    );
  }
}
