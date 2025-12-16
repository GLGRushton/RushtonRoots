import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import {
  HouseholdFormData,
  PersonOption,
  HouseholdFormMember,
  PRIVACY_OPTIONS,
  PrivacyOption,
  MEMBER_ROLES,
  MemberRoleOption
} from '../../models/household-form.model';

/**
 * HouseholdFormComponent - Form for creating and editing households
 * 
 * Features:
 * - Basic information (name, description)
 * - Anchor person selection with autocomplete
 * - Initial members selection (multiple autocomplete)
 * - Privacy settings
 * - Permission defaults
 * - Create/Update mode support
 * 
 * Usage:
 * <app-household-form
 *   [householdId]="householdId"
 *   [initialData]="householdData"
 *   [peopleList]="people"
 *   (formSubmit)="onFormSubmit($event)"
 *   (formCancel)="onFormCancel()">
 * </app-household-form>
 */
@Component({
  selector: 'app-household-form',
  templateUrl: './household-form.component.html',
  styleUrls: ['./household-form.component.scss'],
  standalone: false
})
export class HouseholdFormComponent implements OnInit {
  @Input() householdId?: number; // If editing existing household
  @Input() initialData?: Partial<HouseholdFormData>;
  @Input() peopleList: PersonOption[] = []; // List of all people for selection
  @Output() formSubmit = new EventEmitter<HouseholdFormData>();
  @Output() formCancel = new EventEmitter<void>();

  // Main form group
  householdForm!: FormGroup;

  // State
  isEditMode = false;
  isSubmitting = false;
  
  // Selections
  selectedAnchorPerson: PersonOption | null = null;
  selectedMembers: HouseholdFormMember[] = [];
  
  // Options
  privacyOptions: PrivacyOption[] = PRIVACY_OPTIONS;
  memberRoles: MemberRoleOption[] = MEMBER_ROLES;
  
  // Autocomplete
  filteredPeople: PersonOption[] = [];
  filteredMembers: PersonOption[] = [];

  // Display function for autocomplete
  displayPersonFn = (person: PersonOption | null): string => {
    return person ? person.fullName : '';
  };

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.isEditMode = !!this.householdId;
    this.initializeForm();
    
    if (this.initialData) {
      this.loadInitialData();
    }
  }

  /**
   * Initialize the form with validation
   */
  private initializeForm(): void {
    this.householdForm = this.fb.group({
      householdName: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(2000)],
      anchorPersonId: [null],
      anchorPersonSearch: [''], // For autocomplete display
      initialMemberIds: [[]],
      memberSearch: [''], // For autocomplete display
      privacyLevel: ['family', Validators.required],
      allowMemberInvites: [true],
      allowMemberEdits: [true],
      allowMemberUploads: [true]
    });

    // Setup autocomplete for anchor person
    this.householdForm.get('anchorPersonSearch')?.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(searchTerm => {
        this.filterPeople(searchTerm);
      });

    // Setup autocomplete for members
    this.householdForm.get('memberSearch')?.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(searchTerm => {
        this.filterMembers(searchTerm);
      });
  }

  /**
   * Load initial data for edit mode
   */
  private loadInitialData(): void {
    if (!this.initialData) return;

    this.householdForm.patchValue({
      householdName: this.initialData.householdName || '',
      description: this.initialData.description || '',
      anchorPersonId: this.initialData.anchorPersonId || null,
      privacyLevel: this.initialData.privacyLevel || 'family',
      allowMemberInvites: this.initialData.allowMemberInvites ?? true,
      allowMemberEdits: this.initialData.allowMemberEdits ?? true,
      allowMemberUploads: this.initialData.allowMemberUploads ?? true
    });

    // Set selected anchor person
    if (this.initialData.anchorPersonId) {
      const anchorPerson = this.peopleList.find(p => p.id === this.initialData!.anchorPersonId);
      if (anchorPerson) {
        this.selectedAnchorPerson = anchorPerson;
        this.householdForm.patchValue({
          anchorPersonSearch: anchorPerson.fullName
        });
      }
    }

    // Set selected members
    if (this.initialData.initialMembers && this.initialData.initialMembers.length > 0) {
      this.selectedMembers = [...this.initialData.initialMembers];
    }
  }

  /**
   * Filter people list based on search term
   */
  private filterPeople(searchTerm: string): void {
    if (!searchTerm || searchTerm.length < 2) {
      this.filteredPeople = [];
      return;
    }

    const term = searchTerm.toLowerCase();
    this.filteredPeople = this.peopleList.filter(person =>
      person.fullName.toLowerCase().includes(term) ||
      person.firstName.toLowerCase().includes(term) ||
      person.lastName.toLowerCase().includes(term)
    ).slice(0, 10); // Limit to 10 results
  }

  /**
   * Filter members list (exclude already selected members and anchor person)
   */
  private filterMembers(searchTerm: string): void {
    if (!searchTerm || searchTerm.length < 2) {
      this.filteredMembers = [];
      return;
    }

    const term = searchTerm.toLowerCase();
    const selectedIds = this.selectedMembers.map(m => m.personId);
    const anchorId = this.selectedAnchorPerson?.id;

    this.filteredMembers = this.peopleList.filter(person =>
      !selectedIds.includes(person.id) &&
      person.id !== anchorId &&
      (person.fullName.toLowerCase().includes(term) ||
       person.firstName.toLowerCase().includes(term) ||
       person.lastName.toLowerCase().includes(term))
    ).slice(0, 10); // Limit to 10 results
  }

  /**
   * Handle anchor person selection
   */
  onAnchorPersonSelected(person: PersonOption): void {
    this.selectedAnchorPerson = person;
    this.householdForm.patchValue({
      anchorPersonId: person.id,
      anchorPersonSearch: person.fullName
    });
    this.filteredPeople = [];
  }

  /**
   * Clear anchor person selection
   */
  clearAnchorPerson(): void {
    this.selectedAnchorPerson = null;
    this.householdForm.patchValue({
      anchorPersonId: null,
      anchorPersonSearch: ''
    });
  }

  /**
   * Handle member selection
   */
  onMemberSelected(person: PersonOption): void {
    // Check if already added
    if (this.selectedMembers.some(m => m.personId === person.id)) {
      this.snackBar.open('This person is already added as a member', 'Close', {
        duration: 3000
      });
      return;
    }

    // Add as member with default role
    const newMember: HouseholdFormMember = {
      personId: person.id,
      fullName: person.fullName,
      photoUrl: person.photoUrl,
      role: 'contributor', // Default role
      canInvite: false,
      canEdit: false,
      canUpload: true
    };

    this.selectedMembers.push(newMember);
    
    // Clear search
    this.householdForm.patchValue({
      memberSearch: ''
    });
    this.filteredMembers = [];

    this.snackBar.open(`${person.fullName} added as member`, 'Close', {
      duration: 2000
    });
  }

  /**
   * Remove a member from the selection
   */
  removeMember(member: HouseholdFormMember): void {
    const index = this.selectedMembers.findIndex(m => m.personId === member.personId);
    if (index !== -1) {
      this.selectedMembers.splice(index, 1);
      this.snackBar.open(`${member.fullName} removed`, 'Close', {
        duration: 2000
      });
    }
  }

  /**
   * Update member role
   */
  updateMemberRole(member: HouseholdFormMember, role: 'admin' | 'editor' | 'contributor' | 'viewer'): void {
    member.role = role;
    
    // Update permissions based on role
    switch (role) {
      case 'admin':
        member.canInvite = true;
        member.canEdit = true;
        member.canUpload = true;
        break;
      case 'editor':
        member.canInvite = true;
        member.canEdit = true;
        member.canUpload = true;
        break;
      case 'contributor':
        member.canInvite = false;
        member.canEdit = false;
        member.canUpload = true;
        break;
      case 'viewer':
        member.canInvite = false;
        member.canEdit = false;
        member.canUpload = false;
        break;
    }
  }

  /**
   * Get privacy option icon
   */
  getPrivacyIcon(level: string): string {
    const option = this.privacyOptions.find(o => o.value === level);
    return option?.icon || 'help';
  }

  /**
   * Get role information
   */
  getRoleInfo(roleValue: string): MemberRoleOption | undefined {
    return this.memberRoles.find(r => r.value === roleValue);
  }

  /**
   * Submit form
   */
  onSubmit(): void {
    if (this.householdForm.invalid) {
      this.snackBar.open('Please fill in all required fields', 'Close', {
        duration: 3000
      });
      return;
    }

    this.isSubmitting = true;

    const formValue = this.householdForm.value;
    const formData: HouseholdFormData = {
      id: this.householdId,
      householdName: formValue.householdName,
      description: formValue.description,
      anchorPersonId: formValue.anchorPersonId,
      initialMemberIds: this.selectedMembers.map(m => m.personId),
      initialMembers: this.selectedMembers,
      privacyLevel: formValue.privacyLevel,
      allowMemberInvites: formValue.allowMemberInvites,
      allowMemberEdits: formValue.allowMemberEdits,
      allowMemberUploads: formValue.allowMemberUploads
    };

    this.formSubmit.emit(formData);
  }

  /**
   * Cancel form
   */
  onCancel(): void {
    if (this.householdForm.dirty) {
      const confirmed = confirm('You have unsaved changes. Are you sure you want to cancel?');
      if (!confirmed) {
        return;
      }
    }

    this.formCancel.emit();
  }

  /**
   * Check if form is valid
   */
  isFormValid(): boolean {
    return this.householdForm.valid;
  }
}
