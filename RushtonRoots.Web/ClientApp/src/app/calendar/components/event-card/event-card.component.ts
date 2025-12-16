import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CalendarEvent, RsvpStatus, EVENT_CATEGORIES, RSVP_STATUSES } from '../../models/calendar.model';

/**
 * Phase 8.2: Event Card Component
 * 
 * Displays a calendar event in card format with event details, RSVP status,
 * and quick action buttons.
 * 
 * Features:
 * - Event metadata display (title, date, time, location)
 * - RSVP status indicator
 * - Attendee count
 * - Quick action buttons (view, edit, delete, RSVP)
 * - Category chip with icon and color
 * - Material Design styling
 */
@Component({
  selector: 'app-event-card',
  standalone: false,
  templateUrl: './event-card.component.html',
  styleUrls: ['./event-card.component.scss']
})
export class EventCardComponent {
  @Input() event!: CalendarEvent;
  @Input() canEdit: boolean = false;
  @Input() compact: boolean = false;
  @Input() elevation: number = 2;

  @Output() viewClick = new EventEmitter<CalendarEvent>();
  @Output() editClick = new EventEmitter<CalendarEvent>();
  @Output() deleteClick = new EventEmitter<CalendarEvent>();
  @Output() rsvpClick = new EventEmitter<CalendarEvent>();

  eventCategories = EVENT_CATEGORIES;
  rsvpStatuses = RSVP_STATUSES;

  /**
   * Get category configuration
   */
  getCategoryConfig() {
    if (!this.event.category) return null;
    return this.eventCategories.find(c => c.value === this.event.category);
  }

  /**
   * Get current user's RSVP status
   */
  getCurrentUserRsvpStatus(): RsvpStatus {
    // TODO: Get actual current user ID
    const currentUserAttendee = this.event.attendees.find(a => a.id === 'current-user');
    return currentUserAttendee?.rsvpStatus || RsvpStatus.NotResponded;
  }

  /**
   * Get RSVP status configuration
   */
  getRsvpStatusConfig() {
    const status = this.getCurrentUserRsvpStatus();
    return this.rsvpStatuses.find(s => s.value === status);
  }

  /**
   * Get attending count
   */
  getAttendingCount(): number {
    return this.event.attendees.filter(a => a.rsvpStatus === RsvpStatus.Attending).length;
  }

  /**
   * Get maybe count
   */
  getMaybeCount(): number {
    return this.event.attendees.filter(a => a.rsvpStatus === RsvpStatus.Maybe).length;
  }

  /**
   * Get declined count
   */
  getDeclinedCount(): number {
    return this.event.attendees.filter(a => a.rsvpStatus === RsvpStatus.Declined).length;
  }

  /**
   * Format date range
   */
  getDateRange(): string {
    const start = this.formatDateTime(this.event.start);
    
    if (this.event.allDay) {
      if (this.isSameDay(this.event.start, this.event.end)) {
        return this.formatDate(this.event.start);
      }
      return `${this.formatDate(this.event.start)} - ${this.formatDate(this.event.end)}`;
    }
    
    if (this.isSameDay(this.event.start, this.event.end)) {
      return `${this.formatDate(this.event.start)}, ${this.formatTime(this.event.start)} - ${this.formatTime(this.event.end)}`;
    }
    
    return `${start} - ${this.formatDateTime(this.event.end)}`;
  }

  /**
   * Check if two dates are on the same day
   */
  private isSameDay(date1: Date, date2: Date): boolean {
    return date1.getFullYear() === date2.getFullYear() &&
           date1.getMonth() === date2.getMonth() &&
           date1.getDate() === date2.getDate();
  }

  /**
   * Format date and time
   */
  private formatDateTime(date: Date): string {
    return `${this.formatDate(date)}, ${this.formatTime(date)}`;
  }

  /**
   * Format date
   */
  private formatDate(date: Date): string {
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
      'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    return `${months[date.getMonth()]} ${date.getDate()}, ${date.getFullYear()}`;
  }

  /**
   * Format time
   */
  private formatTime(date: Date): string {
    const hours = date.getHours();
    const minutes = date.getMinutes();
    const ampm = hours >= 12 ? 'PM' : 'AM';
    const formattedHours = hours % 12 || 12;
    const formattedMinutes = minutes.toString().padStart(2, '0');
    return `${formattedHours}:${formattedMinutes} ${ampm}`;
  }

  /**
   * Check if event is upcoming
   */
  isUpcoming(): boolean {
    return this.event.start > new Date();
  }

  /**
   * Check if event is past
   */
  isPast(): boolean {
    return this.event.end < new Date();
  }

  /**
   * Check if event is today
   */
  isToday(): boolean {
    const today = new Date();
    return this.isSameDay(this.event.start, today);
  }

  /**
   * Handle view click
   */
  onViewClick(event: Event): void {
    event.stopPropagation();
    this.viewClick.emit(this.event);
  }

  /**
   * Handle edit click
   */
  onEditClick(event: Event): void {
    event.stopPropagation();
    this.editClick.emit(this.event);
  }

  /**
   * Handle delete click
   */
  onDeleteClick(event: Event): void {
    event.stopPropagation();
    this.deleteClick.emit(this.event);
  }

  /**
   * Handle RSVP click
   */
  onRsvpClick(event: Event): void {
    event.stopPropagation();
    this.rsvpClick.emit(this.event);
  }

  /**
   * Handle card click
   */
  onCardClick(): void {
    this.viewClick.emit(this.event);
  }

  /**
   * Get elevation CSS class
   */
  getElevationClass(): string {
    return `mat-elevation-z${this.elevation}`;
  }
}
