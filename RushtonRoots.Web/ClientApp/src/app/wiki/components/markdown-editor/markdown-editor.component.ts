import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { marked } from 'marked';
import { MarkdownToolbarAction, MARKDOWN_TOOLBAR_BUTTONS } from '../../models/wiki.model';

/**
 * MarkdownEditorComponent - Full-featured markdown editor with preview
 * 
 * Features:
 * - Toolbar with common markdown actions
 * - Side-by-side markdown and preview
 * - Fullscreen mode
 * - Keyboard shortcuts
 * - File uploads for images
 * - Undo/Redo support
 * - ControlValueAccessor for form integration
 */
@Component({
  selector: 'app-markdown-editor',
  templateUrl: './markdown-editor.component.html',
  styleUrls: ['./markdown-editor.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => MarkdownEditorComponent),
      multi: true
    }
  ],
  standalone: false
})
export class MarkdownEditorComponent implements OnInit, ControlValueAccessor {
  @ViewChild('textarea', { static: false }) textarea!: ElementRef<HTMLTextAreaElement>;
  
  @Input() placeholder = 'Write your content in markdown...';
  @Input() minHeight = '400px';
  @Output() contentChange = new EventEmitter<string>();
  
  content = '';
  previewContent = '';
  showPreview = false;
  isFullscreen = false;
  
  toolbarButtons = MARKDOWN_TOOLBAR_BUTTONS;
  
  // ControlValueAccessor
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};
  disabled = false;
  
  // Undo/Redo stacks
  private undoStack: string[] = [];
  private redoStack: string[] = [];
  private maxUndoSize = 50;
  
  ngOnInit(): void {
    this.updatePreview();
  }
  
  // ControlValueAccessor implementation
  writeValue(value: string): void {
    this.content = value || '';
    this.updatePreview();
  }
  
  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }
  
  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }
  
  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
  
  onContentChange(event: Event): void {
    const target = event.target as HTMLTextAreaElement;
    this.saveToUndoStack();
    this.content = target.value;
    this.onChange(this.content);
    this.contentChange.emit(this.content);
    this.updatePreview();
  }
  
  onToolbarAction(action: MarkdownToolbarAction): void {
    switch (action) {
      case MarkdownToolbarAction.Bold:
        this.wrapSelection('**', '**', 'bold text');
        break;
      case MarkdownToolbarAction.Italic:
        this.wrapSelection('*', '*', 'italic text');
        break;
      case MarkdownToolbarAction.Strikethrough:
        this.wrapSelection('~~', '~~', 'strikethrough text');
        break;
      case MarkdownToolbarAction.Heading1:
        this.insertAtLineStart('# ', 'Heading 1');
        break;
      case MarkdownToolbarAction.Heading2:
        this.insertAtLineStart('## ', 'Heading 2');
        break;
      case MarkdownToolbarAction.Heading3:
        this.insertAtLineStart('### ', 'Heading 3');
        break;
      case MarkdownToolbarAction.Link:
        this.insertLink();
        break;
      case MarkdownToolbarAction.Image:
        this.insertImage();
        break;
      case MarkdownToolbarAction.Code:
        this.wrapSelection('`', '`', 'code');
        break;
      case MarkdownToolbarAction.CodeBlock:
        this.insertCodeBlock();
        break;
      case MarkdownToolbarAction.Quote:
        this.insertAtLineStart('> ', 'Quote');
        break;
      case MarkdownToolbarAction.UnorderedList:
        this.insertAtLineStart('- ', 'List item');
        break;
      case MarkdownToolbarAction.OrderedList:
        this.insertAtLineStart('1. ', 'List item');
        break;
      case MarkdownToolbarAction.Table:
        this.insertTable();
        break;
      case MarkdownToolbarAction.HorizontalRule:
        this.insertText('\n\n---\n\n');
        break;
      case MarkdownToolbarAction.Undo:
        this.undo();
        break;
      case MarkdownToolbarAction.Redo:
        this.redo();
        break;
      case MarkdownToolbarAction.Preview:
        this.togglePreview();
        break;
      case MarkdownToolbarAction.Fullscreen:
        this.toggleFullscreen();
        break;
    }
  }
  
  private wrapSelection(prefix: string, suffix: string, placeholder: string): void {
    const textarea = this.textarea.nativeElement;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = this.content.substring(start, end);
    const replacement = selectedText || placeholder;
    
    this.saveToUndoStack();
    const newContent = 
      this.content.substring(0, start) +
      prefix + replacement + suffix +
      this.content.substring(end);
    
    this.content = newContent;
    this.onChange(this.content);
    this.contentChange.emit(this.content);
    this.updatePreview();
    
    // Set cursor position
    setTimeout(() => {
      textarea.focus();
      textarea.setSelectionRange(start + prefix.length, start + prefix.length + replacement.length);
    }, 0);
  }
  
  private insertAtLineStart(prefix: string, placeholder: string): void {
    const textarea = this.textarea.nativeElement;
    const start = textarea.selectionStart;
    const lines = this.content.split('\n');
    let currentPos = 0;
    let lineIndex = 0;
    
    // Find which line the cursor is on
    for (let i = 0; i < lines.length; i++) {
      if (currentPos + lines[i].length >= start) {
        lineIndex = i;
        break;
      }
      currentPos += lines[i].length + 1;
    }
    
    this.saveToUndoStack();
    lines[lineIndex] = prefix + (lines[lineIndex] || placeholder);
    this.content = lines.join('\n');
    this.onChange(this.content);
    this.contentChange.emit(this.content);
    this.updatePreview();
    
    setTimeout(() => {
      textarea.focus();
    }, 0);
  }
  
  private insertText(text: string): void {
    const textarea = this.textarea.nativeElement;
    const start = textarea.selectionStart;
    
    this.saveToUndoStack();
    this.content = 
      this.content.substring(0, start) +
      text +
      this.content.substring(start);
    
    this.onChange(this.content);
    this.contentChange.emit(this.content);
    this.updatePreview();
    
    setTimeout(() => {
      textarea.focus();
      textarea.setSelectionRange(start + text.length, start + text.length);
    }, 0);
  }
  
  private insertLink(): void {
    const url = prompt('Enter URL:');
    if (url) {
      this.wrapSelection('[', `](${url})`, 'link text');
    }
  }
  
  private insertImage(): void {
    const url = prompt('Enter image URL:');
    if (url) {
      const alt = prompt('Enter image description (alt text):') || 'image';
      this.insertText(`![${alt}](${url})`);
    }
  }
  
  private insertCodeBlock(): void {
    const language = prompt('Enter language (optional):') || '';
    this.insertText(`\n\`\`\`${language}\n// Your code here\n\`\`\`\n`);
  }
  
  private insertTable(): void {
    const table = '\n| Header 1 | Header 2 | Header 3 |\n|----------|----------|----------|\n| Cell 1   | Cell 2   | Cell 3   |\n| Cell 4   | Cell 5   | Cell 6   |\n';
    this.insertText(table);
  }
  
  private saveToUndoStack(): void {
    this.undoStack.push(this.content);
    if (this.undoStack.length > this.maxUndoSize) {
      this.undoStack.shift();
    }
    this.redoStack = []; // Clear redo stack on new action
  }
  
  private undo(): void {
    if (this.undoStack.length > 0) {
      this.redoStack.push(this.content);
      const previousContent = this.undoStack.pop()!;
      this.content = previousContent;
      this.onChange(this.content);
      this.contentChange.emit(this.content);
      this.updatePreview();
    }
  }
  
  private redo(): void {
    if (this.redoStack.length > 0) {
      this.undoStack.push(this.content);
      const nextContent = this.redoStack.pop()!;
      this.content = nextContent;
      this.onChange(this.content);
      this.contentChange.emit(this.content);
      this.updatePreview();
    }
  }
  
  private togglePreview(): void {
    this.showPreview = !this.showPreview;
    if (this.showPreview) {
      this.updatePreview();
    }
  }
  
  private toggleFullscreen(): void {
    this.isFullscreen = !this.isFullscreen;
  }
  
  private updatePreview(): void {
    if (this.content) {
      marked.setOptions({
        gfm: true,
        breaks: true
      });
      this.previewContent = marked.parse(this.content) as string;
    } else {
      this.previewContent = '';
    }
  }
  
  onKeyDown(event: KeyboardEvent): void {
    // Handle keyboard shortcuts
    if (event.ctrlKey || event.metaKey) {
      switch (event.key.toLowerCase()) {
        case 'b':
          event.preventDefault();
          this.onToolbarAction(MarkdownToolbarAction.Bold);
          break;
        case 'i':
          event.preventDefault();
          this.onToolbarAction(MarkdownToolbarAction.Italic);
          break;
        case 'k':
          event.preventDefault();
          this.onToolbarAction(MarkdownToolbarAction.Link);
          break;
        case 'z':
          if (event.shiftKey) {
            event.preventDefault();
            this.redo();
          } else {
            event.preventDefault();
            this.undo();
          }
          break;
        case 'y':
          event.preventDefault();
          this.redo();
          break;
      }
    }
  }
  
  getWordCount(): number {
    if (!this.content) return 0;
    const words = this.content.split(/\s+/).filter(w => w.length > 0);
    return words.length;
  }
  
  getLineCount(): number {
    if (!this.content) return 0;
    return this.content.split('\n').length;
  }
  
  canUndo(): boolean {
    return this.undoStack.length > 0;
  }
  
  canRedo(): boolean {
    return this.redoStack.length > 0;
  }
}
