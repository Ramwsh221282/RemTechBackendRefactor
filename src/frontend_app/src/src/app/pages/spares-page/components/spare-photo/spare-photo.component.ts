import { Component, Input, signal, WritableSignal } from '@angular/core';
import { NgIf } from '@angular/common';
import { Spare } from '../../types/Spare';
import { SparesService } from '../../services/SparesService';

@Component({
  selector: 'app-spare-photo',
  imports: [NgIf],
  templateUrl: './spare-photo.component.html',
  styleUrl: './spare-photo.component.scss',
})
export class SparePhotoComponent {
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
