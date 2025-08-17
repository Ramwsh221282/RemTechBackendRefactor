import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { NgForOf, NgIf } from '@angular/common';
import { Spare } from './types/Spare';
import { SparesService } from './services/SparesService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { SparesSearchInputComponent } from './components/spares-search-input/spares-search-input.component';
import { SparePhotoComponent } from './components/spare-photo/spare-photo.component';
import { SpareContentComponent } from './components/spare-content/spare-content.component';
import { SpareTitleComponent } from './components/spare-title/spare-title.component';
import { SpareDetailsComponent } from './components/spare-details/spare-details.component';
import { SpareSourceComponent } from './components/spare-source/spare-source.component';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-spares-page',
  imports: [
    NgForOf,
    SparesSearchInputComponent,
    SparePhotoComponent,
    SpareContentComponent,
    SpareTitleComponent,
    SpareDetailsComponent,
    SpareSourceComponent,
    Paginator,
    NgIf,
  ],
  templateUrl: './spares-page.component.html',
  styleUrl: './spares-page.component.scss',
})
export class SparesPageComponent {
  private readonly _spares: WritableSignal<Spare[]>;
  private readonly _service: SparesService;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _textSearchString: WritableSignal<string | null>;
  private readonly _page: WritableSignal<number>;
  private readonly _totalCount: WritableSignal<number>;

  constructor(service: SparesService, activatedRoute: ActivatedRoute) {
    this._totalCount = signal(0);
    this._spares = signal([]);
    this._textSearchString = signal(null);
    this._page = signal(1);
    this._service = service;
    effect(() => {
      activatedRoute.queryParams
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (params) => {
            const textSearch: string | undefined = params['textSearch'];
            const page: string | undefined = params['page'];
            if (textSearch) {
              this._textSearchString.set(textSearch);
            }
            if (page) {
              this._page.set(Number(page));
            }
          },
        });
    });
    effect(() => {
      const page: number = this._page();
      const textSearch: string | null = this._textSearchString();
      this._service
        .fetch(page, textSearch)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: Spare[]): void => {
            this._spares.set(data);
          },
        });
    });
    effect(() => {
      const textSearch: string | null = this._textSearchString();
      this._service
        .count(textSearch)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (count: number): void => {
            this._totalCount.set(count);
          },
        });
    });
  }

  public get totalCount(): number {
    return this._totalCount();
  }

  public acceptPageChange(paginator: PaginatorState): void {
    const page: number | undefined = paginator.page;
    const totalPages: number | undefined = paginator.pageCount;
    if (!page || !totalPages) {
      this._page.set(1);
      return;
    }
    if (totalPages - page === 1) {
      this._page.set(totalPages);
      return;
    }
    const incremented: number = page + 1;
    this._page.set(incremented);
    window.scrollTo(0, 0);
  }

  public acceptTextSearch(text: string | null): void {
    console.log('accepted');
    this._textSearchString.set(text);
  }

  public get spares(): Spare[] {
    return this._spares();
  }
}
