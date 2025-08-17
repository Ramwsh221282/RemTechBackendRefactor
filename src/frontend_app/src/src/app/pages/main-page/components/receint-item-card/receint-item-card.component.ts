import { Component, Input, signal, WritableSignal } from '@angular/core';
import { SomeRecentItem } from '../../types/SomeRecentItem';
import { ContainedItemsService } from '../../services/contained-items-service';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-receint-item-card',
  imports: [DecimalPipe],
  templateUrl: './receint-item-card.component.html',
  styleUrl: './receint-item-card.component.scss',
})
export class ReceintItemCardComponent {
  @Input({ required: true }) set item_setter(value: SomeRecentItem) {
    this._item.set(value);
  }
  private readonly _item: WritableSignal<SomeRecentItem>;
  constructor() {
    this._item = signal(ContainedItemsService.default());
  }

  public get item(): SomeRecentItem {
    return this._item();
  }
}
