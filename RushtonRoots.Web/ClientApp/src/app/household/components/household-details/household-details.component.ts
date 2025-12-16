import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { 
  HouseholdDetails, 
  HouseholdDetailsTab,
  HouseholdMemberDetails,
  HouseholdActivityEvent,
  MemberActionEvent
} from '../../models/household-details.model';

/**
 * HouseholdDetailsComponent - Main component for displaying detailed household information
 * 
 * Features:
 * - Tabbed interface (Overview, Members, Settings, Activity)
 * - Member management
 * - Permission controls
 * - Settings panel
 * - Activity timeline
 * - Responsive design
 */
@Component({
  selector: 'app-household-details',
  standalone: false,
  templateUrl: './household-details.component.html',
  styleUrls: ['./household-details.component.scss']
})
export class HouseholdDetailsComponent implements OnInit {
  @Input() household!: HouseholdDetails;
  @Input() members: HouseholdMemberDetails[] = [];
  @Input() activityEvents: HouseholdActivityEvent[] = [];
  @Input() canEdit = false;
  @Input() canDelete = false;
  @Input() canManageMembers = false;
  @Input() canEditSettings = false;
  @Input() currentUserId?: number;
  @Input() initialTab: number = 0;

  @Output() editClicked = new EventEmitter<number>();
  @Output() deleteClicked = new EventEmitter<number>();
  @Output() memberActionClicked = new EventEmitter<MemberActionEvent>();
  @Output() inviteMemberClicked = new EventEmitter<void>();
  @Output() settingsUpdated = new EventEmitter<any>();
  @Output() anchorPersonClicked = new EventEmitter<number>();

  selectedTabIndex = 0;
  isEditingDescription = false;
  editedDescription = '';

  tabs: HouseholdDetailsTab[] = [
    { label: 'Overview', icon: 'home', content: 'overview' },
    { label: 'Members', icon: 'people', content: 'members', badge: 0 },
    { label: 'Settings', icon: 'settings', content: 'settings' },
    { label: 'Activity', icon: 'history', content: 'activity' }
  ];

  ngOnInit(): void {
    this.selectedTabIndex = this.initialTab;
    this.updateMemberBadge();
  }

  ngOnChanges(): void {
    this.updateMemberBadge();
  }

  private updateMemberBadge(): void {
    const membersTab = this.tabs.find(t => t.content === 'members');
    if (membersTab) {
      membersTab.badge = this.members.length;
    }
  }

  onEdit(): void {
    this.editClicked.emit(this.household.id);
  }

  onDelete(): void {
    if (confirm(`Are you sure you want to delete the household "${this.household.householdName}"?`)) {
      this.deleteClicked.emit(this.household.id);
    }
  }

  onMemberAction(event: MemberActionEvent): void {
    this.memberActionClicked.emit(event);
  }

  onInviteMember(): void {
    this.inviteMemberClicked.emit();
  }

  onSettingsUpdate(settings: any): void {
    this.settingsUpdated.emit(settings);
  }

  onAnchorPersonClick(): void {
    if (this.household.anchorPersonId) {
      this.anchorPersonClicked.emit(this.household.anchorPersonId);
    }
  }

  startEditingDescription(): void {
    this.isEditingDescription = true;
    this.editedDescription = this.household.description || '';
  }

  saveDescription(): void {
    this.settingsUpdated.emit({ description: this.editedDescription });
    this.isEditingDescription = false;
  }

  cancelEditingDescription(): void {
    this.isEditingDescription = false;
    this.editedDescription = '';
  }

  getPrivacyIcon(): string {
    switch (this.household.privacy) {
      case 'Public': return 'public';
      case 'FamilyOnly': return 'group';
      case 'Private': return 'lock';
      default: return 'info';
    }
  }

  getPrivacyColor(): string {
    switch (this.household.privacy) {
      case 'Public': return 'accent';
      case 'FamilyOnly': return 'primary';
      case 'Private': return 'warn';
      default: return '';
    }
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'N/A';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  }
}
