import { Component, Input } from '@angular/core';

/**
 * PersonCardComponent - Displays a person summary in a card format
 * 
 * Usage:
 * <app-person-card 
 *   [personId]="1"
 *   [firstName]="'John'"
 *   [lastName]="'Doe'"
 *   [birthDate]="'1980-01-15'"
 *   [deathDate]="'2050-12-31'"
 *   [photoUrl]="'https://example.com/photo.jpg'"
 *   [showActions]="true">
 * </app-person-card>
 */
@Component({
  selector: 'app-person-card',
  standalone: false,
  templateUrl: './person-card.component.html',
  styleUrls: ['./person-card.component.scss']
})
export class PersonCardComponent {
  @Input() personId?: number;
  @Input() firstName: string = '';
  @Input() lastName: string = '';
  @Input() birthDate?: string;
  @Input() deathDate?: string;
  @Input() photoUrl?: string;
  @Input() showActions: boolean = false;

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`.trim();
  }

  get lifeSpan(): string {
    const birth = this.birthDate ? new Date(this.birthDate).getFullYear() : '?';
    const death = this.deathDate ? new Date(this.deathDate).getFullYear() : 'Present';
    return `${birth} - ${death}`;
  }

  get initials(): string {
    const firstInitial = this.firstName?.charAt(0)?.toUpperCase() || '';
    const lastInitial = this.lastName?.charAt(0)?.toUpperCase() || '';
    return `${firstInitial}${lastInitial}`;
  }

  onViewDetails(): void {
    if (this.personId) {
      window.location.href = `/Person/Details/${this.personId}`;
    }
  }

  onEdit(): void {
    if (this.personId) {
      window.location.href = `/Person/Edit/${this.personId}`;
    }
  }
}
