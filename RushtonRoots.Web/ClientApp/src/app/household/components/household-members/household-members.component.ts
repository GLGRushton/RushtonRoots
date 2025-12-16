import { Component, Input, Output, EventEmitter } from '@angular/core';
import { HouseholdMemberDetails, MemberActionEvent, HOUSEHOLD_ROLES } from '../../models/household-details.model';

/**
 * HouseholdMembersComponent - Displays and manages household members
 * 
 * Features:
 * - Member list with cards
 * - Role badges
 * - Member actions (edit, change role, remove)
 * - Invite new members
 * - Status indicators (active, invited)
 */
@Component({
  selector: 'app-household-members',
  standalone: false,
  templateUrl: './household-members.component.html',
  styleUrls: ['./household-members.component.scss']
})
export class HouseholdMembersComponent {
  @Input() members: HouseholdMemberDetails[] = [];
  @Input() canManage = false;
  @Input() currentUserId?: number;

  @Output() memberAction = new EventEmitter<MemberActionEvent>();
  @Output() inviteMember = new EventEmitter<void>();

  roles = HOUSEHOLD_ROLES;

  onInviteMember(): void {
    this.inviteMember.emit();
  }

  onEditMember(member: HouseholdMemberDetails): void {
    this.memberAction.emit({
      action: 'edit',
      memberId: member.personId,
      memberPersonId: member.personId
    });
  }

  onChangeRole(member: HouseholdMemberDetails): void {
    this.memberAction.emit({
      action: 'change-role',
      memberId: member.personId,
      memberPersonId: member.personId,
      data: { currentRole: member.role }
    });
  }

  onRemoveMember(member: HouseholdMemberDetails): void {
    if (confirm(`Are you sure you want to remove ${member.fullName} from this household?`)) {
      this.memberAction.emit({
        action: 'remove',
        memberId: member.personId,
        memberPersonId: member.personId
      });
    }
  }

  onResendInvite(member: HouseholdMemberDetails): void {
    this.memberAction.emit({
      action: 'resend-invite',
      memberId: member.personId,
      memberPersonId: member.personId
    });
  }

  onViewProfile(member: HouseholdMemberDetails): void {
    this.memberAction.emit({
      action: 'view-profile',
      memberId: member.personId,
      memberPersonId: member.personId
    });
  }

  getRoleConfig(role: string) {
    return this.roles.find(r => r.role === role) || this.roles[2]; // Default to Member
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Active': return 'accent';
      case 'Invited': return 'primary';
      case 'Inactive': return 'warn';
      default: return '';
    }
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'Active': return 'check_circle';
      case 'Invited': return 'mail';
      case 'Inactive': return 'cancel';
      default: return 'help';
    }
  }

  isCurrentUser(member: HouseholdMemberDetails): boolean {
    return this.currentUserId === member.personId;
  }

  canModifyMember(member: HouseholdMemberDetails): boolean {
    if (!this.canManage) return false;
    if (member.role === 'Owner') return false; // Cannot modify owner
    if (this.isCurrentUser(member)) return false; // Cannot modify self
    return true;
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'N/A';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }

  getActiveMembers(): HouseholdMemberDetails[] {
    return this.members.filter(m => m.status === 'Active');
  }

  getInvitedMembers(): HouseholdMemberDetails[] {
    return this.members.filter(m => m.status === 'Invited');
  }

  getInactiveMembers(): HouseholdMemberDetails[] {
    return this.members.filter(m => m.status === 'Inactive');
  }
}
