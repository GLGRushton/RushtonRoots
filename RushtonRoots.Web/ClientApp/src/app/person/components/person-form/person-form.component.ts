import { Component, OnInit, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject, interval } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PersonFormData, FormDraft } from '../../models/person-form.model';

/**
 * PersonFormComponent - Wizard-based form for creating and editing people
 * Features: Multi-step wizard, validation, autosave, photo upload
 */
@Component({
  selector: 'app-person-form',
  templateUrl: './person-form.component.html',
  styleUrls: ['./person-form.component.scss'],
  standalone: false
})
export class PersonFormComponent implements OnInit, OnDestroy {
  @Input() personId?: number; // If editing existing person
  @Input() initialData?: Partial<PersonFormData>;
  @Output() formSubmit = new EventEmitter<PersonFormData>();
  @Output() formCancel = new EventEmitter<void>();

  // Form groups for each step
  basicInfoForm!: FormGroup;
  datesPlacesForm!: FormGroup;
  additionalInfoForm!: FormGroup;
  photoUploadForm!: FormGroup;

  // State
  isLinear = true;
  isSubmitting = false;
  photoPreviewUrl: string | null = null;
  selectedPhoto: File | null = null;

  // Autosave
  private readonly AUTOSAVE_INTERVAL = 30000; // 30 seconds
  private readonly DRAFT_STORAGE_KEY = 'person-form-draft';
  private destroy$ = new Subject<void>();
  lastSaveTime: Date | null = null;

  // Gender options
  genderOptions = ['Male', 'Female', 'Other', 'Unknown'];

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.initializeForms();
    this.loadDraft();
    this.setupAutosave();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForms(): void {
    // Step 1: Basic Information
    this.basicInfoForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
      middleName: ['', Validators.maxLength(100)],
      lastName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
      suffix: ['', Validators.maxLength(20)],
      gender: ['']
    });

    // Step 2: Dates & Places
    this.datesPlacesForm = this.fb.group({
      dateOfBirth: [''],
      placeOfBirth: [''],
      isDeceased: [false],
      dateOfDeath: [''],
      placeOfDeath: ['']
    });

    // Watch isDeceased to conditionally validate death fields
    this.datesPlacesForm.get('isDeceased')?.valueChanges.subscribe(isDeceased => {
      const deathDateControl = this.datesPlacesForm.get('dateOfDeath');
      const deathPlaceControl = this.datesPlacesForm.get('placeOfDeath');
      
      if (isDeceased) {
        deathDateControl?.enable();
        deathPlaceControl?.enable();
      } else {
        deathDateControl?.setValue('');
        deathPlaceControl?.setValue('');
        deathDateControl?.disable();
        deathPlaceControl?.disable();
      }
    });

    // Step 3: Additional Information
    this.additionalInfoForm = this.fb.group({
      householdId: [''],
      biography: ['', Validators.maxLength(5000)],
      occupation: ['', Validators.maxLength(200)],
      education: ['', Validators.maxLength(500)],
      notes: ['', Validators.maxLength(2000)]
    });

    // Step 4: Photo Upload
    this.photoUploadForm = this.fb.group({
      photoUrl: ['']
    });

    // Populate with initial data if editing
    if (this.initialData) {
      this.populateFormWithData(this.initialData);
    }
  }

  private populateFormWithData(data: Partial<PersonFormData>): void {
    if (data.firstName) this.basicInfoForm.patchValue({ firstName: data.firstName });
    if (data.middleName) this.basicInfoForm.patchValue({ middleName: data.middleName });
    if (data.lastName) this.basicInfoForm.patchValue({ lastName: data.lastName });
    if (data.suffix) this.basicInfoForm.patchValue({ suffix: data.suffix });
    if (data.gender) this.basicInfoForm.patchValue({ gender: data.gender });

    if (data.dateOfBirth) this.datesPlacesForm.patchValue({ dateOfBirth: data.dateOfBirth });
    if (data.placeOfBirth) this.datesPlacesForm.patchValue({ placeOfBirth: data.placeOfBirth });
    if (data.isDeceased !== undefined) this.datesPlacesForm.patchValue({ isDeceased: data.isDeceased });
    if (data.dateOfDeath) this.datesPlacesForm.patchValue({ dateOfDeath: data.dateOfDeath });
    if (data.placeOfDeath) this.datesPlacesForm.patchValue({ placeOfDeath: data.placeOfDeath });

    if (data.householdId) this.additionalInfoForm.patchValue({ householdId: data.householdId });
    if (data.biography) this.additionalInfoForm.patchValue({ biography: data.biography });
    if (data.occupation) this.additionalInfoForm.patchValue({ occupation: data.occupation });
    if (data.education) this.additionalInfoForm.patchValue({ education: data.education });
    if (data.notes) this.additionalInfoForm.patchValue({ notes: data.notes });

    if (data.photoUrl) {
      this.photoUploadForm.patchValue({ photoUrl: data.photoUrl });
      this.photoPreviewUrl = data.photoUrl;
    }
  }

  private setupAutosave(): void {
    // Auto-save every 30 seconds
    interval(this.AUTOSAVE_INTERVAL)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.saveDraft();
      });

    // Also save on form value changes (debounced through interval)
    this.basicInfoForm.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => {
      // Mark that form has changed
    });
  }

  private saveDraft(): void {
    if (!this.isFormDirty()) {
      return;
    }

    const formData = this.gatherFormData();
    const draft: FormDraft = {
      formData,
      lastSaved: new Date(),
      step: 0 // Could track current step if needed
    };

    try {
      localStorage.setItem(this.DRAFT_STORAGE_KEY, JSON.stringify(draft));
      this.lastSaveTime = draft.lastSaved;
      this.showSnackbar('Draft saved', 'dismiss', 2000);
    } catch (error) {
      console.error('Failed to save draft:', error);
    }
  }

  private loadDraft(): void {
    try {
      const draftJson = localStorage.getItem(this.DRAFT_STORAGE_KEY);
      if (draftJson && !this.initialData) {
        const draft: FormDraft = JSON.parse(draftJson);
        const savedDate = new Date(draft.lastSaved);
        const hoursSinceLastSave = (Date.now() - savedDate.getTime()) / (1000 * 60 * 60);

        // Only load drafts less than 24 hours old
        if (hoursSinceLastSave < 24) {
          const result = confirm(`A draft from ${savedDate.toLocaleString()} was found. Would you like to restore it?`);
          if (result) {
            this.populateFormWithData(draft.formData);
            this.showSnackbar('Draft restored', 'dismiss', 3000);
          } else {
            this.clearDraft();
          }
        } else {
          this.clearDraft();
        }
      }
    } catch (error) {
      console.error('Failed to load draft:', error);
    }
  }

  private clearDraft(): void {
    try {
      localStorage.removeItem(this.DRAFT_STORAGE_KEY);
    } catch (error) {
      console.error('Failed to clear draft:', error);
    }
  }

  private isFormDirty(): boolean {
    return this.basicInfoForm.dirty || 
           this.datesPlacesForm.dirty || 
           this.additionalInfoForm.dirty || 
           this.photoUploadForm.dirty;
  }

  onPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validate file type
      if (!file.type.startsWith('image/')) {
        this.showSnackbar('Please select an image file', 'error', 3000);
        return;
      }

      // Validate file size (max 5MB)
      const maxSize = 5 * 1024 * 1024; // 5MB
      if (file.size > maxSize) {
        this.showSnackbar('Image size must be less than 5MB', 'error', 3000);
        return;
      }

      this.selectedPhoto = file;

      // Create preview
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        this.photoPreviewUrl = e.target?.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  removePhoto(): void {
    this.selectedPhoto = null;
    this.photoPreviewUrl = null;
    this.photoUploadForm.patchValue({ photoUrl: '' });
  }

  private gatherFormData(): PersonFormData {
    return {
      ...this.basicInfoForm.value,
      ...this.datesPlacesForm.value,
      ...this.additionalInfoForm.value,
      photoFile: this.selectedPhoto || undefined,
      photoUrl: this.photoPreviewUrl || undefined
    };
  }

  onSubmit(): void {
    // Validate all forms
    if (!this.basicInfoForm.valid) {
      this.showSnackbar('Please complete the required fields in Basic Information', 'error', 4000);
      return;
    }

    this.isSubmitting = true;
    const formData = this.gatherFormData();

    // Emit the form data
    this.formSubmit.emit(formData);

    // Clear draft after successful submit
    this.clearDraft();

    // Show success message
    this.showSnackbar(
      this.personId ? 'Person updated successfully!' : 'Person created successfully!',
      'success',
      3000
    );

    // Reset submitting state
    setTimeout(() => {
      this.isSubmitting = false;
    }, 1000);
  }

  onCancel(): void {
    if (this.isFormDirty()) {
      const result = confirm('You have unsaved changes. Are you sure you want to cancel?');
      if (!result) {
        return;
      }
    }
    
    this.formCancel.emit();
  }

  private showSnackbar(message: string, action: string, duration: number): void {
    this.snackBar.open(message, action, {
      duration,
      horizontalPosition: 'center',
      verticalPosition: 'bottom'
    });
  }

  // Getters for template access
  get isDeceased(): boolean {
    return this.datesPlacesForm.get('isDeceased')?.value || false;
  }

  get formTitle(): string {
    return this.personId ? 'Edit Person' : 'Create New Person';
  }

  get submitButtonText(): string {
    return this.personId ? 'Update Person' : 'Create Person';
  }
}
