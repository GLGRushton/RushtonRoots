import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CalendarEvent,
  EventFormData,
  EVENT_CATEGORIES,
  RECURRENCE_FREQUENCIES,
  REMINDER_TIME_OPTIONS,
  RecurrenceFrequency,
  ReminderType,
  EventReminder,
  PersonOption
} from '../../models/calendar.model';

/**
 * Phase 8.2: Event Form Dialog Component
 * 
 * Dialog for creating and editing calendar events.
 * 
 * Features:
 * - Title, description, date/time fields
 * - Location input
 * - Attendee selection with autocomplete
 * - Recurring event options
 * - Reminder settings
 * - Form validation
 * - Material Design styling
 */
@Component({
  selector: 'app-event-form-dialog',
  standalone: false,
  templateUrl: './event-form-dialog.component.html',
  styleUrls: ['./event-form-dialog.component.scss']
})
export class EventFormDialogComponent implements OnInit {
  eventForm!: FormGroup;
  isEditMode: boolean = false;
  eventCategories = EVENT_CATEGORIES;
  recurrenceFrequencies = RECURRENCE_FREQUENCIES;
  reminderTimeOptions = REMINDER_TIME_OPTIONS;
  reminderTypes = [
    { value: ReminderType.Email, label: 'Email' },
    { value: ReminderType.Push, label: 'Push Notification' },
    { value: ReminderType.Both, label: 'Both' }
  ];

  // Sample person options for attendee selection
  personOptions: PersonOption[] = [
    { id: '1', name: 'John Rushton', email: 'john@rushtonroots.com' },
    { id: '2', name: 'Mary Rushton', email: 'mary@rushtonroots.com' },
    { id: '3', name: 'David Rushton', email: 'david@rushtonroots.com' },
    { id: '4', name: 'Sarah Johnson', email: 'sarah@rushtonroots.com' },
    { id: '5', name: 'Michael Smith', email: 'michael@rushtonroots.com' }
  ];

  filteredPersonOptions: PersonOption[] = [];

  // Days of week for weekly recurrence
  daysOfWeek = [
    { value: 0, label: 'Sun', name: 'Sunday' },
    { value: 1, label: 'Mon', name: 'Monday' },
    { value: 2, label: 'Tue', name: 'Tuesday' },
    { value: 3, label: 'Wed', name: 'Wednesday' },
    { value: 4, label: 'Thu', name: 'Thursday' },
    { value: 5, label: 'Fri', name: 'Friday' },
    { value: 6, label: 'Sat', name: 'Saturday' }
  ];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<EventFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { event?: CalendarEvent; startDate?: Date }
  ) {
    this.isEditMode = !!data.event;
    this.filteredPersonOptions = this.personOptions;
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  /**
   * Initialize the form
   */
  private initializeForm(): void {
    const event = this.data.event;
    const startDate = event?.start || this.data.startDate || new Date();
    const endDate = event?.end || new Date(startDate.getTime() + 3600000); // 1 hour later

    this.eventForm = this.fb.group({
      title: [event?.title || '', [Validators.required, Validators.maxLength(200)]],
      description: [event?.description || '', Validators.maxLength(2000)],
      startDate: [startDate, Validators.required],
      startTime: [this.formatTimeForInput(startDate), Validators.required],
      endDate: [endDate, Validators.required],
      endTime: [this.formatTimeForInput(endDate), Validators.required],
      allDay: [event?.allDay || false],
      location: [event?.location || '', Validators.maxLength(200)],
      attendees: [event?.attendees.map(a => a.id) || []],
      category: [event?.category || null],
      isPrivate: [event?.isPrivate || false],
      rsvpRequired: [event?.rsvpRequired || false],
      
      // Recurrence fields
      hasRecurrence: [!!event?.recurrence],
      recurrenceFrequency: [event?.recurrence?.frequency || RecurrenceFrequency.Weekly],
      recurrenceInterval: [event?.recurrence?.interval || 1, [Validators.min(1), Validators.max(99)]],
      recurrenceEndDate: [event?.recurrence?.endDate || null],
      recurrenceOccurrences: [event?.recurrence?.occurrences || null, [Validators.min(1), Validators.max(999)]],
      recurrenceEndType: [event?.recurrence?.endDate ? 'date' : 'occurrences'],
      recurrenceDaysOfWeek: [event?.recurrence?.daysOfWeek || []],

      // Reminders
      reminders: this.fb.array(this.createReminderControls(event?.reminders || []))
    });

    // Subscribe to allDay changes
    this.eventForm.get('allDay')?.valueChanges.subscribe(allDay => {
      if (allDay) {
        this.eventForm.get('startTime')?.disable();
        this.eventForm.get('endTime')?.disable();
      } else {
        this.eventForm.get('startTime')?.enable();
        this.eventForm.get('endTime')?.enable();
      }
    });

    // Subscribe to recurrence changes
    this.eventForm.get('hasRecurrence')?.valueChanges.subscribe(hasRecurrence => {
      if (hasRecurrence) {
        this.eventForm.get('recurrenceFrequency')?.enable();
        this.eventForm.get('recurrenceInterval')?.enable();
      } else {
        this.eventForm.get('recurrenceFrequency')?.disable();
        this.eventForm.get('recurrenceInterval')?.disable();
        this.eventForm.get('recurrenceEndDate')?.disable();
        this.eventForm.get('recurrenceOccurrences')?.disable();
        this.eventForm.get('recurrenceDaysOfWeek')?.disable();
      }
    });
  }

  /**
   * Create reminder form controls
   */
  private createReminderControls(reminders: EventReminder[]): FormGroup[] {
    if (reminders.length === 0) {
      // Add one default reminder
      return [this.createReminderGroup()];
    }
    return reminders.map(r => this.createReminderGroup(r));
  }

  /**
   * Create a single reminder form group
   */
  private createReminderGroup(reminder?: EventReminder): FormGroup {
    return this.fb.group({
      id: [reminder?.id || this.generateId()],
      type: [reminder?.type || ReminderType.Email],
      minutesBefore: [reminder?.minutesBefore || 60],
      enabled: [reminder?.enabled ?? true]
    });
  }

  /**
   * Get reminders form array
   */
  get reminders(): FormArray {
    return this.eventForm.get('reminders') as FormArray;
  }

  /**
   * Add a new reminder
   */
  addReminder(): void {
    this.reminders.push(this.createReminderGroup());
  }

  /**
   * Remove a reminder
   */
  removeReminder(index: number): void {
    this.reminders.removeAt(index);
  }

  /**
   * Format time for input[type="time"]
   */
  private formatTimeForInput(date: Date): string {
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }

  /**
   * Filter person options based on search
   */
  filterPersonOptions(searchText: string): void {
    if (!searchText) {
      this.filteredPersonOptions = this.personOptions;
      return;
    }

    const search = searchText.toLowerCase();
    this.filteredPersonOptions = this.personOptions.filter(p =>
      p.name.toLowerCase().includes(search) ||
      p.email.toLowerCase().includes(search)
    );
  }

  /**
   * Toggle day of week selection
   */
  toggleDayOfWeek(day: number): void {
    const daysOfWeek: number[] = this.eventForm.get('recurrenceDaysOfWeek')?.value || [];
    const index = daysOfWeek.indexOf(day);
    
    if (index === -1) {
      daysOfWeek.push(day);
    } else {
      daysOfWeek.splice(index, 1);
    }
    
    this.eventForm.get('recurrenceDaysOfWeek')?.setValue(daysOfWeek);
  }

  /**
   * Check if day of week is selected
   */
  isDayOfWeekSelected(day: number): boolean {
    const daysOfWeek: number[] = this.eventForm.get('recurrenceDaysOfWeek')?.value || [];
    return daysOfWeek.includes(day);
  }

  /**
   * Generate a unique ID
   */
  private generateId(): string {
    return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.eventForm.valid) {
      const formValue = this.eventForm.getRawValue();
      
      const eventData: EventFormData = {
        id: this.data.event?.id,
        title: formValue.title,
        description: formValue.description,
        startDate: this.combineDateTime(formValue.startDate, formValue.allDay ? '00:00' : formValue.startTime),
        endDate: this.combineDateTime(formValue.endDate, formValue.allDay ? '23:59' : formValue.endTime),
        allDay: formValue.allDay,
        location: formValue.location,
        attendees: formValue.attendees,
        category: formValue.category,
        isPrivate: formValue.isPrivate,
        rsvpRequired: formValue.rsvpRequired,
        recurrence: formValue.hasRecurrence ? {
          frequency: formValue.recurrenceFrequency,
          interval: formValue.recurrenceInterval,
          endDate: formValue.recurrenceEndType === 'date' ? formValue.recurrenceEndDate : undefined,
          occurrences: formValue.recurrenceEndType === 'occurrences' ? formValue.recurrenceOccurrences : undefined,
          daysOfWeek: formValue.recurrenceFrequency === RecurrenceFrequency.Weekly ? formValue.recurrenceDaysOfWeek : undefined
        } : undefined,
        reminders: formValue.reminders
      };

      this.dialogRef.close(eventData);
    }
  }

  /**
   * Combine date and time
   */
  private combineDateTime(date: Date, time: string): Date {
    const [hours, minutes] = time.split(':').map(Number);
    const combined = new Date(date);
    combined.setHours(hours, minutes, 0, 0);
    return combined;
  }

  /**
   * Handle cancel
   */
  onCancel(): void {
    this.dialogRef.close();
  }

  /**
   * Get form title
   */
  getDialogTitle(): string {
    return this.isEditMode ? 'Edit Event' : 'Create New Event';
  }
}
