import { Component, Input, signal, WritableSignal } from '@angular/core';

@Component({
  selector: 'app-gray-text-info',
  imports: [],
  templateUrl: './gray-text-info.component.html',
  styleUrl: './gray-text-info.component.scss',
})
export class GrayTextInfoComponent {
  @Input({ required: true }) set text_setter(value: string) {
    this.text.set(value);
  }

  public readonly text: WritableSignal<string>;

  constructor() {
    this.text = signal('');
  }
}
