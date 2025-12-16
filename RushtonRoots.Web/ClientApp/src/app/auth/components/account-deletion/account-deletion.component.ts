import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountDeletionRequest, ACCOUNT_DELETION_REASONS } from '../../models/user-profile.model';

/**
 * AccountDeletionComponent - Handle account deletion flow
 */
@Component({
  selector: 'app-account-deletion',
  templateUrl: './account-deletion.component.html',
  styleUrls: ['./account-deletion.component.scss'],
  standalone: false
})
export class AccountDeletionComponent implements OnInit {
  @Input() canDelete = true;
  @Output() accountDelete = new EventEmitter<AccountDeletionRequest>();

  deletionForm!: FormGroup;
  showDeletionForm = false;
  deletionReasons = ACCOUNT_DELETION_REASONS;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.deletionForm = this.fb.group({
      reason: ['', Validators.required],
      feedback: [''],
      confirmEmail: ['', [Validators.required, Validators.email]],
      confirmPassword: ['', Validators.required],
      transferDataTo: [''],
      deleteImmediately: [false]
    });
  }

  toggleDeletionForm(): void {
    this.showDeletionForm = !this.showDeletionForm;
    if (!this.showDeletionForm) {
      this.deletionForm.reset();
    }
  }

  submitDeletion(): void {
    if (this.deletionForm.invalid) {
      this.deletionForm.markAllAsTouched();
      return;
    }

    const request: AccountDeletionRequest = this.deletionForm.value;
    this.accountDelete.emit(request);
  }

  getFieldError(fieldName: string): string {
    const field = this.deletionForm.get(fieldName);
    if (!field || !field.touched || !field.errors) return '';
    
    if (field.errors['required']) return 'This field is required';
    if (field.errors['email']) return 'Invalid email address';
    
    return 'Invalid value';
  }
}
