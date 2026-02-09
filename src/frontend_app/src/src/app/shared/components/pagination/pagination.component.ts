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
    this.totalCount.set(value);
  }

  @Input({ required: true }) set page_size(value: number) {
    this.pageSize.set(value);
  }

  @Input({ required: true }) set current_page(value: number) {
    this.page.set(value);
  }

  readonly totalCount: WritableSignal<number> = signal(0);
  readonly pageSize: WritableSignal<number> = signal(0);
  readonly page: WritableSignal<number> = signal(0);

  public changePageClick(paginator: PaginatorState): void {
    const page: number = this.resolvePage(paginator);
    this.pageChanged.emit(page);
  }

  private resolvePage(paginator: PaginatorState): number {
    return paginator.page ? paginator.page + 1 : 1;
  }
}
