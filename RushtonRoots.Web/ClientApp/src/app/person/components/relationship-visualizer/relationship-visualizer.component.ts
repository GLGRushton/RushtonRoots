import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { PersonRelationship, RelatedPersonInfo } from '../../models/person-details.model';

/**
 * RelationshipVisualizerComponent - Displays family relationships in an organized view
 * 
 * Features:
 * - Shows parents, spouses/partners, children
 * - Clickable cards to navigate to related persons
 * - Visual grouping by relationship type
 * - Handles missing relationships gracefully
 */
@Component({
  selector: 'app-relationship-visualizer',
  standalone: false,
  templateUrl: './relationship-visualizer.component.html',
  styleUrls: ['./relationship-visualizer.component.scss']
})
export class RelationshipVisualizerComponent implements OnInit {
  @Input() personId!: number;
  @Input() personName!: string;
  @Input() relationships: PersonRelationship[] = [];
  @Output() personClicked = new EventEmitter<number>();

  parents: PersonRelationship[] = [];
  spouses: PersonRelationship[] = [];
  children: PersonRelationship[] = [];
  siblings: PersonRelationship[] = [];

  ngOnInit(): void {
    this.categorizeRelationships();
  }

  ngOnChanges(): void {
    this.categorizeRelationships();
  }

  private categorizeRelationships(): void {
    this.parents = this.relationships.filter(r => r.relationshipType === 'parent');
    this.spouses = this.relationships.filter(r => 
      r.relationshipType === 'spouse' || r.relationshipType === 'partner'
    );
    this.children = this.relationships.filter(r => r.relationshipType === 'child');
    this.siblings = this.relationships.filter(r => r.relationshipType === 'sibling');
  }

  onPersonClick(personId: number): void {
    this.personClicked.emit(personId);
  }

  formatDate(date?: Date | string): string {
    if (!date) return '';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.getFullYear().toString();
  }

  getLifeSpan(person: RelatedPersonInfo): string {
    const birth = person.dateOfBirth ? this.formatDate(person.dateOfBirth) : '?';
    const death = person.isDeceased && person.dateOfDeath ? this.formatDate(person.dateOfDeath) : (person.isDeceased ? '?' : 'Present');
    return `${birth} - ${death}`;
  }

  getRelationshipDuration(relationship: PersonRelationship): string {
    if (!relationship.startDate) return '';
    
    const start = this.formatDate(relationship.startDate);
    const end = relationship.endDate ? this.formatDate(relationship.endDate) : 'Present';
    
    return `${start} - ${end}`;
  }

  getDefaultPhotoUrl(): string {
    return '/images/default-avatar.png';
  }

  handleImageError(event: any): void {
    event.target.src = this.getDefaultPhotoUrl();
  }
}
