import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { map, startWith, debounceTime, switchMap } from 'rxjs/operators';
import {
  ParentChildFormData,
  PersonOption,
  RELATIONSHIP_TYPES,
  ValidationResult
} from '../../models/parent-child.model';

/**
 * ParentChildFormComponent - Form for creating/editing parent-child relationships
 * Phase 5.2: Parent-Child Relationships
 */
@Component({
  selector: 'app-parent-child-form',
  standalone: false,
  templateUrl: './parent-child-form.component.html',
  styleUrls: ['./parent-child-form.component.scss']
})
export class ParentChildFormComponent implements OnInit {
  /**
   * Existing relationship data for edit mode
   */
  @Input() relationship?: ParentChildFormData;

  /**
   * Available people for selection
   */
  @Input() availablePeople: PersonOption[] = [];

  /**
   * Event emitted when form is submitted
   */
  @Output() submitted = new EventEmitter<ParentChildFormData>();

  /**
   * Event emitted when form is cancelled
   */
  @Output() cancelled = new EventEmitter<void>();

  /**
   * Event emitted when validation is requested
   */
  @Output() validateRequested = new EventEmitter<ParentChildFormData>();

  /**
   * Parent-child relationship form
   */
  relationshipForm!: FormGroup;

  /**
   * Available relationship types
   */
  relationshipTypes = RELATIONSHIP_TYPES;

  /**
   * Filtered parent options
   */
  filteredParentOptions!: Observable<PersonOption[]>;

  /**
   * Filtered child options
   */
  filteredChildOptions!: Observable<PersonOption[]>;

  /**
   * Form submission state
   */
  isSubmitting = false;

  /**
   * Edit mode flag
   */
  isEditMode = false;

  /**
   * Validation result
   */
  validationResult?: ValidationResult;

  /**
   * Show validation panel
   */
  showValidation = false;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.isEditMode = !!this.relationship?.id;
    this.initializeForm();
    this.setupAutocomplete();
    this.loadSamplePeople();
  }

  /**
   * Initialize form
   */
  private initializeForm(): void {
    this.relationshipForm = this.fb.group({
      parentPersonId: [this.relationship?.parentPersonId || null, Validators.required],
      parentSearch: [''],
      childPersonId: [this.relationship?.childPersonId || null, Validators.required],
      childSearch: [''],
      relationshipType: [this.relationship?.relationshipType || 'biological', Validators.required],
      notes: [this.relationship?.notes || '', Validators.maxLength(500)],
      isVerified: [this.relationship?.isVerified || false]
    });

    // Load selected persons if in edit mode
    if (this.isEditMode && this.relationship) {
      const parent = this.availablePeople.find(p => p.id === this.relationship!.parentPersonId);
      const child = this.availablePeople.find(p => p.id === this.relationship!.childPersonId);
      
      if (parent) {
        this.relationshipForm.patchValue({ parentSearch: parent.name });
      }
      if (child) {
        this.relationshipForm.patchValue({ childSearch: child.name });
      }
    }
  }

  /**
   * Load sample people for demonstration
   */
  private loadSamplePeople(): void {
    if (this.availablePeople.length === 0) {
      this.availablePeople = [
        { id: 1, name: 'John Smith', birthDate: new Date('1955-12-25'), age: 68 },
        { id: 2, name: 'Jane Doe', birthDate: new Date('1957-03-14'), age: 66 },
        { id: 3, name: 'Robert Smith Jr.', birthDate: new Date('1980-06-15'), age: 43 },
        { id: 4, name: 'Emily Smith', birthDate: new Date('2005-06-15'), age: 18 },
        { id: 5, name: 'Michael Johnson', birthDate: new Date('2010-03-20'), age: 13 },
        { id: 6, name: 'Sarah Johnson', birthDate: new Date('1975-08-10'), age: 48 },
        { id: 7, name: 'David Brown', birthDate: new Date('1970-11-30'), age: 53 },
        { id: 8, name: 'Sophie Brown', birthDate: new Date('2015-09-10'), age: 8 }
      ];
    }
  }

  /**
   * Setup autocomplete for parent and child selections
   */
  private setupAutocomplete(): void {
    // Parent autocomplete
    this.filteredParentOptions = this.relationshipForm.get('parentSearch')!.valueChanges
      .pipe(
        startWith(''),
        debounceTime(300),
        map(value => this.filterPeople(value || ''))
      );

    // Child autocomplete
    this.filteredChildOptions = this.relationshipForm.get('childSearch')!.valueChanges
      .pipe(
        startWith(''),
        debounceTime(300),
        map(value => this.filterPeople(value || ''))
      );
  }

  /**
   * Filter people based on search text
   */
  private filterPeople(searchText: string): PersonOption[] {
    const filterValue = searchText.toLowerCase();
    return this.availablePeople.filter(person =>
      person.name.toLowerCase().includes(filterValue)
    );
  }

  /**
   * Handle parent selection
   */
  onParentSelected(person: PersonOption): void {
    this.relationshipForm.patchValue({
      parentPersonId: person.id,
      parentSearch: person.name
    });
  }

  /**
   * Handle child selection
   */
  onChildSelected(person: PersonOption): void {
    this.relationshipForm.patchValue({
      childPersonId: person.id,
      childSearch: person.name
    });
  }

  /**
   * Display function for autocomplete
   */
  displayPerson(person: PersonOption | null): string {
    return person ? person.name : '';
  }

  /**
   * Get selected relationship type configuration
   */
  get selectedTypeConfig() {
    const type = this.relationshipForm.get('relationshipType')?.value;
    return this.relationshipTypes.find(t => t.value === type) || this.relationshipTypes[0];
  }

  /**
   * Validate relationship
   */
  onValidate(): void {
    if (this.relationshipForm.valid) {
      const formData = this.getFormData();
      this.validateRequested.emit(formData);
      
      // Sample validation result - replace with actual validation
      this.validationResult = {
        isValid: true,
        errors: [],
        warnings: [
          {
            type: 'age-gap',
            message: 'Large age gap detected',
            details: 'The age difference between parent and child is unusually large (40+ years)'
          }
        ]
      };
      this.showValidation = true;
    }
  }

  /**
   * Get form data
   */
  private getFormData(): ParentChildFormData {
    const formValue = this.relationshipForm.value;
    return {
      id: this.relationship?.id,
      parentPersonId: formValue.parentPersonId,
      childPersonId: formValue.childPersonId,
      relationshipType: formValue.relationshipType,
      notes: formValue.notes,
      isVerified: formValue.isVerified
    };
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.relationshipForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      const formData = this.getFormData();
      
      // Simulate API call
      setTimeout(() => {
        this.submitted.emit(formData);
        this.isSubmitting = false;
      }, 500);
    }
  }

  /**
   * Handle form cancellation
   */
  onCancel(): void {
    if (this.relationshipForm.dirty) {
      if (confirm('You have unsaved changes. Are you sure you want to cancel?')) {
        this.cancelled.emit();
      }
    } else {
      this.cancelled.emit();
    }
  }

  /**
   * Get person initials for avatar
   */
  getInitials(name: string): string {
    const names = name.split(' ');
    return names.length > 1
      ? `${names[0][0]}${names[names.length - 1][0]}`.toUpperCase()
      : name.substring(0, 2).toUpperCase();
  }
}
