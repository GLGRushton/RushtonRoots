import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { KeyboardNavigationService, KeyboardShortcut } from '../../services/keyboard-navigation.service';

/**
 * Dialog component showing available keyboard shortcuts
 */
@Component({
  selector: 'app-keyboard-shortcuts-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatDividerModule],
  templateUrl: './keyboard-shortcuts-dialog.component.html',
  styleUrls: ['./keyboard-shortcuts-dialog.component.scss']
})
export class KeyboardShortcutsDialogComponent {
  shortcuts: KeyboardShortcut[] = [];

  constructor(
    private keyboardService: KeyboardNavigationService,
    public dialogRef: MatDialogRef<KeyboardShortcutsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.shortcuts = this.keyboardService.getAllShortcuts();
  }

  /**
   * Format shortcut key combination for display
   */
  formatShortcut(shortcut: KeyboardShortcut): string {
    const parts: string[] = [];
    
    if (shortcut.ctrlKey) parts.push('Ctrl');
    if (shortcut.altKey) parts.push('Alt');
    if (shortcut.shiftKey) parts.push('Shift');
    if (shortcut.metaKey) parts.push('Cmd');
    parts.push(shortcut.key.toUpperCase());
    
    return parts.join(' + ');
  }

  /**
   * Close dialog
   */
  close(): void {
    this.dialogRef.close();
  }
}
