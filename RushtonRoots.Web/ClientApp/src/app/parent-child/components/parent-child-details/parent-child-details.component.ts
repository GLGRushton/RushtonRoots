import { Component, Input, Output, EventEmitter, OnInit, OnChanges } from '@angular/core';
import { 
  ParentChildDetails, 
  ParentChildDetailsTab,
  ParentChildEvidence,
  ParentChildEvent,
  FamilyTreeNode
} from '../../models/parent-child.model';

/**
 * ParentChildDetailsComponent - Main component for displaying detailed parent-child relationship information
 * Phase 5.2: ParentChild Details
 * 
 * Features:
 * - Tabbed interface (Overview, Family Context, Evidence, Timeline)
 * - Relationship summary with parent and child information
 * - Integration with FamilyTreeMiniComponent for family context
 * - Evidence tracking section
 * - Timeline with key events
 * - Action buttons (edit, delete, verify)
 * - Responsive design
 */
@Component({
  selector: 'app-parent-child-details',
  standalone: false,
  templateUrl: './parent-child-details.component.html',
  styleUrls: ['./parent-child-details.component.scss']
})
export class ParentChildDetailsComponent implements OnInit, OnChanges {
  @Input() relationship!: ParentChildDetails;
  @Input() evidence: ParentChildEvidence[] = [];
  @Input() events: ParentChildEvent[] = [];
  @Input() grandparents: FamilyTreeNode[] = [];
  @Input() siblings: FamilyTreeNode[] = [];
  @Input() canEdit = false;
  @Input() canDelete = false;
  @Input() canVerify = false;
  @Input() initialTab: number = 0;

  @Output() editClicked = new EventEmitter<number>();
  @Output() deleteClicked = new EventEmitter<number>();
  @Output() verifyClicked = new EventEmitter<number>();
  @Output() personClicked = new EventEmitter<number>();
  @Output() evidenceAdded = new EventEmitter<ParentChildEvidence>();
  @Output() evidenceDeleted = new EventEmitter<number>();
  @Output() noteUpdated = new EventEmitter<string>();

  selectedTabIndex = 0;
  isEditingNotes = false;
  editedNotes = '';

  tabs: ParentChildDetailsTab[] = [
    { label: 'Overview', icon: 'info', content: 'overview' },
    { label: 'Family Context', icon: 'account_tree', content: 'family', badge: 0 },
    { label: 'Evidence', icon: 'fact_check', content: 'evidence', badge: 0 },
    { label: 'Timeline', icon: 'timeline', content: 'timeline', badge: 0 }
  ];

  ngOnInit(): void {
    this.selectedTabIndex = this.initialTab;
    this.updateBadges();
  }

  ngOnChanges(): void {
    this.updateBadges();
  }

  private updateBadges(): void {
    const familyTab = this.tabs.find(t => t.content === 'family');
    if (familyTab) {
      familyTab.badge = (this.grandparents?.length || 0) + (this.siblings?.length || 0);
    }

    const evidenceTab = this.tabs.find(t => t.content === 'evidence');
    if (evidenceTab) {
      evidenceTab.badge = this.evidence?.length || 0;
    }

    const timelineTab = this.tabs.find(t => t.content === 'timeline');
    if (timelineTab) {
      timelineTab.badge = this.events?.length || 0;
    }
  }

  onEdit(): void {
    this.editClicked.emit(this.relationship.id);
  }

  onDelete(): void {
    if (confirm(`Are you sure you want to delete this relationship?`)) {
      this.deleteClicked.emit(this.relationship.id);
    }
  }

  onVerify(): void {
    this.verifyClicked.emit(this.relationship.id);
  }

  onParentClick(): void {
    this.personClicked.emit(this.relationship.parentPersonId);
  }

  onChildClick(): void {
    this.personClicked.emit(this.relationship.childPersonId);
  }

  onPersonFromTreeClick(personId: number): void {
    this.personClicked.emit(personId);
  }

  onEditNotes(): void {
    this.isEditingNotes = true;
    this.editedNotes = this.relationship.notes || '';
  }

  onSaveNotes(): void {
    this.noteUpdated.emit(this.editedNotes);
    this.isEditingNotes = false;
  }

  onCancelEditNotes(): void {
    this.isEditingNotes = false;
    this.editedNotes = '';
  }

  formatDate(date?: Date): string {
    if (!date) return 'Unknown';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  }

  getAge(birthDate?: Date, deathDate?: Date): string {
    if (!birthDate) return '';
    
    const birth = new Date(birthDate);
    const end = deathDate ? new Date(deathDate) : new Date();
    
    let age = end.getFullYear() - birth.getFullYear();
    const monthDiff = end.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && end.getDate() < birth.getDate())) {
      age--;
    }
    
    return deathDate ? `(${age} years)` : `(Age ${age})`;
  }

  getVerificationStatusColor(): string {
    return this.relationship.isVerified ? 'accent' : 'warn';
  }

  getVerificationStatusIcon(): string {
    return this.relationship.isVerified ? 'verified' : 'help_outline';
  }

  getVerificationStatusText(): string {
    return this.relationship.isVerified ? 'Verified' : 'Unverified';
  }

  getConfidenceColor(): string {
    const confidence = this.relationship.confidence || 0;
    if (confidence >= 80) return 'accent';
    if (confidence >= 50) return 'primary';
    return 'warn';
  }

  getEvidenceTypeIcon(type: string): string {
    const iconMap: { [key: string]: string } = {
      'source': 'source',
      'document': 'description',
      'dna': 'biotech',
      'photo': 'photo',
      'other': 'attach_file'
    };
    return iconMap[type] || 'attach_file';
  }
}
