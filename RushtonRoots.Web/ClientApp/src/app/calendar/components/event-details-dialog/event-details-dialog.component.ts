import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { CalendarEvent, RSVP_STATUSES, EVENT_CATEGORIES } from '../../models/calendar.model';
import { EventFormDialogComponent } from '../event-form-dialog/event-form-dialog.component';
import { EventRsvpDialogComponent } from '../event-rsvp-dialog/event-rsvp-dialog.component';

/**
 * Phase 8.2: Event Details Dialog Component
 * 
 * Dialog for viewing full event details.
 * 
 * Features:
 * - Full event information display
 * - Attendee list with RSVP status
 * - Edit/delete actions
 * - Export to iCal functionality
 * - RSVP button
 */
@Component({
  selector: 'app-event-details-dialog',
  standalone: false,
  templateUrl: './event-details-dialog.component.html',
  styleUrls: ['./event-details-dialog.component.scss']
})
export class EventDetailsDialogComponent {
  rsvpStatuses = RSVP_STATUSES;
  eventCategories = EVENT_CATEGORIES;

  constructor(
    private dialog: MatDialog,
    public dialogRef: MatDialogRef<EventDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { event: CalendarEvent; canEdit: boolean }
  ) {}

  /**
   * Get category configuration
   */
  getCategoryConfig() {
    if (!this.data.event.category) return null;
    return this.eventCategories.find(c => c.value === this.data.event.category);
  }

  /**
   * Get RSVP status configuration
   */
  getRsvpStatusConfig(status: string) {
    return this.rsvpStatuses.find(s => s.value === status);
  }

  /**
   * Format event date range
   */
  getEventDateRange(): string {
    const start = this.data.event.start;
    const end = this.data.event.end;
    
    if (this.data.event.allDay) {
      if (this.isSameDay(start, end)) {
        return this.formatDate(start);
      }
      return `${this.formatDate(start)} - ${this.formatDate(end)}`;
    }
    
    return `${this.formatDate(start)}, ${this.formatTime(start)} - ${this.formatTime(end)}`;
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
   * Format date
   */
  private formatDate(date: Date): string {
    const months = ['January', 'February', 'March', 'April', 'May', 'June',
      'July', 'August', 'September', 'October', 'November', 'December'];
    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    return `${days[date.getDay()]}, ${months[date.getMonth()]} ${date.getDate()}, ${date.getFullYear()}`;
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
   * Get recurrence description
   */
  getRecurrenceDescription(): string {
    if (!this.data.event.recurrence) return '';
    
    const { frequency, interval = 1 } = this.data.event.recurrence;
    const freqText = interval === 1 ? frequency : `${interval} ${frequency}`;
    
    return `Repeats ${freqText}`;
  }

  /**
   * Get reminder descriptions
   */
  getReminderDescriptions(): string[] {
    return this.data.event.reminders
      .filter(r => r.enabled)
      .map(r => {
        const minutes = r.minutesBefore;
        let timeText = '';
        
        if (minutes === 0) timeText = 'At time of event';
        else if (minutes < 60) timeText = `${minutes} minutes before`;
        else if (minutes < 1440) timeText = `${minutes / 60} hours before`;
        else timeText = `${minutes / 1440} days before`;
        
        return `${timeText} (${r.type})`;
      });
  }

  /**
   * Get attending count
   */
  getAttendingCount(): number {
    return this.data.event.attendees.filter(a => a.rsvpStatus === 'attending').length;
  }

  /**
   * Open RSVP dialog
   */
  openRsvpDialog(): void {
    const dialogRef = this.dialog.open(EventRsvpDialogComponent, {
      width: '600px',
      data: { event: this.data.event }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Emit RSVP action
        this.dialogRef.close({ action: 'rsvp', data: result });
      }
    });
  }

  /**
   * Open edit dialog
   */
  openEditDialog(): void {
    const dialogRef = this.dialog.open(EventFormDialogComponent, {
      width: '900px',
      maxHeight: '90vh',
      data: { event: this.data.event }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.dialogRef.close({ action: 'edit', data: result });
      }
    });
  }

  /**
   * Delete event
   */
  deleteEvent(): void {
    if (confirm(`Are you sure you want to delete the event "${this.data.event.title}"?`)) {
      this.dialogRef.close({ action: 'delete', event: this.data.event });
    }
  }

  /**
   * Export to iCal
   */
  exportToICal(): void {
    const icalContent = this.generateICalContent();
    this.downloadFile(icalContent, `${this.data.event.title}.ics`, 'text/calendar');
  }

  /**
   * Generate iCal content
   */
  private generateICalContent(): string {
    const event = this.data.event;
    const now = new Date();
    
    // Format dates for iCal (YYYYMMDDTHHmmss)
    const formatICalDate = (date: Date): string => {
      const year = date.getFullYear();
      const month = (date.getMonth() + 1).toString().padStart(2, '0');
      const day = date.getDate().toString().padStart(2, '0');
      const hours = date.getHours().toString().padStart(2, '0');
      const minutes = date.getMinutes().toString().padStart(2, '0');
      const seconds = date.getSeconds().toString().padStart(2, '0');
      return `${year}${month}${day}T${hours}${minutes}${seconds}`;
    };

    const lines = [
      'BEGIN:VCALENDAR',
      'VERSION:2.0',
      'PRODID:-//RushtonRoots//Calendar//EN',
      'CALSCALE:GREGORIAN',
      'METHOD:PUBLISH',
      'BEGIN:VEVENT',
      `UID:${event.id}@rushtonroots.com`,
      `DTSTAMP:${formatICalDate(now)}`,
      `DTSTART:${formatICalDate(event.start)}`,
      `DTEND:${formatICalDate(event.end)}`,
      `SUMMARY:${event.title}`,
    ];

    if (event.description) {
      lines.push(`DESCRIPTION:${event.description.replace(/\n/g, '\\n')}`);
    }

    if (event.location) {
      lines.push(`LOCATION:${event.location}`);
    }

    if (event.organizer) {
      lines.push(`ORGANIZER;CN=${event.organizer.name}:mailto:${event.organizer.email}`);
    }

    // Add attendees
    event.attendees.forEach(attendee => {
      lines.push(`ATTENDEE;CN=${attendee.name};RSVP=TRUE:mailto:${attendee.email}`);
    });

    // Add reminders
    event.reminders.filter(r => r.enabled).forEach(reminder => {
      lines.push('BEGIN:VALARM');
      lines.push('ACTION:DISPLAY');
      lines.push(`TRIGGER:-PT${reminder.minutesBefore}M`);
      lines.push('END:VALARM');
    });

    lines.push('END:VEVENT');
    lines.push('END:VCALENDAR');

    return lines.join('\r\n');
  }

  /**
   * Download file
   */
  private downloadFile(content: string, filename: string, mimeType: string): void {
    const blob = new Blob([content], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  /**
   * Close dialog
   */
  close(): void {
    this.dialogRef.close();
  }
}
