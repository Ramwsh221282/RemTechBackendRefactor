import { Component, Input, signal, WritableSignal } from '@angular/core';

@Component({
  selector: 'app-blue-info-text',
  imports: [],
  templateUrl: './blue-info-text.component.html',
  styleUrl: './blue-info-text.component.scss',
})
export class BlueInfoTextComponent {
  @Input({ required: true }) set text_setter(value: string) {
    this.text.set(value);
  }
  public readonly text: WritableSignal<string>;
  constructor() {
    this.text = signal('');
  }
}
