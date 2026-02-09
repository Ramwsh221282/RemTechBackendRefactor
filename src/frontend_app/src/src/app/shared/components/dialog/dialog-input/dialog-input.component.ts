import { FormsModule } from '@angular/forms';
import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-dialog-input',
  imports: [FormsModule],
  templateUrl: './dialog-input.component.html',
  styleUrl: './dialog-input.component.scss',
})
export class DialogInputComponent {
  @Input() type: string = 'text';
  @Input() placeHolder: string = '';
  @Input({ required: true }) valueBind: string = '';
  @Output() valueBindChange: EventEmitter<string> = new EventEmitter();

  public onInputChange($event: Event): void {
    $event.preventDefault();
    const input = $event.target as HTMLInputElement;
    const newValue = input.value;
    this.valueBind = newValue;
    this.valueBindChange.emit(newValue);
  }
}
