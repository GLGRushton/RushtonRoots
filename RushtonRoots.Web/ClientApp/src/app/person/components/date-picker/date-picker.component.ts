import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

/**
 * DatePickerComponent - Reusable date picker wrapper using Angular Material Datepicker
 * Supports form control binding with ControlValueAccessor
 */
@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.scss'],
  standalone: false,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DatePickerComponent),
      multi: true
    }
  ]
})
export class DatePickerComponent implements ControlValueAccessor {
  @Input() label = 'Select Date';
  @Input() placeholder = 'MM/DD/YYYY';
  @Input() minDate?: Date;
  @Input() maxDate?: Date;
  @Input() hint?: string;
  @Input() required = false;
  @Input() disabled = false;
  @Output() dateChange = new EventEmitter<Date | null>();

  value: Date | null = null;
  onChange: (value: Date | null) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: Date | string | null): void {
    if (value) {
      this.value = typeof value === 'string' ? new Date(value) : value;
    } else {
      this.value = null;
    }
  }

  registerOnChange(fn: (value: Date | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onDateChange(date: Date | null): void {
    this.value = date;
    this.onChange(date);
    this.onTouched();
    this.dateChange.emit(date);
  }
}
