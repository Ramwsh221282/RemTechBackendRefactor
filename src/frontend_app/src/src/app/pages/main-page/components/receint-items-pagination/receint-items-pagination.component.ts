import {
  Component,
  DestroyRef,
  effect,
  EventEmitter,
  inject,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { ContainedItemsService } from '../../services/contained-items-service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-receint-items-pagination',
  imports: [Paginator, NgIf],
  templateUrl: './receint-items-pagination.component.html',
  styleUrl: './receint-items-pagination.component.scss',
})
export class ReceintItemsPaginationComponent {
  @Input({ required: true }) set page_setter(value: number) {
    this._currentPage.set(value);
  }
  @Output() pageChanged: EventEmitter<number> = new EventEmitter();
  private readonly _currentPage: WritableSignal<number>;
  private readonly _totalCount: WritableSignal<number>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private _isCountInited: boolean = false;
  constructor(service: ContainedItemsService) {
    this._currentPage = signal(1);
    this._totalCount = signal(0);
    effect(() => {
      if (this._isCountInited) return;
      service
        .fetchRecentCount()
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (value: number): void => {
            this._totalCount.set(value);
            this._isCountInited = true;
          },
        });
    });
  }

  public get totalAmount(): number {
    return this._totalCount();
  }

  public acceptPageChange(paginator: PaginatorState): void {
    const page: number | undefined = paginator.page;
    const totalPages: number | undefined = paginator.pageCount;
    if (!page || !totalPages) {
      this.changePage(1);
      return;
    }
    if (totalPages - page === 1) {
      this.changePage(totalPages);
      return;
    }
    const incremented: number = page + 1;
    this.changePage(incremented);
  }

  private changePage(page: number): void {
    this._currentPage.set(page);
    this.pageChanged.emit(page);
  }
}
