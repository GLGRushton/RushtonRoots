import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

// Angular Material Modules
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonToggleModule } from '@angular/material/button-toggle';

// FullCalendar Modules
import { FullCalendarModule } from '@fullcalendar/angular';

// Calendar Components
import { CalendarComponent } from './components/calendar/calendar.component';
import { EventCardComponent } from './components/event-card/event-card.component';
import { EventFormDialogComponent } from './components/event-form-dialog/event-form-dialog.component';
import { EventRsvpDialogComponent } from './components/event-rsvp-dialog/event-rsvp-dialog.component';
import { EventDetailsDialogComponent } from './components/event-details-dialog/event-details-dialog.component';

/**
 * Phase 8.2: Calendar Module
 * 
 * This module contains all calendar and event-related components.
 * 
 * Components:
 * - CalendarComponent: Main calendar view with FullCalendar integration
 * - EventCardComponent: Event card display
 * - EventFormDialogComponent: Create/edit event dialog
 * - EventRsvpDialogComponent: RSVP to event dialog
 * - EventDetailsDialogComponent: View event details dialog
 * 
 * Features:
 * - Interactive calendar with multiple views
 * - Event creation, editing, and deletion
 * - RSVP management
 * - Recurring events
 * - Event reminders
 * - iCal export
 */
@NgModule({
  declarations: [
    CalendarComponent,
    EventCardComponent,
    EventFormDialogComponent,
    EventRsvpDialogComponent,
    EventDetailsDialogComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    
    // FullCalendar
    FullCalendarModule,
    
    // Material Modules
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    MatRadioModule,
    MatChipsModule,
    MatMenuModule,
    MatTooltipModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatButtonToggleModule
  ],
  exports: [
    CalendarComponent,
    EventCardComponent,
    EventFormDialogComponent,
    EventRsvpDialogComponent,
    EventDetailsDialogComponent
  ]
})
export class CalendarModule { }
