import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { CalendarOptions, EventClickArg, DateSelectArg, EventApi } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';
import interactionPlugin from '@fullcalendar/interaction';
import { FullCalendarComponent } from '@fullcalendar/angular';
import {
  CalendarEvent,
  CalendarView,
  CalendarFilter,
  CalendarActionEvent,
  CALENDAR_VIEWS,
  EVENT_CATEGORIES,
  RSVP_STATUSES,
  RsvpStatus,
  EventCategory,
  SAMPLE_EVENTS
} from '../../models/calendar.model';

/**
 * Phase 8.2: Calendar Component
 * 
 * Main calendar component with FullCalendar integration.
 * Features:
 * - Month, week, day, and list views
 * - Event creation, editing, and deletion
 * - Event filtering by category and RSVP status
 * - Responsive design
 * - Event click handlers
 */
@Component({
  selector: 'app-calendar',
  standalone: false,
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {
  @ViewChild('calendar') calendarComponent!: FullCalendarComponent;

  // Inputs
  @Input() events: CalendarEvent[] = [];
  @Input() initialView: CalendarView = CalendarView.Month;
  @Input() canEdit: boolean = true;

  // Outputs
  @Output() eventClick = new EventEmitter<CalendarEvent>();
  @Output() eventCreate = new EventEmitter<Date>();
  @Output() eventEdit = new EventEmitter<CalendarEvent>();
  @Output() eventDelete = new EventEmitter<CalendarEvent>();
  @Output() dateSelect = new EventEmitter<{ start: Date; end: Date }>();

  // Public properties
  calendarOptions!: CalendarOptions;
  currentView: CalendarView = CalendarView.Month;
  currentDate: Date = new Date();
  calendarViews = CALENDAR_VIEWS;
  eventCategories = EVENT_CATEGORIES;
  rsvpStatuses = RSVP_STATUSES;

  // Filter state
  filter: CalendarFilter = {
    categories: [],
    rsvpStatuses: [],
    showPrivateEvents: true,
    searchQuery: ''
  };
  showFilters: boolean = false;

  // Loading state
  isLoading: boolean = false;

  ngOnInit(): void {
    this.initializeCalendar();
    
    // Use sample events if none provided
    if (this.events.length === 0) {
      this.events = SAMPLE_EVENTS;
    }
  }

  /**
   * Initialize FullCalendar options
   */
  private initializeCalendar(): void {
    this.currentView = this.initialView;

    this.calendarOptions = {
      plugins: [dayGridPlugin, timeGridPlugin, listPlugin, interactionPlugin],
      initialView: this.currentView,
      headerToolbar: {
        left: '',
        center: '',
        right: ''
      },
      editable: this.canEdit,
      selectable: this.canEdit,
      selectMirror: true,
      dayMaxEvents: true,
      weekends: true,
      events: this.getFullCalendarEvents(),
      select: this.handleDateSelect.bind(this),
      eventClick: this.handleEventClick.bind(this),
      eventDrop: this.handleEventDrop.bind(this),
      eventResize: this.handleEventResize.bind(this),
      height: 'auto',
      contentHeight: 'auto',
      expandRows: true
    };
  }

  /**
   * Convert CalendarEvent to FullCalendar EventInput format
   */
  private getFullCalendarEvents(): any[] {
    return this.getFilteredEvents().map(event => ({
      id: event.id,
      title: event.title,
      start: event.start,
      end: event.end,
      allDay: event.allDay,
      backgroundColor: event.color,
      borderColor: event.color,
      extendedProps: {
        description: event.description,
        location: event.location,
        category: event.category,
        attendees: event.attendees,
        rsvpRequired: event.rsvpRequired
      }
    }));
  }

  /**
   * Get filtered events based on current filter settings
   */
  private getFilteredEvents(): CalendarEvent[] {
    return this.events.filter(event => {
      // Filter by category
      if (this.filter.categories.length > 0 && event.category) {
        if (!this.filter.categories.includes(event.category)) {
          return false;
        }
      }

      // Filter by RSVP status
      if (this.filter.rsvpStatuses.length > 0) {
        const currentUserRsvp = event.attendees.find(a => a.id === 'current-user'); // TODO: Get actual current user
        if (!currentUserRsvp || !this.filter.rsvpStatuses.includes(currentUserRsvp.rsvpStatus)) {
          return false;
        }
      }

      // Filter private events
      if (!this.filter.showPrivateEvents && event.isPrivate) {
        return false;
      }

      // Filter by search query
      if (this.filter.searchQuery) {
        const query = this.filter.searchQuery.toLowerCase();
        const matchesTitle = event.title.toLowerCase().includes(query);
        const matchesDescription = event.description?.toLowerCase().includes(query);
        const matchesLocation = event.location?.toLowerCase().includes(query);
        
        if (!matchesTitle && !matchesDescription && !matchesLocation) {
          return false;
        }
      }

      return true;
    });
  }

  /**
   * Handle date selection for creating new event
   */
  private handleDateSelect(selectInfo: DateSelectArg): void {
    const calendarApi = selectInfo.view.calendar;
    calendarApi.unselect(); // clear date selection

    this.dateSelect.emit({
      start: selectInfo.start,
      end: selectInfo.end
    });
  }

  /**
   * Handle event click
   */
  private handleEventClick(clickInfo: EventClickArg): void {
    const eventId = clickInfo.event.id;
    const event = this.events.find(e => e.id === eventId);
    
    if (event) {
      this.eventClick.emit(event);
    }
  }

  /**
   * Handle event drop (drag and drop)
   */
  private handleEventDrop(info: any): void {
    const eventId = info.event.id;
    const event = this.events.find(e => e.id === eventId);
    
    if (event) {
      const updatedEvent = {
        ...event,
        start: info.event.start,
        end: info.event.end || info.event.start
      };
      this.eventEdit.emit(updatedEvent);
    }
  }

  /**
   * Handle event resize
   */
  private handleEventResize(info: any): void {
    const eventId = info.event.id;
    const event = this.events.find(e => e.id === eventId);
    
    if (event) {
      const updatedEvent = {
        ...event,
        start: info.event.start,
        end: info.event.end || info.event.start
      };
      this.eventEdit.emit(updatedEvent);
    }
  }

  /**
   * Change calendar view
   */
  changeView(view: CalendarView): void {
    this.currentView = view;
    const calendarApi = this.calendarComponent.getApi();
    calendarApi.changeView(view);
  }

  /**
   * Navigate to today
   */
  goToToday(): void {
    const calendarApi = this.calendarComponent.getApi();
    calendarApi.today();
    this.currentDate = calendarApi.getDate();
  }

  /**
   * Navigate to previous period
   */
  goPrevious(): void {
    const calendarApi = this.calendarComponent.getApi();
    calendarApi.prev();
    this.currentDate = calendarApi.getDate();
  }

  /**
   * Navigate to next period
   */
  goNext(): void {
    const calendarApi = this.calendarComponent.getApi();
    calendarApi.next();
    this.currentDate = calendarApi.getDate();
  }

  /**
   * Get formatted current date range
   */
  getCurrentDateRange(): string {
    const calendarApi = this.calendarComponent?.getApi();
    if (!calendarApi) {
      return this.formatDate(this.currentDate);
    }

    const view = calendarApi.view;
    const start = view.currentStart;
    const end = view.currentEnd;

    switch (this.currentView) {
      case CalendarView.Month:
        return this.formatDate(start, 'MMMM yyyy');
      case CalendarView.Week:
        return `${this.formatDate(start, 'MMM d')} - ${this.formatDate(end, 'MMM d, yyyy')}`;
      case CalendarView.Day:
        return this.formatDate(start, 'MMMM d, yyyy');
      default:
        return this.formatDate(start);
    }
  }

  /**
   * Format date to string
   */
  private formatDate(date: Date, format: string = 'MMMM d, yyyy'): string {
    // Simple date formatting (in production, use a library like date-fns or moment)
    const months = ['January', 'February', 'March', 'April', 'May', 'June',
      'July', 'August', 'September', 'October', 'November', 'December'];
    const monthsShort = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
      'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    return format
      .replace('MMMM', months[date.getMonth()])
      .replace('MMM', monthsShort[date.getMonth()])
      .replace('yyyy', date.getFullYear().toString())
      .replace('d', date.getDate().toString());
  }

  /**
   * Toggle filter panel
   */
  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  /**
   * Apply filters
   */
  applyFilters(): void {
    this.refreshCalendar();
  }

  /**
   * Clear all filters
   */
  clearFilters(): void {
    this.filter = {
      categories: [],
      rsvpStatuses: [],
      showPrivateEvents: true,
      searchQuery: ''
    };
    this.refreshCalendar();
  }

  /**
   * Refresh calendar with updated events
   */
  refreshCalendar(): void {
    const calendarApi = this.calendarComponent.getApi();
    const events = this.getFullCalendarEvents();
    
    // Remove all events
    calendarApi.getEvents().forEach(event => event.remove());
    
    // Add filtered events
    events.forEach(event => calendarApi.addEvent(event));
  }

  /**
   * Create new event
   */
  createEvent(): void {
    this.eventCreate.emit(new Date());
  }

  /**
   * Toggle category filter
   */
  toggleCategoryFilter(category: EventCategory): void {
    const index = this.filter.categories.indexOf(category);
    if (index === -1) {
      this.filter.categories.push(category);
    } else {
      this.filter.categories.splice(index, 1);
    }
  }

  /**
   * Toggle RSVP status filter
   */
  toggleRsvpStatusFilter(status: RsvpStatus): void {
    const index = this.filter.rsvpStatuses.indexOf(status);
    if (index === -1) {
      this.filter.rsvpStatuses.push(status);
    } else {
      this.filter.rsvpStatuses.splice(index, 1);
    }
  }

  /**
   * Check if category is selected in filter
   */
  isCategorySelected(category: EventCategory): boolean {
    return this.filter.categories.includes(category);
  }

  /**
   * Check if RSVP status is selected in filter
   */
  isRsvpStatusSelected(status: RsvpStatus): boolean {
    return this.filter.rsvpStatuses.includes(status);
  }
}
