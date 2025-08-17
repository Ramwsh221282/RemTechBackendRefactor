import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { NgForOf, NgIf } from '@angular/common';
import { ContainedItemsInfoComponent } from '../main-page/components/contained-items-info/contained-items-info.component';
import { PopularBrandsBlockComponent } from '../main-page/components/popular-brands-block/popular-brands-block.component';
import { PopularCategoriesBlockComponent } from '../main-page/components/popular-categories-block/popular-categories-block.component';
import { RecentItemsListComponent } from '../main-page/components/recent-items-list/recent-items-list.component';
import { QueryCategoriesResponse } from './types/QueryCategoriesResponse';
import { AllCategoriesService } from './services/AllCategoriesService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { Button } from 'primeng/button';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StringUtils } from '../../shared/utils/string-utils';

@Component({
  selector: 'app-all-categories-page',
  imports: [NgForOf, PaginationComponent, NgIf, Button, ReactiveFormsModule],
  templateUrl: './all-categories-page.component.html',
  styleUrl: './all-categories-page.component.scss',
})
export class AllCategoriesPageComponent {
  private readonly _page: WritableSignal<number>;
  private readonly _text: WritableSignal<string | null>;
  private readonly _categories: WritableSignal<QueryCategoriesResponse[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _totalCount: WritableSignal<number>;
  public readonly searchForm: FormGroup = new FormGroup({
    text: new FormControl(''),
  });

  constructor(service: AllCategoriesService) {
    this._page = signal(1);
    this._text = signal(null);
    this._categories = signal([]);
    this._totalCount = signal(0);
    effect(() => {
      const text: string | null = this._text();
      service
        .fetchCount(text)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (amount: number): void => {
            this._totalCount.set(amount);
          },
        });
    });
    effect(() => {
      const page: number = this._page();
      const text: string | null = this._text();
      service
        .fetchCategories(page, text)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: QueryCategoriesResponse[]): void => {
            this._categories.set(data);
          },
        });
    });
  }

  public resetSearchForm(): void {
    this.searchForm.reset();
    this.submitSearch();
  }

  public textSearchFormSubmit(): void {
    this.submitSearch();
  }

  private submitSearch(): void {
    const formValues = this.searchForm.value;
    const text: string = formValues.text;
    if (StringUtils.isEmptyOrWhiteSpace(text)) {
      this._text.set(null);
      return;
    }
    this._text.set(text);
  }

  public get currentPage(): number {
    return this._page();
  }

  public get totalCount(): number {
    return this._totalCount();
  }

  public changePage(page: number): void {
    this._page.set(page);
  }

  public get categories(): QueryCategoriesResponse[] {
    return this._categories();
  }
}
