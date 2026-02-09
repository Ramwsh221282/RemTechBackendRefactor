import { Component, Input, signal, WritableSignal } from '@angular/core';
import { ItemStats } from '../../../../shared/api/main-page/main-page-responses';

@Component({
  selector: 'app-contained-items-info',
  imports: [],
  templateUrl: './contained-items-info.component.html',
  styleUrl: './contained-items-info.component.scss',
})
export class ContainedItemsInfoComponent {
  constructor() {
    this.sparesCount = signal(0);
    this.vehiclesCount = signal(0);
  }

  readonly sparesCount: WritableSignal<number>;
  readonly vehiclesCount: WritableSignal<number>;

  @Input({ required: true }) set item_stats(value: ItemStats[]) {
    for (const item of value) {
      if (item.ItemType === 'Запчасти') this.sparesCount.set(item.Count);
      if (item.ItemType === 'Техника') this.vehiclesCount.set(item.Count);
    }
  }
}
