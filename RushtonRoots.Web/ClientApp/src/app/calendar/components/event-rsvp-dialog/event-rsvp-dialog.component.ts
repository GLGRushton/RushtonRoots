import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CalendarEvent, RsvpStatus, RsvpFormData, RSVP_STATUSES } from '../../models/calendar.model';

/**
 * Phase 8.2: Event RSVP Dialog Component
 * 
 * Dialog for responding to event invitations.
 * 
 * Features:
 * - RSVP status selection (attending, maybe, declined)
 * - Optional comment field
 * - Guest count selector
 * - Event information display
 */
@Component({
  selector: 'app-event-rsvp-dialog',
  templateUrl: './event-rsvp-dialog.component.html',
  styleUrls: ['./event-rsvp-dialog.component.scss']
})
export class EventRsvpDialogComponent implements OnInit {
  rsvpForm!: FormGroup;
  rsvpStatuses = RSVP_STATUSES;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<EventRsvpDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { event: CalendarEvent }
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  /**
   * Initialize the form
   */
  private initializeForm(): void {
    // Get current user's RSVP if exists
    const currentUserAttendee = this.data.event.attendees.find(a => a.id === 'current-user');

    this.rsvpForm = this.fb.group({
      status: [currentUserAttendee?.rsvpStatus || RsvpStatus.NotResponded, Validators.required],
      comment: [currentUserAttendee?.rsvpComment || '', Validators.maxLength(500)],
      guestCount: [currentUserAttendee?.guestCount || 0, [Validators.min(0), Validators.max(20)]]
    });
  }

  /**
   * Format event date range
   */
  getEventDateRange(): string {
    const start = this.data.event.start;
    const end = this.data.event.end;
    
    if (this.data.event.allDay) {
      return this.formatDate(start);
    }
    
    return `${this.formatDate(start)}, ${this.formatTime(start)} - ${this.formatTime(end)}`;
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
   * Get selected status config
   */
  getSelectedStatusConfig() {
    const status = this.rsvpForm.get('status')?.value;
    return this.rsvpStatuses.find(s => s.value === status);
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.rsvpForm.valid) {
      const formValue = this.rsvpForm.value;
      
      const rsvpData: RsvpFormData = {
        eventId: this.data.event.id,
        status: formValue.status,
        comment: formValue.comment,
        guestCount: formValue.guestCount
      };

      this.dialogRef.close(rsvpData);
    }
  }

  /**
   * Handle cancel
   */
  onCancel(): void {
    this.dialogRef.close();
  }
}
