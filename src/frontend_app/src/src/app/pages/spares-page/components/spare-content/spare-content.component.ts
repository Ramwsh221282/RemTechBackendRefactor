import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Spare } from '../../types/Spare';
import { SparesService } from '../../services/SparesService';
import { DecimalPipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-spare-content',
  imports: [],
  templateUrl: './spare-content.component.html',
  styleUrl: './spare-content.component.scss',
})
export class SpareContentComponent {
  @Input({ required: true }) set spare_setter(value: Spare) {
    this._spare.set(value);
  }

  private readonly _spare: WritableSignal<Spare>;
  constructor() {
    this._spare = signal(SparesService.default());
  }

  public get spare(): Spare {
    return this._spare();
  }
}
