import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { 
  PartnershipFormData, 
  PersonOption,
  PARTNERSHIP_TYPES 
} from '../../models/partnership.model';
import { Observable, of } from 'rxjs';
import { map, startWith, debounceTime, switchMap } from 'rxjs/operators';

/**
 * PartnershipFormComponent - Form for creating/editing partnerships
 * Phase 5.1: Partnership Management
 */
@Component({
  selector: 'app-partnership-form',
  templateUrl: './partnership-form.component.html',
  styleUrls: ['./partnership-form.component.scss']
})
export class PartnershipFormComponent implements OnInit {
  /**
   * Existing partnership data for edit mode
   */
  @Input() partnership?: PartnershipFormData;

  /**
   * Available people for selection
   */
  @Input() availablePeople: PersonOption[] = [];

  /**
   * Event emitted when form is submitted
   */
  @Output() submitted = new EventEmitter<PartnershipFormData>();

  /**
   * Event emitted when form is cancelled
   */
  @Output() cancelled = new EventEmitter<void>();

  /**
   * Partnership form
   */
  partnershipForm!: FormGroup;

  /**
   * Available partnership types
   */
  partnershipTypes = PARTNERSHIP_TYPES;

  /**
   * Filtered person A options
   */
  filteredPersonAOptions!: Observable<PersonOption[]>;

  /**
   * Filtered person B options
   */
  filteredPersonBOptions!: Observable<PersonOption[]>;

  /**
   * Form submission state
   */
  isSubmitting = false;

  /**
   * Edit mode flag
   */
  isEditMode = false;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.isEditMode = !!this.partnership?.id;
    this.initializeForm();
    this.setupAutocomplete();
  }

  /**
   * Initialize form
   */
  private initializeForm(): void {
    this.partnershipForm = this.fb.group({
      personAId: [this.partnership?.personAId || null, Validators.required],
      personASearch: [''],
      personBId: [this.partnership?.personBId || null, Validators.required],
      personBSearch: [''],
      partnershipType: [this.partnership?.partnershipType || 'married', Validators.required],
      startDate: [this.partnership?.startDate || null],
      endDate: [this.partnership?.endDate || null],
      location: [this.partnership?.location || ''],
      notes: [this.partnership?.notes || '', Validators.maxLength(1000)]
    });

    // Load selected persons if in edit mode
    if (this.isEditMode && this.partnership) {
      const personA = this.availablePeople.find(p => p.id === this.partnership!.personAId);
      const personB = this.availablePeople.find(p => p.id === this.partnership!.personBId);
      
      if (personA) {
        this.partnershipForm.patchValue({ personASearch: personA.name });
      }
      
      if (personB) {
        this.partnershipForm.patchValue({ personBSearch: personB.name });
      }
    }
  }

  /**
   * Setup autocomplete for person selection
   */
  private setupAutocomplete(): void {
    // Person A autocomplete
    this.filteredPersonAOptions = this.partnershipForm.get('personASearch')!.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      map(value => this.filterPersons(value || '', 'personA'))
    );

    // Person B autocomplete
    this.filteredPersonBOptions = this.partnershipForm.get('personBSearch')!.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      map(value => this.filterPersons(value || '', 'personB'))
    );
  }

  /**
   * Filter persons based on search text
   */
  private filterPersons(searchText: string, excludeField: 'personA' | 'personB'): PersonOption[] {
    const excludeId = excludeField === 'personA' 
      ? this.partnershipForm.get('personBId')?.value 
      : this.partnershipForm.get('personAId')?.value;

    if (!searchText) {
      return this.availablePeople.filter(p => p.id !== excludeId);
    }

    const filterValue = searchText.toLowerCase();
    return this.availablePeople.filter(person =>
      person.id !== excludeId &&
      person.name.toLowerCase().includes(filterValue)
    );
  }

  /**
   * Handle person A selection
   */
  onPersonASelected(person: PersonOption): void {
    this.partnershipForm.patchValue({
      personAId: person.id,
      personASearch: person.name
    });
  }

  /**
   * Handle person B selection
   */
  onPersonBSelected(person: PersonOption): void {
    this.partnershipForm.patchValue({
      personBId: person.id,
      personBSearch: person.name
    });
  }

  /**
   * Display function for autocomplete
   */
  displayPerson(person: PersonOption): string {
    return person ? person.name : '';
  }

  /**
   * Get person display with lifespan
   */
  getPersonDisplay(person: PersonOption): string {
    return person.lifeSpan 
      ? `${person.name} (${person.lifeSpan})` 
      : person.name;
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.partnershipForm.invalid) {
      this.partnershipForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const formData: PartnershipFormData = {
      id: this.partnership?.id,
      personAId: this.partnershipForm.value.personAId,
      personBId: this.partnershipForm.value.personBId,
      partnershipType: this.partnershipForm.value.partnershipType,
      startDate: this.partnershipForm.value.startDate,
      endDate: this.partnershipForm.value.endDate,
      location: this.partnershipForm.value.location,
      notes: this.partnershipForm.value.notes
    };

    this.submitted.emit(formData);
  }

  /**
   * Handle form cancellation
   */
  onCancel(): void {
    if (this.partnershipForm.dirty) {
      if (confirm('Are you sure you want to discard your changes?')) {
        this.cancelled.emit();
      }
    } else {
      this.cancelled.emit();
    }
  }

  /**
   * Get form control
   */
  getControl(controlName: string) {
    return this.partnershipForm.get(controlName);
  }

  /**
   * Check if form control has error
   */
  hasError(controlName: string, errorName: string): boolean {
    const control = this.getControl(controlName);
    return control ? control.hasError(errorName) && control.touched : false;
  }

  /**
   * Get character count for notes
   */
  getNotesCharCount(): number {
    return this.partnershipForm.value.notes?.length || 0;
  }
}
