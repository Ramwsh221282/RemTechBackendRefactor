import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Cleaner } from '../../types/Cleaner';
import { CleanersService } from '../../services/CleanersService';

@Component({
  selector: 'app-cleaner-processed-amount',
  imports: [],
  templateUrl: './cleaner-processed-amount.component.html',
  styleUrl: './cleaner-processed-amount.component.scss',
})
export class CleanerProcessedAmountComponent {
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

  public get processedAmount(): number {
    return this._cleaner().cleanedAmount;
  }
}
