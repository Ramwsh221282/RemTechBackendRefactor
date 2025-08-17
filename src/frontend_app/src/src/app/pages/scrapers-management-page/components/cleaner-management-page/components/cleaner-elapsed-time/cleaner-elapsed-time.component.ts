import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Cleaner } from '../../types/Cleaner';
import { CleanersService } from '../../services/CleanersService';

@Component({
  selector: 'app-cleaner-elapsed-time',
  imports: [],
  templateUrl: './cleaner-elapsed-time.component.html',
  styleUrl: './cleaner-elapsed-time.component.scss',
})
export class CleanerElapsedTimeComponent {
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

  public get hours(): number {
    return this._cleaner().hours;
  }

  public get seconds(): number {
    return this._cleaner().seconds;
  }

  public get minutes(): number {
    return this._cleaner().minutes;
  }
}
