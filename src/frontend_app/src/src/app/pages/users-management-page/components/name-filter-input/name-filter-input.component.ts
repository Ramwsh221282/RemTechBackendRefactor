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
  selector: 'app-name-filter-input',
  imports: [],
  templateUrl: './name-filter-input.component.html',
  styleUrl: './name-filter-input.component.scss',
})
export class NameFilterInputComponent {
  @Output() onNameFilterChange: EventEmitter<string | null> =
    new EventEmitter();
  @Input({ required: true }) set name_setter(value: string | null) {
    this._nameFilterInput.set(value);
  }
  private readonly _nameFilterInput: WritableSignal<string | null>;
  constructor() {
    this._nameFilterInput = signal('');
  }
  public get nameInput(): string | null {
    return this._nameFilterInput();
  }

  public nameChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const inputValue = input.value;
    if (StringUtils.isEmptyOrWhiteSpace(inputValue)) {
      this.onNameFilterChange.emit(null);
      this._nameFilterInput.set(null);
      return;
    }
    this._nameFilterInput.set(inputValue);
    this.onNameFilterChange.emit(inputValue);
  }

  public clearFilter(): void {
    this._nameFilterInput.set(null);
    this.onNameFilterChange.emit(null);
  }
}
