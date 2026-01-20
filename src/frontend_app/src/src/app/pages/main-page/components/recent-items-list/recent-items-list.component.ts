import {
  Component,
  DestroyRef,
  effect,
  inject,
  Input,
  signal,
  WritableSignal,
} from '@angular/core';
import { ContainedItemsService } from '../../services/contained-items-service';
import { SomeRecentItem } from '../../types/SomeRecentItem';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReceintItemCardComponent } from '../receint-item-card/receint-item-card.component';
import { ReceintItemsPaginationComponent } from '../receint-items-pagination/receint-items-pagination.component';
import { NgIf } from '@angular/common';
import { Paginator } from 'primeng/paginator';
import { VehicleResponse } from '../../../../shared/api/vehicles-module/vehicles-api.responses';

@Component({
  selector: 'app-recent-items-list',
  imports: [ReceintItemCardComponent, ReceintItemsPaginationComponent],
  templateUrl: './recent-items-list.component.html',
  styleUrl: './recent-items-list.component.scss',
})
export class RecentItemsListComponent {
  constructor() {
    this._page = signal(1);
    this.vehicles = signal([]);
    // effect(() => {
    //   const page: number = this._page();
    //   service
    //     .fetchRecent(page)
    //     .pipe(takeUntilDestroyed(this._destroyRef))
    //     .subscribe({
    //       next: (recent: SomeRecentItem[]): void => {
    //         this._items.set(recent);
    //       },
    //     });
    // });
  }

  @Input({ required: true }) set vehicles_setter(value: VehicleResponse[]) {
    this.vehicles.set(value);
  }

  readonly vehicles: WritableSignal<VehicleResponse[]>;
  private readonly _page: WritableSignal<number>;

  public get page(): number {
    return this._page();
  }

  public acceptPageChange($event: number): void {
    this._page.set($event);
  }
}
