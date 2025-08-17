import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Cleaner } from '../../types/Cleaner';
import { CleanersService } from '../../services/CleanersService';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-cleaner-state-select',
  imports: [NgClass],
  templateUrl: './cleaner-state-select.component.html',
  styleUrl: './cleaner-state-select.component.scss',
})
export class CleanerStateSelectComponent {
  @Input({ required: true }) set cleaner_setter(value: Cleaner) {
    this._cleaner.set(value);
  }

  private readonly _cleaner: WritableSignal<Cleaner>;

  constructor() {
    this._cleaner = signal(CleanersService.default());
  }

  public get cleaner(): Cleaner {
    return this._cleaner();
  }

  public get currentState(): string {
    return this._cleaner().state;
  }
}
