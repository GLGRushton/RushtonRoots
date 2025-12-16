import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { 
  PartnershipDetails, 
  PartnershipDetailsTab,
  PartnershipChild,
  PartnershipPhoto,
  PartnershipEvent
} from '../../models/partnership.model';

/**
 * PartnershipDetailsComponent - Main component for displaying detailed partnership information
 * Phase 4.2: Partnership Details
 * 
 * Features:
 * - Tabbed interface (Overview, Timeline, Children, Media, Events)
 * - Partnership summary with both partners
 * - Timeline integration
 * - Children list
 * - Media/photo gallery
 * - Events listing
 * - Action buttons (edit, delete)
 * - Responsive design
 */
@Component({
  selector: 'app-partnership-details',
  standalone: false,
  templateUrl: './partnership-details.component.html',
  styleUrls: ['./partnership-details.component.scss']
})
export class PartnershipDetailsComponent implements OnInit {
  @Input() partnership!: PartnershipDetails;
  @Input() children: PartnershipChild[] = [];
  @Input() photos: PartnershipPhoto[] = [];
  @Input() events: PartnershipEvent[] = [];
  @Input() canEdit = false;
  @Input() canDelete = false;
  @Input() initialTab: number = 0;

  @Output() editClicked = new EventEmitter<number>();
  @Output() deleteClicked = new EventEmitter<number>();
  @Output() personClicked = new EventEmitter<number>();
  @Output() childClicked = new EventEmitter<number>();
  @Output() photoUploaded = new EventEmitter<File>();
  @Output() photoDeleted = new EventEmitter<number>();
  @Output() photoPrimaryChanged = new EventEmitter<number>();
  @Output() eventAdded = new EventEmitter<PartnershipEvent>();

  selectedTabIndex = 0;
  isEditingNotes = false;
  editedNotes = '';
  isEditingDescription = false;
  editedDescription = '';

  tabs: PartnershipDetailsTab[] = [
    { label: 'Overview', icon: 'favorite', content: 'overview' },
    { label: 'Timeline', icon: 'timeline', content: 'timeline' },
    { label: 'Children', icon: 'family_restroom', content: 'children', badge: 0 },
    { label: 'Media', icon: 'photo_library', content: 'media', badge: 0 },
    { label: 'Events', icon: 'event', content: 'events', badge: 0 }
  ];

  ngOnInit(): void {
    this.selectedTabIndex = this.initialTab;
    this.updateBadges();
  }

  ngOnChanges(): void {
    this.updateBadges();
  }

  private updateBadges(): void {
    const childrenTab = this.tabs.find(t => t.content === 'children');
    if (childrenTab) {
      childrenTab.badge = this.children.length;
    }

    const mediaTab = this.tabs.find(t => t.content === 'media');
    if (mediaTab) {
      mediaTab.badge = this.photos.length;
    }

    const eventsTab = this.tabs.find(t => t.content === 'events');
    if (eventsTab) {
      eventsTab.badge = this.events.length;
    }
  }

  onEdit(): void {
    this.editClicked.emit(this.partnership.id);
  }

  onDelete(): void {
    if (confirm(`Are you sure you want to delete this partnership?`)) {
      this.deleteClicked.emit(this.partnership.id);
    }
  }

  onPersonAClick(): void {
    this.personClicked.emit(this.partnership.personAId);
  }

  onPersonBClick(): void {
    this.personClicked.emit(this.partnership.personBId);
  }

  onChildClick(childId: number): void {
    this.childClicked.emit(childId);
  }

  onPhotoUpload(file: File): void {
    this.photoUploaded.emit(file);
  }

  onPhotoDelete(photoId: number): void {
    this.photoDeleted.emit(photoId);
  }

  onPhotoPrimaryChange(photoId: number): void {
    this.photoPrimaryChanged.emit(photoId);
  }

  onEventAdd(event: PartnershipEvent): void {
    this.eventAdded.emit(event);
  }

  startEditingNotes(): void {
    this.isEditingNotes = true;
    this.editedNotes = this.partnership.notes || '';
  }

  saveNotes(): void {
    // Emit event to save notes
    this.isEditingNotes = false;
  }

  cancelEditingNotes(): void {
    this.isEditingNotes = false;
    this.editedNotes = '';
  }

  startEditingDescription(): void {
    this.isEditingDescription = true;
    this.editedDescription = this.partnership.description || '';
  }

  saveDescription(): void {
    // Emit event to save description
    this.isEditingDescription = false;
  }

  cancelEditingDescription(): void {
    this.isEditingDescription = false;
    this.editedDescription = '';
  }

  getDuration(): string {
    if (!this.partnership.duration) {
      return this.calculateDuration();
    }
    return this.partnership.duration;
  }

  private calculateDuration(): string {
    if (!this.partnership.startDate) {
      return 'Unknown duration';
    }

    const start = new Date(this.partnership.startDate);
    const end = this.partnership.endDate ? new Date(this.partnership.endDate) : new Date();

    let years = end.getFullYear() - start.getFullYear();
    let months = end.getMonth() - start.getMonth();

    if (months < 0) {
      years--;
      months += 12;
    }

    if (years === 0 && months === 0) {
      return 'Less than a month';
    } else if (years === 0) {
      return `${months} ${months === 1 ? 'month' : 'months'}`;
    } else if (months === 0) {
      return `${years} ${years === 1 ? 'year' : 'years'}`;
    } else {
      return `${years} ${years === 1 ? 'year' : 'years'}, ${months} ${months === 1 ? 'month' : 'months'}`;
    }
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'N/A';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  getStatusColor(): string {
    switch (this.partnership.status) {
      case 'current':
        return 'primary';
      case 'divorced':
        return 'warn';
      case 'widowed':
      case 'separated':
      case 'ended':
        return 'accent';
      default:
        return 'accent';
    }
  }

  getStatusIcon(): string {
    switch (this.partnership.status) {
      case 'current':
        return 'favorite';
      case 'divorced':
        return 'heart_broken';
      case 'widowed':
        return 'sentiment_very_dissatisfied';
      case 'separated':
        return 'pending';
      case 'ended':
        return 'history';
      default:
        return 'help_outline';
    }
  }

  getPartnershipTypeIcon(): string {
    switch (this.partnership.partnershipType) {
      case 'married':
        return 'favorite';
      case 'engaged':
        return 'diamond';
      case 'partnered':
        return 'volunteer_activism';
      case 'relationship':
        return 'people';
      case 'commonlaw':
        return 'handshake';
      default:
        return 'more_horiz';
    }
  }
}
