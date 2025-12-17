import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { KeyboardNavigationService, KeyboardShortcut } from '../../../accessibility/services/keyboard-navigation.service';

interface ShortcutGroup {
  title: string;
  shortcuts: KeyboardShortcut[];
}

/**
 * KeyboardShortcutsDialogComponent - Dialog showing all keyboard shortcuts
 * 
 * Displays a comprehensive list of all available keyboard shortcuts
 * organized by category.
 */
@Component({
  selector: 'app-keyboard-shortcuts-dialog',
  standalone: false,
  templateUrl: './keyboard-shortcuts-dialog.component.html',
  styleUrls: ['./keyboard-shortcuts-dialog.component.scss']
})
export class KeyboardShortcutsDialogComponent implements OnInit {
  shortcutGroups: ShortcutGroup[] = [];

  constructor(
    public dialogRef: MatDialogRef<KeyboardShortcutsDialogComponent>,
    private keyboardService: KeyboardNavigationService
  ) {}

  ngOnInit(): void {
    this.loadShortcuts();
  }

  /**
   * Load and organize shortcuts
   */
  private loadShortcuts(): void {
    const allShortcuts = this.keyboardService.getAllShortcuts();

    this.shortcutGroups = [
      {
        title: 'Navigation',
        shortcuts: [
          { key: 'h', altKey: true, description: 'Go to Home', action: () => {} },
          { key: 'p', altKey: true, description: 'Go to People', action: () => {} },
          { key: 's', altKey: true, description: 'Go to Search', action: () => {} }
        ]
      },
      {
        title: 'Search & Focus',
        shortcuts: allShortcuts.filter(s => 
          s.description.toLowerCase().includes('search') || 
          s.description.toLowerCase().includes('focus')
        )
      },
      {
        title: 'Accessibility',
        shortcuts: allShortcuts.filter(s =>
          s.description.toLowerCase().includes('skip') ||
          s.description.toLowerCase().includes('accessibility')
        )
      },
      {
        title: 'Help',
        shortcuts: allShortcuts.filter(s =>
          s.description.toLowerCase().includes('help') ||
          s.description.toLowerCase().includes('shortcuts')
        )
      }
    ].filter(group => group.shortcuts.length > 0);
  }

  /**
   * Format shortcut key combination
   */
  formatShortcut(shortcut: KeyboardShortcut): string {
    const keys: string[] = [];

    if (shortcut.ctrlKey) keys.push('Ctrl');
    if (shortcut.altKey) keys.push('Alt');
    if (shortcut.shiftKey) keys.push('Shift');
    if (shortcut.metaKey) keys.push('Cmd');
    
    keys.push(shortcut.key.toUpperCase());

    return keys.join(' + ');
  }

  /**
   * Close dialog
   */
  close(): void {
    this.dialogRef.close();
  }
}
