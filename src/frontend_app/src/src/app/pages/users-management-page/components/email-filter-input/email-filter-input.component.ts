import {
  Component,
  EventEmitter,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { StringUtils } from '../../../../shared/utils/string-utils';

@Component({
  selector: 'app-email-filter-input',
  imports: [],
  templateUrl: './email-filter-input.component.html',
  styleUrl: './email-filter-input.component.scss',
})
export class EmailFilterInputComponent {
  @Output() onEmailInputChange: EventEmitter<string | null> =
    new EventEmitter();
  @Input({ required: true }) set email_setter(value: string | null) {
    this._emailFilterInput.set(value);
  }
  private readonly _emailFilterInput: WritableSignal<string | null>;
  constructor() {
    this._emailFilterInput = signal('');
  }
  public get emailInput(): string | null {
    return this._emailFilterInput();
  }
  public emailChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const inputValue = input.value;
    if (StringUtils.isEmptyOrWhiteSpace(inputValue)) {
      this.onEmailInputChange.emit(null);
      this._emailFilterInput.set(null);
      return;
    }
    this.onEmailInputChange.emit(inputValue);
    this._emailFilterInput.set(inputValue);
  }

  public clearFilter(): void {
    this._emailFilterInput.set(null);
    this.onEmailInputChange.emit(null);
  }
}
