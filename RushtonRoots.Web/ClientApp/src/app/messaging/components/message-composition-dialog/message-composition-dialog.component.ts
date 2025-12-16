import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageCompositionData, Participant } from '../../models/messaging.model';

/**
 * Phase 8.3: Message Composition Dialog Component
 * 
 * Dialog for composing new messages or starting new conversations.
 * Allows selecting recipients, adding subject, composing message, and attaching files.
 * 
 * Features:
 * - Recipient selection with autocomplete
 * - Optional subject line
 * - Message composition textarea
 * - File attachments
 * - Character count
 * - Send and cancel actions
 */
@Component({
  selector: 'app-message-composition-dialog',
  standalone: false,
  templateUrl: './message-composition-dialog.component.html',
  styleUrls: ['./message-composition-dialog.component.scss']
})
export class MessageCompositionDialogComponent implements OnInit {
  messageForm: FormGroup;
  availableRecipients: Participant[] = [];
  selectedRecipients: Participant[] = [];
  attachedFiles: File[] = [];
  maxMessageLength = 2000;
  maxSubjectLength = 100;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<MessageCompositionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { 
      recipients?: Participant[];
      availableRecipients?: Participant[];
      subject?: string;
      replyToMessageId?: number;
    }
  ) {
    this.availableRecipients = data.availableRecipients || [];
    
    this.messageForm = this.fb.group({
      subject: [data.subject || '', [Validators.maxLength(this.maxSubjectLength)]],
      content: ['', [Validators.required, Validators.maxLength(this.maxMessageLength)]],
      recipients: [[], [Validators.required, Validators.minLength(1)]]
    });
  }

  ngOnInit(): void {
    if (this.data.recipients && this.data.recipients.length > 0) {
      this.selectedRecipients = this.data.recipients;
      this.messageForm.patchValue({
        recipients: this.data.recipients.map(r => r.id)
      });
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
      
      // Reset input to allow selecting the same file again
      input.value = '';
    }
  }

  /**
   * Remove attached file
   */
  removeFile(index: number): void {
    this.attachedFiles.splice(index, 1);
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
   * Handle recipient selection
   */
  onRecipientSelectionChange(participants: Participant[]): void {
    this.selectedRecipients = participants;
    this.messageForm.patchValue({
      recipients: participants.map(p => p.id)
    });
  }

  /**
   * Add a recipient from autocomplete
   */
  addRecipient(participant: Participant): void {
    if (!this.isRecipientSelected(participant)) {
      this.selectedRecipients.push(participant);
      this.messageForm.patchValue({
        recipients: this.selectedRecipients.map(p => p.id)
      });
    }
  }

  /**
   * Check if recipient is already selected
   */
  isRecipientSelected(participant: Participant): boolean {
    return this.selectedRecipients.some(p => p.id === participant.id);
  }

  /**
   * Remove recipient
   */
  removeRecipient(participant: Participant): void {
    this.selectedRecipients = this.selectedRecipients.filter(p => p.id !== participant.id);
    this.messageForm.patchValue({
      recipients: this.selectedRecipients.map(p => p.id)
    });
  }

  /**
   * Get participant initials
   */
  getParticipantInitials(participant: Participant): string {
    return `${participant.firstName.charAt(0)}${participant.lastName.charAt(0)}`;
  }

  /**
   * Send message
   */
  onSend(): void {
    if (this.messageForm.valid) {
      const formValue = this.messageForm.value;
      const messageData: MessageCompositionData = {
        recipientIds: formValue.recipients,
        subject: formValue.subject || undefined,
        content: formValue.content,
        attachments: this.attachedFiles.length > 0 ? this.attachedFiles : undefined,
        replyToMessageId: this.data.replyToMessageId
      };
      
      this.dialogRef.close(messageData);
    }
  }

  /**
   * Cancel and close dialog
   */
  onCancel(): void {
    if (this.messageForm.dirty) {
      if (confirm('Discard this message?')) {
        this.dialogRef.close();
      }
    } else {
      this.dialogRef.close();
    }
  }

  /**
   * Get character count display
   */
  getCharacterCount(field: 'content' | 'subject'): string {
    const value = this.messageForm.get(field)?.value || '';
    const maxLength = field === 'content' ? this.maxMessageLength : this.maxSubjectLength;
    return `${value.length}/${maxLength}`;
  }
}
