import {
  Component,
  EventEmitter,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { Paginator, PaginatorState } from 'primeng/paginator';

@Component({
  selector: 'app-pagination',
  imports: [Paginator],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss',
})
export class PaginationComponent {
  @Output() pageChanged: EventEmitter<number> = new EventEmitter();
  @Input({ required: true }) set total_count(value: number) {
    this._totalCount.set(value);
  }
  @Input({ required: true }) set current_page(value: number) {
    this._currentPage.set(value);
  }
  @Input({ required: true }) set page_size(value: number) {
    this._pageSize.set(value);
  }

  private readonly _totalCount: WritableSignal<number>;
  private readonly _currentPage: WritableSignal<number>;
  private readonly _pageSize: WritableSignal<number>;
  constructor() {
    this._totalCount = signal(0);
    this._currentPage = signal(0);
    this._pageSize = signal(0);
  }

  public get totalAmount(): number {
    return this._totalCount();
  }

  public get pageSize(): number {
    return this._pageSize();
  }

  public changePageClick(paginator: PaginatorState): void {
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
