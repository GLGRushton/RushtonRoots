import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { 
  PersonDetails, 
  TimelineEvent, 
  PersonRelationship, 
  PersonPhoto,
  PersonDetailsTab 
} from '../../models/person-details.model';

/**
 * PersonDetailsComponent - Main component for displaying detailed person information
 * 
 * Features:
 * - Tabbed interface (Overview, Timeline, Relationships, Photos)
 * - Edit-in-place functionality
 * - Action buttons (edit, delete, share)
 * - Responsive design
 * - Integrates all sub-components
 */
@Component({
  selector: 'app-person-details',
  standalone: false,
  templateUrl: './person-details.component.html',
  styleUrls: ['./person-details.component.scss']
})
export class PersonDetailsComponent implements OnInit {
  @Input() person!: PersonDetails;
  @Input() timelineEvents: TimelineEvent[] = [];
  @Input() relationships: PersonRelationship[] = [];
  @Input() photos: PersonPhoto[] = [];
  @Input() canEdit = false;
  @Input() canDelete = false;
  @Input() initialTab: number = 0;

  @Output() editClicked = new EventEmitter<number>();
  @Output() deleteClicked = new EventEmitter<number>();
  @Output() shareClicked = new EventEmitter<number>();
  @Output() relationshipPersonClicked = new EventEmitter<number>();
  @Output() photoUploaded = new EventEmitter<File>();
  @Output() photoDeleted = new EventEmitter<number>();
  @Output() photoPrimaryChanged = new EventEmitter<number>();
  @Output() fieldUpdated = new EventEmitter<{ field: string, value: any }>();

  selectedTabIndex = 0;
  isEditingBio = false;
  editedBio = '';

  tabs: PersonDetailsTab[] = [
    { label: 'Overview', icon: 'person', content: 'overview' },
    { label: 'Timeline', icon: 'timeline', content: 'timeline' },
    { label: 'Relationships', icon: 'people', content: 'relationships' },
    { label: 'Photos', icon: 'photo_library', content: 'photos' }
  ];

  ngOnInit(): void {
    this.selectedTabIndex = this.initialTab;
  }

  onEdit(): void {
    this.editClicked.emit(this.person.id);
  }

  onDelete(): void {
    if (confirm(`Are you sure you want to delete ${this.person.fullName}?`)) {
      this.deleteClicked.emit(this.person.id);
    }
  }

  onShare(): void {
    this.shareClicked.emit(this.person.id);
  }

  onRelationshipPersonClick(personId: number): void {
    this.relationshipPersonClicked.emit(personId);
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

  startEditingBio(): void {
    this.isEditingBio = true;
    this.editedBio = this.person.biography || '';
  }

  saveBio(): void {
    this.fieldUpdated.emit({ field: 'biography', value: this.editedBio });
    this.isEditingBio = false;
  }

  cancelEditingBio(): void {
    this.isEditingBio = false;
    this.editedBio = '';
  }

  formatDate(date?: Date | string): string {
    if (!date) return 'Unknown';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  }

  getAge(): number | null {
    if (!this.person.dateOfBirth) return null;

    const birth = new Date(this.person.dateOfBirth);
    const endDate = this.person.isDeceased && this.person.dateOfDeath 
      ? new Date(this.person.dateOfDeath)
      : new Date();

    let age = endDate.getFullYear() - birth.getFullYear();
    const monthDiff = endDate.getMonth() - birth.getMonth();

    if (monthDiff < 0 || (monthDiff === 0 && endDate.getDate() < birth.getDate())) {
      age--;
    }

    return age >= 0 ? age : null;
  }

  getLifespan(): string {
    const age = this.getAge();
    if (age === null) return '';
    
    if (this.person.isDeceased) {
      return `Lived ${age} years`;
    } else {
      return `Age ${age}`;
    }
  }

  getPrimaryPhoto(): string {
    if (this.person.photoUrl) {
      return this.person.photoUrl;
    }
    
    const primaryPhoto = this.photos.find(p => p.isPrimary);
    if (primaryPhoto) {
      return primaryPhoto.photoUrl;
    }

    return '/images/default-avatar.png';
  }

  handleImageError(event: any): void {
    event.target.src = '/images/default-avatar.png';
  }

  copyShareLink(): void {
    const url = window.location.href;
    navigator.clipboard.writeText(url).then(() => {
      alert('Link copied to clipboard!');
    });
  }
}
