import { Component, Input, Output, EventEmitter } from '@angular/core';

/**
 * EmptyStateComponent - Display "no data" states with optional action
 * 
 * Usage:
 * <app-empty-state 
 *   [icon]="'person_off'"
 *   [message]="'No people found'"
 *   [submessage]="'Try adjusting your search criteria'"
 *   [actionLabel]="'Add Person'"
 *   (actionClick)="onAddPerson()">
 * </app-empty-state>
 */
@Component({
  selector: 'app-empty-state',
  standalone: false,
  templateUrl: './empty-state.component.html',
  styleUrls: ['./empty-state.component.scss']
})
export class EmptyStateComponent {
  @Input() icon: string = 'inbox';
  @Input() message: string = 'No data available';
  @Input() submessage?: string;
  @Input() actionLabel?: string;
  @Output() actionClick = new EventEmitter<void>();

  onAction(): void {
    this.actionClick.emit();
  }
}
