import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Cleaner } from '../../types/Cleaner';
import { CleanersService } from '../../services/CleanersService';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-cleaner-last-run-info',
  imports: [DatePipe],
  templateUrl: './cleaner-last-run-info.component.html',
  styleUrl: './cleaner-last-run-info.component.scss',
})
export class CleanerLastRunInfoComponent {
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

  public get lastRunDate(): Date {
    return this._cleaner().lastRun;
  }
}
