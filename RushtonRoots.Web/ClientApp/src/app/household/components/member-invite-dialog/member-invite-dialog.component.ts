import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MemberInvitation, HouseholdRole, HOUSEHOLD_ROLES } from '../../models/household-details.model';

/**
 * MemberInviteDialogComponent - Dialog for inviting new household members
 * 
 * Features:
 * - Email input with validation
 * - Name fields (optional)
 * - Role selection
 * - Personal message (optional)
 * - Form validation
 */
@Component({
  selector: 'app-member-invite-dialog',
  standalone: false,
  templateUrl: './member-invite-dialog.component.html',
  styleUrls: ['./member-invite-dialog.component.scss']
})
export class MemberInviteDialogComponent {
  invitation: MemberInvitation = {
    email: '',
    firstName: '',
    lastName: '',
    role: 'Member',
    personalMessage: ''
  };

  roles = HOUSEHOLD_ROLES.filter(r => r.role !== 'Owner'); // Cannot invite as Owner
  emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

  constructor(
    public dialogRef: MatDialogRef<MemberInviteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { householdName: string }
  ) {}

  onCancel(): void {
    this.dialogRef.close();
  }

  onSend(): void {
    if (this.isValid()) {
      this.dialogRef.close(this.invitation);
    }
  }

  isValid(): boolean {
    return this.emailPattern.test(this.invitation.email) && 
           this.invitation.role !== undefined;
  }

  isEmailValid(): boolean {
    return this.invitation.email === '' || this.emailPattern.test(this.invitation.email);
  }

  getRoleDescription(role: HouseholdRole): string {
    const roleConfig = this.roles.find(r => r.role === role);
    return roleConfig ? roleConfig.description : '';
  }
}
