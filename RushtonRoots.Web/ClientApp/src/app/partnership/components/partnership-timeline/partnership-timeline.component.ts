import { Component, Input, OnInit } from '@angular/core';
import { PartnershipCard, PartnershipTimeline, PartnershipTimelineEvent } from '../../models/partnership.model';

/**
 * PartnershipTimelineComponent - Displays partnership relationship timeline
 * Phase 5.1: Partnership Management
 */
@Component({
  selector: 'app-partnership-timeline',
  standalone: false,
  templateUrl: './partnership-timeline.component.html',
  styleUrls: ['./partnership-timeline.component.scss']
})
export class PartnershipTimelineComponent implements OnInit {
  /**
   * Partnership data
   */
  @Input() partnership!: PartnershipCard;

  /**
   * Timeline data with events
   */
  timeline?: PartnershipTimeline;

  ngOnInit(): void {
    this.buildTimeline();
  }

  /**
   * Build timeline from partnership data
   */
  private buildTimeline(): void {
    const events: PartnershipTimelineEvent[] = [];

    // Add start event
    if (this.partnership.startDate) {
      events.push({
        id: 1,
        date: new Date(this.partnership.startDate),
        dateDisplay: this.formatDate(new Date(this.partnership.startDate)),
        eventType: this.getStartEventType(),
        eventTypeDisplay: this.getStartEventTypeDisplay(),
        title: this.getStartEventTitle(),
        icon: this.getEventIcon('start'),
        color: '#2e7d32',
        location: this.partnership.location
      });
    }

    // Add end event if applicable
    if (this.partnership.endDate) {
      events.push({
        id: 2,
        date: new Date(this.partnership.endDate),
        dateDisplay: this.formatDate(new Date(this.partnership.endDate)),
        eventType: this.getEndEventType(),
        eventTypeDisplay: this.getEndEventTypeDisplay(),
        title: this.getEndEventTitle(),
        icon: this.getEventIcon('end'),
        color: this.getEndEventColor(),
        location: this.partnership.location
      });
    }

    // Sort events chronologically
    events.sort((a, b) => a.date.getTime() - b.date.getTime());

    // Calculate duration
    const duration = this.calculateDuration();
    const yearsActive = this.calculateYearsActive();

    this.timeline = {
      partnership: this.partnership,
      events,
      duration,
      yearsActive
    };
  }

  /**
   * Get start event type
   */
  private getStartEventType(): 'start' | 'marriage' | 'engagement' | 'other' {
    switch (this.partnership.partnershipType) {
      case 'married':
        return 'marriage';
      case 'engaged':
        return 'engagement';
      default:
        return 'start';
    }
  }

  /**
   * Get start event type display
   */
  private getStartEventTypeDisplay(): string {
    switch (this.partnership.partnershipType) {
      case 'married':
        return 'Marriage';
      case 'engaged':
        return 'Engagement';
      case 'partnered':
        return 'Partnership Began';
      default:
        return 'Relationship Began';
    }
  }

  /**
   * Get start event title
   */
  private getStartEventTitle(): string {
    return `${this.partnership.personAName} and ${this.partnership.personBName} ${this.getStartEventTypeDisplay().toLowerCase()}`;
  }

  /**
   * Get end event type
   */
  private getEndEventType(): 'end' | 'divorce' | 'separation' | 'other' {
    switch (this.partnership.status) {
      case 'divorced':
        return 'divorce';
      case 'separated':
        return 'separation';
      default:
        return 'end';
    }
  }

  /**
   * Get end event type display
   */
  private getEndEventTypeDisplay(): string {
    switch (this.partnership.status) {
      case 'divorced':
        return 'Divorce';
      case 'separated':
        return 'Separation';
      case 'widowed':
        return 'Partnership Ended (Widowed)';
      default:
        return 'Partnership Ended';
    }
  }

  /**
   * Get end event title
   */
  private getEndEventTitle(): string {
    return `Partnership ended: ${this.getEndEventTypeDisplay()}`;
  }

  /**
   * Get end event color
   */
  private getEndEventColor(): string {
    switch (this.partnership.status) {
      case 'divorced':
        return '#c62828';
      case 'separated':
        return '#f57c00';
      case 'widowed':
        return '#757575';
      default:
        return '#666';
    }
  }

  /**
   * Get event icon
   */
  private getEventIcon(eventType: string): string {
    switch (eventType) {
      case 'start':
      case 'marriage':
        return 'favorite';
      case 'engagement':
        return 'diamond';
      case 'divorce':
        return 'heart_broken';
      case 'separation':
        return 'pending';
      case 'end':
        return 'event_busy';
      default:
        return 'event';
    }
  }

  /**
   * Calculate duration string
   */
  private calculateDuration(): string {
    if (!this.partnership.startDate) {
      return 'Unknown duration';
    }

    const start = new Date(this.partnership.startDate);
    const end = this.partnership.endDate ? new Date(this.partnership.endDate) : new Date();

    // Calculate total months difference
    let years = end.getFullYear() - start.getFullYear();
    let months = end.getMonth() - start.getMonth();

    // Adjust for negative months
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

  /**
   * Calculate years active (more accurate with partial years)
   */
  private calculateYearsActive(): number {
    if (!this.partnership.startDate) {
      return 0;
    }

    const start = new Date(this.partnership.startDate);
    const end = this.partnership.endDate ? new Date(this.partnership.endDate) : new Date();

    // Calculate total months and convert to years (rounded)
    let years = end.getFullYear() - start.getFullYear();
    let months = end.getMonth() - start.getMonth();
    
    if (months < 0) {
      years--;
      months += 12;
    }

    const totalMonths = years * 12 + months;
    return Math.max(0, Math.round(totalMonths / 12));
  }

  /**
   * Format date for display
   */
  private formatDate(date: Date): string {
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
