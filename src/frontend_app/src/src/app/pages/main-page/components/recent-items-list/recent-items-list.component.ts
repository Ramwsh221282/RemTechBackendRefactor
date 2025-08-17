import {
  Component,
  DestroyRef,
  effect,
  inject,
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

@Component({
  selector: 'app-recent-items-list',
  imports: [ReceintItemCardComponent, ReceintItemsPaginationComponent],
  templateUrl: './recent-items-list.component.html',
  styleUrl: './recent-items-list.component.scss',
})
export class RecentItemsListComponent {
  private readonly _page: WritableSignal<number>;
  private readonly _items: WritableSignal<SomeRecentItem[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);

  constructor(service: ContainedItemsService) {
    this._page = signal(1);
    this._items = signal([]);
    effect(() => {
      const page: number = this._page();
      service
        .fetchRecent(page)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (recent: SomeRecentItem[]): void => {
            this._items.set(recent);
          },
        });
    });
  }

  public get items(): SomeRecentItem[] {
    return this._items();
  }

  public get page(): number {
    return this._page();
  }

  public acceptPageChange($event: number): void {
    this._page.set($event);
  }
}
