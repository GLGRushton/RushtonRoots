import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MessageThread, Message, MessageCompositionData, MessageStatus, MessageWithStatus, Participant } from '../../models/messaging.model';

/**
 * Phase 8.3: Chat Interface Component
 * 
 * Displays a conversation thread with real-time messaging capabilities.
 * Includes message composition, typing indicators, file attachments,
 * message status indicators, and real-time updates.
 * 
 * Features:
 * - Message list with sender avatars and timestamps
 * - Message composition input with file attachments
 * - Send button with keyboard shortcuts (Enter to send)
 * - Real-time message delivery indicators
 * - Typing indicators when other users are typing
 * - Message grouping by date
 * - Auto-scroll to latest message
 * - Edit and delete messages
 * - Message reactions (future)
 */
@Component({
  selector: 'app-chat-interface',
  standalone: false,
  templateUrl: './chat-interface.component.html',
  styleUrls: ['./chat-interface.component.scss']
})
export class ChatInterfaceComponent implements OnInit, OnDestroy, AfterViewChecked {
  @Input() thread?: MessageThread;
  @Input() currentUserId: number = 1;
  @Input() canSendMessages: boolean = true;
  
  @Output() messageSent = new EventEmitter<MessageCompositionData>();
  @Output() messageDeleted = new EventEmitter<number>();
  @Output() messageEdited = new EventEmitter<{ messageId: number; content: string }>();
  @Output() typingStarted = new EventEmitter<void>();
  @Output() typingStopped = new EventEmitter<void>();
  @Output() attachmentSelected = new EventEmitter<File[]>();

  @ViewChild('messageList') messageList?: ElementRef;
  @ViewChild('fileInput') fileInput?: ElementRef;

  messageControl = new FormControl('', [Validators.maxLength(2000)]);
  attachedFiles: File[] = [];
  isTyping: boolean = false;
  typingTimeout: any;
  editingMessageId?: number;
  editingContent: string = '';
  shouldScrollToBottom: boolean = true;
  MessageStatus = MessageStatus; // Expose enum to template

  ngOnInit(): void {
    // Listen for typing changes
    this.messageControl.valueChanges.subscribe(() => {
      this.handleTyping();
    });
  }

  ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  ngOnDestroy(): void {
    if (this.typingTimeout) {
      clearTimeout(this.typingTimeout);
    }
    this.stopTyping();
  }

  /**
   * Handle typing indicator
   */
  handleTyping(): void {
    if (!this.isTyping && this.messageControl.value) {
      this.isTyping = true;
      this.typingStarted.emit();
    }

    // Clear existing timeout
    if (this.typingTimeout) {
      clearTimeout(this.typingTimeout);
    }

    // Set new timeout to stop typing after 3 seconds
    this.typingTimeout = setTimeout(() => {
      this.stopTyping();
    }, 3000);
  }

  /**
   * Stop typing indicator
   */
  stopTyping(): void {
    if (this.isTyping) {
      this.isTyping = false;
      this.typingStopped.emit();
    }
  }

  /**
   * Send message
   */
  sendMessage(): void {
    const content = this.messageControl.value?.trim();
    
    if (!content && this.attachedFiles.length === 0) {
      return;
    }

    if (!this.thread) {
      return;
    }

    const messageData: MessageCompositionData = {
      recipientIds: this.thread.participants
        .filter(p => p.id !== this.currentUserId)
        .map(p => p.id),
      content: content || '',
      attachments: this.attachedFiles.length > 0 ? this.attachedFiles : undefined
    };

    this.messageSent.emit(messageData);
    this.messageControl.reset();
    this.attachedFiles = [];
    this.stopTyping();
    this.shouldScrollToBottom = true;
  }

  /**
   * Get online status text for header
   */
  getOnlineStatus(): string {
    if (!this.thread) {
      return 'Offline';
    }
    const otherParticipant = this.thread.participants.find(p => p.id !== this.currentUserId);
    return otherParticipant?.isOnline ? 'Online' : 'Offline';
  }

  /**
   * Handle Enter key in edit mode
   */
  onEditEnter(event: Event): void {
    const keyboardEvent = event as KeyboardEvent;
    keyboardEvent.preventDefault();
    this.saveEditedMessage();
  }

  /**
   * Handle Enter key press
   */
  onKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  /**
   * Handle file selection
   */
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const newFiles = Array.from(input.files);
      this.attachedFiles.push(...newFiles);
      this.attachmentSelected.emit(newFiles);
      
      // Reset input to allow selecting the same file again
      input.value = '';
    }
  }

  /**
   * Trigger file input click
   */
  selectFiles(): void {
    this.fileInput?.nativeElement.click();
  }

  /**
   * Remove attached file
   */
  removeFile(index: number): void {
    this.attachedFiles.splice(index, 1);
  }

  /**
   * Start editing message
   */
  startEditMessage(message: Message): void {
    this.editingMessageId = message.id;
    this.editingContent = message.content;
  }

  /**
   * Save edited message
   */
  saveEditedMessage(): void {
    if (this.editingMessageId && this.editingContent.trim()) {
      this.messageEdited.emit({
        messageId: this.editingMessageId,
        content: this.editingContent.trim()
      });
      this.cancelEdit();
    }
  }

  /**
   * Cancel editing
   */
  cancelEdit(): void {
    this.editingMessageId = undefined;
    this.editingContent = '';
  }

  /**
   * Delete message
   */
  deleteMessage(messageId: number): void {
    if (confirm('Are you sure you want to delete this message?')) {
      this.messageDeleted.emit(messageId);
    }
  }

  /**
   * Check if message is from current user
   */
  isOwnMessage(message: Message): boolean {
    return message.senderId === this.currentUserId;
  }

  /**
   * Get message status
   */
  getMessageStatus(message: Message): MessageStatus {
    const messageWithStatus = message as MessageWithStatus;
    return messageWithStatus.status || MessageStatus.SENT;
  }

  /**
   * Group messages by date
   */
  getMessagesGroupedByDate(): { date: Date; messages: Message[] }[] {
    if (!this.thread?.messages) {
      return [];
    }

    const groups = new Map<string, Message[]>();
    
    this.thread.messages.forEach(message => {
      const dateKey = new Date(message.timestamp).toDateString();
      if (!groups.has(dateKey)) {
        groups.set(dateKey, []);
      }
      groups.get(dateKey)!.push(message);
    });

    return Array.from(groups.entries()).map(([dateStr, messages]) => ({
      date: new Date(dateStr),
      messages: messages.sort((a, b) => 
        new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
      )
    }));
  }

  /**
   * Format date for display
   */
  formatDate(date: Date): string {
    const today = new Date();
    const messageDate = new Date(date);
    
    if (messageDate.toDateString() === today.toDateString()) {
      return 'Today';
    }
    
    const yesterday = new Date(today);
    yesterday.setDate(yesterday.getDate() - 1);
    if (messageDate.toDateString() === yesterday.toDateString()) {
      return 'Yesterday';
    }
    
    return messageDate.toLocaleDateString('en-US', { 
      month: 'long', 
      day: 'numeric', 
      year: messageDate.getFullYear() !== today.getFullYear() ? 'numeric' : undefined 
    });
  }

  /**
   * Format message timestamp
   */
  formatMessageTime(date: Date): string {
    return new Date(date).toLocaleTimeString('en-US', { 
      hour: 'numeric', 
      minute: '2-digit',
      hour12: true 
    });
  }

  /**
   * Get participant name by ID
   */
  getParticipantName(senderId: number): string {
    const participant = this.thread?.participants.find(p => p.id === senderId);
    if (!participant) {
      return 'Unknown';
    }
    return `${participant.firstName} ${participant.lastName}`;
  }

  /**
   * Get participant avatar by ID
   */
  getParticipantAvatar(senderId: number): string | undefined {
    return this.thread?.participants.find(p => p.id === senderId)?.avatarUrl;
  }

  /**
   * Get participant initials by ID
   */
  getParticipantInitials(senderId: number): string {
    const participant = this.thread?.participants.find(p => p.id === senderId);
    if (!participant) {
      return 'U';
    }
    return `${participant.firstName.charAt(0)}${participant.lastName.charAt(0)}`;
  }

  /**
   * Get typing users display
   */
  getTypingUsersDisplay(): string {
    if (!this.thread?.typingUsers || this.thread.typingUsers.length === 0) {
      return '';
    }
    
    const names = this.thread.typingUsers
      .filter(u => u.id !== this.currentUserId)
      .map(u => u.firstName);
    
    if (names.length === 0) {
      return '';
    }
    
    if (names.length === 1) {
      return `${names[0]} is typing...`;
    }
    
    if (names.length === 2) {
      return `${names[0]} and ${names[1]} are typing...`;
    }
    
    return `${names[0]} and ${names.length - 1} others are typing...`;
  }

  /**
   * Get message character count
   */
  getMessageCharacterCount(): number {
    return this.messageControl.value?.length || 0;
  }

  /**
   * Check if user can send message
   */
  canSend(): boolean {
    const hasContent = this.messageControl.value?.trim();
    return (hasContent && hasContent.length > 0) || this.attachedFiles.length > 0;
  }

  /**
   * Format file size
   */
  formatFileSize(bytes: number): string {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1048576) return (bytes / 1024).toFixed(1) + ' KB';
    return (bytes / 1048576).toFixed(1) + ' MB';
  }

  /**
   * Scroll to bottom of message list
   */
  scrollToBottom(): void {
    try {
      if (this.messageList) {
        this.messageList.nativeElement.scrollTop = this.messageList.nativeElement.scrollHeight;
      }
    } catch (err) {
      console.error('Error scrolling to bottom:', err);
    }
  }

  /**
   * Track by function for messages
   */
  trackByMessageId(index: number, message: Message): number {
    return message.id;
  }
}
