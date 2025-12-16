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
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule } from '@angular/material/dialog';
import { MatBadgeModule } from '@angular/material/badge';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { TextFieldModule } from '@angular/cdk/text-field';

// Messaging Components
import { MessageThreadComponent } from './components/message-thread/message-thread.component';
import { ChatInterfaceComponent } from './components/chat-interface/chat-interface.component';
import { NotificationPanelComponent } from './components/notification-panel/notification-panel.component';
import { MessageCompositionDialogComponent } from './components/message-composition-dialog/message-composition-dialog.component';

/**
 * Phase 8.3: Messaging Module
 * 
 * This module contains all messaging and notification-related components.
 * 
 * Components:
 * - MessageThreadComponent: Display message thread list
 * - ChatInterfaceComponent: Real-time chat interface
 * - NotificationPanelComponent: Notification panel with grouping
 * - MessageCompositionDialogComponent: Dialog for composing new messages
 * 
 * Features:
 * - Real-time messaging with typing indicators
 * - Message threads with unread counts
 * - Notification grouping and filtering
 * - File attachments
 * - Message composition dialog
 * - Responsive design for mobile
 */
@NgModule({
  declarations: [
    MessageThreadComponent,
    ChatInterfaceComponent,
    NotificationPanelComponent,
    MessageCompositionDialogComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    
    // Material Modules
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatChipsModule,
    MatMenuModule,
    MatTooltipModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatBadgeModule,
    MatExpansionModule,
    MatAutocompleteModule,
    TextFieldModule
  ],
  exports: [
    MessageThreadComponent,
    ChatInterfaceComponent,
    NotificationPanelComponent,
    MessageCompositionDialogComponent
  ]
})
export class MessagingModule { }
