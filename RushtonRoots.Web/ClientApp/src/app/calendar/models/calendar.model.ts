/**
 * Phase 8.2: Calendar & Events - TypeScript Models
 * 
 * This file defines all TypeScript interfaces and types for the calendar and events feature.
 * These models support event management, RSVP tracking, recurring events, and calendar views.
 */

// ===========================
// Event Models
// ===========================

/**
 * Main event interface representing a calendar event
 */
export interface CalendarEvent {
  id: string;
  title: string;
  description?: string;
  start: Date;
  end: Date;
  allDay?: boolean;
  location?: string;
  organizer: EventAttendee;
  attendees: EventAttendee[];
  recurrence?: RecurrenceRule;
  reminders: EventReminder[];
  color?: string;
  category?: EventCategory;
  isPrivate?: boolean;
  rsvpRequired?: boolean;
  createdAt: Date;
  updatedAt: Date;
}

/**
 * Event attendee with RSVP status
 */
export interface EventAttendee {
  id: string;
  name: string;
  email: string;
  avatarUrl?: string;
  rsvpStatus: RsvpStatus;
  rsvpComment?: string;
  guestCount?: number;
  respondedAt?: Date;
}

/**
 * RSVP status enumeration
 */
export enum RsvpStatus {
  Pending = 'pending',
  Attending = 'attending',
  Maybe = 'maybe',
  Declined = 'declined',
  NotResponded = 'not_responded'
}

/**
 * RSVP status configuration with display properties
 */
export interface RsvpStatusConfig {
  value: RsvpStatus;
  label: string;
  icon: string;
  color: string;
}

/**
 * RSVP status configurations
 */
export const RSVP_STATUSES: RsvpStatusConfig[] = [
  { value: RsvpStatus.Attending, label: 'Attending', icon: 'check_circle', color: '#4caf50' },
  { value: RsvpStatus.Maybe, label: 'Maybe', icon: 'help', color: '#ff9800' },
  { value: RsvpStatus.Declined, label: 'Declined', icon: 'cancel', color: '#f44336' },
  { value: RsvpStatus.NotResponded, label: 'Not Responded', icon: 'schedule', color: '#9e9e9e' }
];

/**
 * Event category enumeration
 */
export enum EventCategory {
  Birthday = 'birthday',
  Anniversary = 'anniversary',
  Reunion = 'reunion',
  Holiday = 'holiday',
  Memorial = 'memorial',
  Wedding = 'wedding',
  Baptism = 'baptism',
  Graduation = 'graduation',
  Meeting = 'meeting',
  Other = 'other'
}

/**
 * Event category configuration with display properties
 */
export interface EventCategoryConfig {
  value: EventCategory;
  label: string;
  icon: string;
  color: string;
}

/**
 * Event category configurations
 */
export const EVENT_CATEGORIES: EventCategoryConfig[] = [
  { value: EventCategory.Birthday, label: 'Birthday', icon: 'cake', color: '#e91e63' },
  { value: EventCategory.Anniversary, label: 'Anniversary', icon: 'favorite', color: '#9c27b0' },
  { value: EventCategory.Reunion, label: 'Family Reunion', icon: 'groups', color: '#3f51b5' },
  { value: EventCategory.Holiday, label: 'Holiday', icon: 'celebration', color: '#2196f3' },
  { value: EventCategory.Memorial, label: 'Memorial', icon: 'local_florist', color: '#607d8b' },
  { value: EventCategory.Wedding, label: 'Wedding', icon: 'favorite_border', color: '#ff5722' },
  { value: EventCategory.Baptism, label: 'Baptism', icon: 'water_drop', color: '#00bcd4' },
  { value: EventCategory.Graduation, label: 'Graduation', icon: 'school', color: '#009688' },
  { value: EventCategory.Meeting, label: 'Meeting', icon: 'business_center', color: '#795548' },
  { value: EventCategory.Other, label: 'Other', icon: 'event', color: '#9e9e9e' }
];

// ===========================
// Recurrence Models
// ===========================

/**
 * Recurrence frequency enumeration
 */
export enum RecurrenceFrequency {
  Daily = 'daily',
  Weekly = 'weekly',
  Monthly = 'monthly',
  Yearly = 'yearly'
}

/**
 * Recurrence rule for recurring events
 */
export interface RecurrenceRule {
  frequency: RecurrenceFrequency;
  interval?: number; // e.g., every 2 weeks
  endDate?: Date;
  occurrences?: number; // alternatively, stop after N occurrences
  daysOfWeek?: number[]; // 0 = Sunday, 1 = Monday, etc.
  dayOfMonth?: number; // for monthly recurrence
  monthOfYear?: number; // for yearly recurrence
}

/**
 * Recurrence frequency configuration
 */
export interface RecurrenceFrequencyConfig {
  value: RecurrenceFrequency;
  label: string;
  description: string;
}

/**
 * Recurrence frequency configurations
 */
export const RECURRENCE_FREQUENCIES: RecurrenceFrequencyConfig[] = [
  { value: RecurrenceFrequency.Daily, label: 'Daily', description: 'Repeats every day' },
  { value: RecurrenceFrequency.Weekly, label: 'Weekly', description: 'Repeats every week' },
  { value: RecurrenceFrequency.Monthly, label: 'Monthly', description: 'Repeats every month' },
  { value: RecurrenceFrequency.Yearly, label: 'Yearly', description: 'Repeats every year' }
];

// ===========================
// Reminder Models
// ===========================

/**
 * Event reminder configuration
 */
export interface EventReminder {
  id: string;
  type: ReminderType;
  minutesBefore: number;
  enabled: boolean;
}

/**
 * Reminder type enumeration
 */
export enum ReminderType {
  Email = 'email',
  Push = 'push',
  Both = 'both'
}

/**
 * Reminder time options
 */
export interface ReminderTimeOption {
  label: string;
  minutes: number;
}

/**
 * Predefined reminder time options
 */
export const REMINDER_TIME_OPTIONS: ReminderTimeOption[] = [
  { label: 'At time of event', minutes: 0 },
  { label: '5 minutes before', minutes: 5 },
  { label: '15 minutes before', minutes: 15 },
  { label: '30 minutes before', minutes: 30 },
  { label: '1 hour before', minutes: 60 },
  { label: '2 hours before', minutes: 120 },
  { label: '1 day before', minutes: 1440 },
  { label: '2 days before', minutes: 2880 },
  { label: '1 week before', minutes: 10080 }
];

// ===========================
// Calendar View Models
// ===========================

/**
 * Calendar view types
 */
export enum CalendarView {
  Month = 'dayGridMonth',
  Week = 'timeGridWeek',
  Day = 'timeGridDay',
  List = 'listWeek'
}

/**
 * Calendar view configuration
 */
export interface CalendarViewConfig {
  value: CalendarView;
  label: string;
  icon: string;
}

/**
 * Calendar view configurations
 */
export const CALENDAR_VIEWS: CalendarViewConfig[] = [
  { value: CalendarView.Month, label: 'Month', icon: 'calendar_view_month' },
  { value: CalendarView.Week, label: 'Week', icon: 'calendar_view_week' },
  { value: CalendarView.Day, label: 'Day', icon: 'calendar_view_day' },
  { value: CalendarView.List, label: 'List', icon: 'list' }
];

/**
 * Calendar filter options
 */
export interface CalendarFilter {
  categories: EventCategory[];
  rsvpStatuses: RsvpStatus[];
  showPrivateEvents: boolean;
  searchQuery?: string;
}

// ===========================
// Form Models
// ===========================

/**
 * Event form data for creating/editing events
 */
export interface EventFormData {
  id?: string;
  title: string;
  description?: string;
  startDate: Date;
  startTime?: string;
  endDate: Date;
  endTime?: string;
  allDay: boolean;
  location?: string;
  attendees: string[]; // Array of person IDs
  category?: EventCategory;
  isPrivate: boolean;
  rsvpRequired: boolean;
  recurrence?: RecurrenceRule;
  reminders: EventReminder[];
}

/**
 * RSVP form data
 */
export interface RsvpFormData {
  eventId: string;
  status: RsvpStatus;
  comment?: string;
  guestCount?: number;
}

/**
 * Person option for attendee selection
 */
export interface PersonOption {
  id: string;
  name: string;
  email: string;
  avatarUrl?: string;
}

// ===========================
// Action Events
// ===========================

/**
 * Calendar action event types
 */
export interface CalendarActionEvent {
  action: 'create' | 'edit' | 'delete' | 'view' | 'rsvp';
  event?: CalendarEvent;
}

/**
 * Event details action event
 */
export interface EventDetailsAction {
  action: 'edit' | 'delete' | 'export' | 'rsvp';
  event: CalendarEvent;
}

// ===========================
// Export Models
// ===========================

/**
 * iCal export options
 */
export interface ICalExportOptions {
  eventId: string;
  includeReminders: boolean;
  includeAttendees: boolean;
}

// ===========================
// Sample Data
// ===========================

/**
 * Sample events for demonstration
 */
export const SAMPLE_EVENTS: CalendarEvent[] = [
  {
    id: '1',
    title: 'Family Reunion 2025',
    description: 'Annual family reunion at the park. Bring food to share!',
    start: new Date(2025, 6, 15, 12, 0), // July 15, 2025, 12:00 PM
    end: new Date(2025, 6, 15, 18, 0), // July 15, 2025, 6:00 PM
    allDay: false,
    location: 'Central Park, Pavilion 3',
    organizer: {
      id: '1',
      name: 'John Rushton',
      email: 'john@rushtonroots.com',
      avatarUrl: undefined,
      rsvpStatus: RsvpStatus.Attending
    },
    attendees: [
      {
        id: '2',
        name: 'Mary Rushton',
        email: 'mary@rushtonroots.com',
        rsvpStatus: RsvpStatus.Attending,
        guestCount: 2,
        respondedAt: new Date(2025, 5, 1)
      },
      {
        id: '3',
        name: 'David Rushton',
        email: 'david@rushtonroots.com',
        rsvpStatus: RsvpStatus.Maybe,
        respondedAt: new Date(2025, 5, 5)
      }
    ],
    reminders: [
      { id: 'r1', type: ReminderType.Both, minutesBefore: 1440, enabled: true }
    ],
    color: '#3f51b5',
    category: EventCategory.Reunion,
    isPrivate: false,
    rsvpRequired: true,
    createdAt: new Date(2025, 4, 1),
    updatedAt: new Date(2025, 4, 1)
  },
  {
    id: '2',
    title: 'Grandma\'s 90th Birthday',
    description: 'Surprise party for Grandma\'s 90th birthday!',
    start: new Date(2025, 7, 20, 14, 0), // August 20, 2025, 2:00 PM
    end: new Date(2025, 7, 20, 17, 0),
    allDay: false,
    location: 'Johnson Family Home',
    organizer: {
      id: '4',
      name: 'Sarah Johnson',
      email: 'sarah@rushtonroots.com',
      rsvpStatus: RsvpStatus.Attending
    },
    attendees: [],
    reminders: [
      { id: 'r2', type: ReminderType.Email, minutesBefore: 2880, enabled: true }
    ],
    color: '#e91e63',
    category: EventCategory.Birthday,
    isPrivate: false,
    rsvpRequired: true,
    createdAt: new Date(2025, 4, 10),
    updatedAt: new Date(2025, 4, 10)
  },
  {
    id: '3',
    title: 'Christmas Dinner',
    description: 'Annual Christmas dinner at the main house.',
    start: new Date(2025, 11, 25, 17, 0), // December 25, 2025, 5:00 PM
    end: new Date(2025, 11, 25, 21, 0),
    allDay: false,
    location: 'Main House',
    organizer: {
      id: '1',
      name: 'John Rushton',
      email: 'john@rushtonroots.com',
      rsvpStatus: RsvpStatus.Attending
    },
    attendees: [],
    recurrence: {
      frequency: RecurrenceFrequency.Yearly,
      interval: 1
    },
    reminders: [
      { id: 'r3', type: ReminderType.Both, minutesBefore: 10080, enabled: true }
    ],
    color: '#2196f3',
    category: EventCategory.Holiday,
    isPrivate: false,
    rsvpRequired: false,
    createdAt: new Date(2024, 0, 1),
    updatedAt: new Date(2024, 0, 1)
  }
];
