import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { MessageThread, Message, Participant } from '../../models/messaging.model';

/**
 * Phase 8.3: Message Thread Component
 * 
 * Displays a list of message threads with unread indicators and last message preview.
 * Shows participant avatars, online status, and timestamp.
 * Supports thread selection, archiving, muting, and deletion.
 * 
 * Features:
 * - Thread list with avatars and online status
 * - Unread message count badges
 * - Last message preview
 * - Thread actions (archive, mute, delete)
 * - Search and filter threads
 * - Responsive design for mobile
 */
@Component({
  selector: 'app-message-thread',
  standalone: false,
  templateUrl: './message-thread.component.html',
  styleUrls: ['./message-thread.component.scss']
})
export class MessageThreadComponent implements OnInit, OnDestroy {
  @Input() threads: MessageThread[] = [];
  @Input() currentUserId: number = 1; // Current user ID for filtering
  @Input() selectedThreadId?: number;
  @Input() showArchived: boolean = false;
  
  @Output() threadSelected = new EventEmitter<MessageThread>();
  @Output() threadArchived = new EventEmitter<number>();
  @Output() threadDeleted = new EventEmitter<number>();
  @Output() threadMuted = new EventEmitter<number>();
  @Output() newMessageClicked = new EventEmitter<void>();

  searchQuery: string = '';
  filteredThreads: MessageThread[] = [];
  isLoading: boolean = false;

  ngOnInit(): void {
    this.filterThreads();
  }

  ngOnDestroy(): void {
    // Cleanup if needed
  }

  /**
   * Filter threads based on search query and archived status
   */
  filterThreads(): void {
    this.filteredThreads = this.threads.filter(thread => {
      // Filter by archived status
      if (thread.isArchived !== this.showArchived) {
        return false;
      }

      // Filter by search query
      if (this.searchQuery) {
        const query = this.searchQuery.toLowerCase();
        const participantNames = this.getParticipantNames(thread).toLowerCase();
        const lastMessage = thread.lastMessage?.content.toLowerCase() || '';
        const subject = thread.subject?.toLowerCase() || '';
        
        return participantNames.includes(query) || 
               lastMessage.includes(query) || 
               subject.includes(query);
      }

      return true;
    });

    // Sort by last updated (most recent first)
    this.filteredThreads.sort((a, b) => {
      return new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime();
    });
  }

  /**
   * Handle search input changes
   */
  onSearchChange(query: string): void {
    this.searchQuery = query;
    this.filterThreads();
  }

  /**
   * Select a thread
   */
  selectThread(thread: MessageThread): void {
    this.selectedThreadId = thread.id;
    this.threadSelected.emit(thread);
  }

  /**
   * Archive a thread
   */
  archiveThread(threadId: number, event: Event): void {
    event.stopPropagation();
    this.threadArchived.emit(threadId);
  }

  /**
   * Delete a thread
   */
  deleteThread(threadId: number, event: Event): void {
    event.stopPropagation();
    if (confirm('Are you sure you want to delete this conversation?')) {
      this.threadDeleted.emit(threadId);
    }
  }

  /**
   * Mute/unmute a thread
   */
  toggleMuteThread(threadId: number, event: Event): void {
    event.stopPropagation();
    this.threadMuted.emit(threadId);
  }

  /**
   * Start a new message
   */
  startNewMessage(): void {
    this.newMessageClicked.emit();
  }

  /**
   * Get participant names for display
   */
  getParticipantNames(thread: MessageThread): string {
    const otherParticipants = thread.participants.filter(p => p.id !== this.currentUserId);
    if (otherParticipants.length === 0) {
      return 'You';
    }
    if (otherParticipants.length === 1) {
      return `${otherParticipants[0].firstName} ${otherParticipants[0].lastName}`;
    }
    if (otherParticipants.length === 2) {
      return `${otherParticipants[0].firstName} ${otherParticipants[0].lastName} and ${otherParticipants[1].firstName} ${otherParticipants[1].lastName}`;
    }
    return `${otherParticipants[0].firstName} ${otherParticipants[0].lastName} and ${otherParticipants.length - 1} others`;
  }

  /**
   * Get participant avatar (first other participant)
   */
  getParticipantAvatar(thread: MessageThread): string | undefined {
    const otherParticipants = thread.participants.filter(p => p.id !== this.currentUserId);
    return otherParticipants.length > 0 ? otherParticipants[0].avatarUrl : undefined;
  }

  /**
   * Get participant initials
   */
  getParticipantInitials(thread: MessageThread): string {
    const otherParticipants = thread.participants.filter(p => p.id !== this.currentUserId);
    if (otherParticipants.length === 0) {
      return 'Y';
    }
    const participant = otherParticipants[0];
    return `${participant.firstName.charAt(0)}${participant.lastName.charAt(0)}`;
  }

  /**
   * Check if any participant is online
   */
  isParticipantOnline(thread: MessageThread): boolean {
    return thread.participants.some(p => p.id !== this.currentUserId && p.isOnline);
  }

  /**
   * Get last message preview text
   */
  getLastMessagePreview(thread: MessageThread): string {
    if (!thread.lastMessage) {
      return 'No messages yet';
    }
    const maxLength = 60;
    const content = thread.lastMessage.content;
    return content.length > maxLength ? content.substring(0, maxLength) + '...' : content;
  }

  /**
   * Format timestamp for display
   */
  formatTimestamp(date: Date): string {
    const now = new Date();
    const messageDate = new Date(date);
    const diffMs = now.getTime() - messageDate.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) {
      return 'Just now';
    } else if (diffMins < 60) {
      return `${diffMins}m ago`;
    } else if (diffHours < 24) {
      return `${diffHours}h ago`;
    } else if (diffDays < 7) {
      return `${diffDays}d ago`;
    } else {
      return messageDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
    }
  }

  /**
   * Track by function for ngFor performance
   */
  trackByThreadId(index: number, thread: MessageThread): number {
    return thread.id;
  }
}
