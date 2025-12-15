import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

/**
 * SearchBarComponent - Reusable search interface with debounced input
 * 
 * Usage:
 * <app-search-bar 
 *   [placeholder]="'Search people...'"
 *   [debounceTime]="300"
 *   (searchChanged)="onSearch($event)">
 * </app-search-bar>
 */
@Component({
  selector: 'app-search-bar',
  standalone: false,
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.scss']
})
export class SearchBarComponent {
  @Input() placeholder: string = 'Search...';
  @Input() debounceTime: number = 300;
  @Output() searchChanged = new EventEmitter<string>();

  searchTerm: string = '';
  private searchSubject = new Subject<string>();

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(this.debounceTime),
      distinctUntilChanged()
    ).subscribe(term => {
      this.searchChanged.emit(term);
    });
  }

  onSearchInput(value: string): void {
    this.searchTerm = value;
    this.searchSubject.next(value);
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.searchSubject.next('');
  }

  ngOnDestroy(): void {
    this.searchSubject.complete();
  }
}
