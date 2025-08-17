import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Spare } from '../../types/Spare';
import { SparesService } from '../../services/SparesService';

@Component({
  selector: 'app-spare-title',
  imports: [],
  templateUrl: './spare-title.component.html',
  styleUrl: './spare-title.component.scss',
})
export class SpareTitleComponent {
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
