import { Component, Input, Output, EventEmitter, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { MediaItem, MediaTag } from '../../models/media-gallery.model';

interface PersonOption {
  id: number;
  name: string;
  avatarUrl?: string;
}

/**
 * PhotoTaggingComponent - Interface for tagging people in photos
 * 
 * Features:
 * - Click on photo to add person tags
 * - Position tags on faces
 * - Search and autocomplete for people
 * - View and edit existing tags
 * - Remove tags
 */
@Component({
  selector: 'app-photo-tagging',
  standalone: false,
  templateUrl: './photo-tagging.component.html',
  styleUrls: ['./photo-tagging.component.scss']
})
export class PhotoTaggingComponent implements OnInit {
  @Input() photo!: MediaItem;
  @Input() availablePeople: PersonOption[] = [];
  @Output() tagAdded = new EventEmitter<MediaTag>();
  @Output() tagRemoved = new EventEmitter<number>();
  @Output() tagMoved = new EventEmitter<{ tagId: number; x: number; y: number }>();
  @Output() close = new EventEmitter<void>();

  @ViewChild('photoImage') photoImage?: ElementRef<HTMLImageElement>;

  tags: MediaTag[] = [];
  isAddingTag = false;
  newTagPosition?: { x: number; y: number };
  selectedPerson?: PersonOption;
  personControl = new FormControl('');
  filteredPeople$?: Observable<PersonOption[]>;
  showTagList = true;

  ngOnInit(): void {
    this.tags = [...(this.photo.tags || [])];
    this.setupPersonAutocomplete();
  }

  private setupPersonAutocomplete(): void {
    this.filteredPeople$ = this.personControl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterPeople(value || ''))
    );
  }

  private filterPeople(value: string): PersonOption[] {
    if (!value) return this.availablePeople;
    const filterValue = value.toLowerCase();
    return this.availablePeople.filter(person =>
      person.name.toLowerCase().includes(filterValue)
    );
  }

  onPhotoClick(event: MouseEvent): void {
    if (!this.isAddingTag || !this.photoImage) return;

    const rect = this.photoImage.nativeElement.getBoundingClientRect();
    const x = ((event.clientX - rect.left) / rect.width) * 100;
    const y = ((event.clientY - rect.top) / rect.height) * 100;

    this.newTagPosition = { x, y };
  }

  startAddingTag(): void {
    this.isAddingTag = true;
    this.newTagPosition = undefined;
    this.selectedPerson = undefined;
    this.personControl.setValue('');
  }

  cancelAddingTag(): void {
    this.isAddingTag = false;
    this.newTagPosition = undefined;
    this.selectedPerson = undefined;
  }

  selectPerson(person: PersonOption): void {
    this.selectedPerson = person;
  }

  confirmTag(): void {
    if (!this.selectedPerson || !this.newTagPosition) return;

    const newTag: MediaTag = {
      id: Date.now(), // Temporary ID
      name: this.selectedPerson.name,
      personId: this.selectedPerson.id,
      personName: this.selectedPerson.name,
      x: this.newTagPosition.x,
      y: this.newTagPosition.y
    };

    this.tags.push(newTag);
    this.tagAdded.emit(newTag);
    this.cancelAddingTag();
  }

  removeTag(tag: MediaTag): void {
    const index = this.tags.findIndex(t => t.id === tag.id);
    if (index > -1) {
      this.tags.splice(index, 1);
      this.tagRemoved.emit(tag.id);
    }
  }

  onTagDragStart(event: DragEvent, tag: MediaTag): void {
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
      event.dataTransfer.setData('tagId', tag.id.toString());
    }
  }

  onPhotoDrop(event: DragEvent): void {
    event.preventDefault();
    if (!event.dataTransfer || !this.photoImage) return;

    const tagId = parseInt(event.dataTransfer.getData('tagId'));
    const tag = this.tags.find(t => t.id === tagId);
    if (!tag) return;

    const rect = this.photoImage.nativeElement.getBoundingClientRect();
    const x = ((event.clientX - rect.left) / rect.width) * 100;
    const y = ((event.clientY - rect.top) / rect.height) * 100;

    tag.x = x;
    tag.y = y;
    this.tagMoved.emit({ tagId, x, y });
  }

  onPhotoDragOver(event: DragEvent): void {
    event.preventDefault();
    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = 'move';
    }
  }

  toggleTagList(): void {
    this.showTagList = !this.showTagList;
  }

  onClose(): void {
    this.close.emit();
  }

  getTagStyle(tag: MediaTag): any {
    return {
      left: `${tag.x}%`,
      top: `${tag.y}%`
    };
  }

  displayPersonName(person: PersonOption | null): string {
    return person ? person.name : '';
  }
}
