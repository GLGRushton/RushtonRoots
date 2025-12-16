import { Component, Input, OnInit } from '@angular/core';
import { TimelineEvent } from '../../models/person-details.model';

/**
 * PersonTimelineComponent - Displays a timeline of significant life events
 * 
 * Features:
 * - Vertical timeline with icons
 * - Auto-populated birth/death events
 * - Chronologically sorted events
 * - Event type icons and colors
 */
@Component({
  selector: 'app-person-timeline',
  standalone: false,
  templateUrl: './person-timeline.component.html',
  styleUrls: ['./person-timeline.component.scss']
})
export class PersonTimelineComponent implements OnInit {
  @Input() personId!: number;
  @Input() events: TimelineEvent[] = [];
  @Input() dateOfBirth?: Date | string;
  @Input() dateOfDeath?: Date | string;
  @Input() isDeceased = false;

  sortedEvents: TimelineEvent[] = [];

  ngOnInit(): void {
    this.sortedEvents = this.buildTimeline();
  }

  ngOnChanges(): void {
    this.sortedEvents = this.buildTimeline();
  }

  private buildTimeline(): TimelineEvent[] {
    const allEvents: TimelineEvent[] = [...this.events];

    // Add birth event if we have a date of birth
    if (this.dateOfBirth) {
      allEvents.push({
        id: -1,
        personId: this.personId,
        title: 'Birth',
        date: this.dateOfBirth,
        eventType: 'birth',
        icon: 'cake',
        description: 'Born'
      });
    }

    // Add death event if person is deceased and we have a date
    if (this.isDeceased && this.dateOfDeath) {
      allEvents.push({
        id: -2,
        personId: this.personId,
        title: 'Death',
        date: this.dateOfDeath,
        eventType: 'death',
        icon: 'sentiment_dissatisfied',
        description: 'Passed away'
      });
    }

    // Sort events chronologically
    return allEvents.sort((a, b) => {
      const dateA = new Date(a.date).getTime();
      const dateB = new Date(b.date).getTime();
      return dateA - dateB;
    });
  }

  getEventIcon(event: TimelineEvent): string {
    if (event.icon) {
      return event.icon;
    }

    switch (event.eventType) {
      case 'birth':
        return 'cake';
      case 'death':
        return 'sentiment_dissatisfied';
      case 'marriage':
        return 'favorite';
      case 'education':
        return 'school';
      case 'career':
        return 'work';
      case 'milestone':
        return 'star';
      default:
        return 'event';
    }
  }

  getEventColor(event: TimelineEvent): string {
    switch (event.eventType) {
      case 'birth':
        return 'primary';
      case 'death':
        return 'warn';
      case 'marriage':
        return 'accent';
      case 'education':
        return 'primary';
      case 'career':
        return 'primary';
      case 'milestone':
        return 'accent';
      default:
        return 'primary';
    }
  }

  formatDate(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  }

  getAge(eventDate: Date | string): number | null {
    if (!this.dateOfBirth) {
      return null;
    }

    const birth = new Date(this.dateOfBirth);
    const event = new Date(eventDate);
    
    let age = event.getFullYear() - birth.getFullYear();
    const monthDiff = event.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && event.getDate() < birth.getDate())) {
      age--;
    }
    
    return age >= 0 ? age : null;
  }
}
