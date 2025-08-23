import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { QueryCategoriesResponse } from '../all-categories-page/types/QueryCategoriesResponse';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { StringUtils } from '../../shared/utils/string-utils';
import { QueryBrandResponse } from './types/QueryBrandResponse';
import { AllBrandsService } from './services/AllBrandsService';
import { Button } from 'primeng/button';
import { NgForOf, NgIf } from '@angular/common';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { PopularBrand } from '../main-page/types/PopularBrand';
import { Router } from '@angular/router';

@Component({
  selector: 'app-all-brands-page',
  imports: [
    Button,
    FormsModule,
    NgForOf,
    NgIf,
    PaginationComponent,
    ReactiveFormsModule,
  ],
  templateUrl: './all-brands-page.component.html',
  styleUrl: './all-brands-page.component.scss',
})
export class AllBrandsPageComponent {
  private readonly _page: WritableSignal<number>;
  private readonly _text: WritableSignal<string | null>;
  private readonly _brands: WritableSignal<QueryBrandResponse[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _totalCount: WritableSignal<number>;
  public readonly searchForm: FormGroup = new FormGroup({
    text: new FormControl(''),
  });

  constructor(
    service: AllBrandsService,
    private readonly _router: Router,
  ) {
    this._page = signal(1);
    this._text = signal(null);
    this._brands = signal([]);
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
        .fetchBrands(page, text)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: QueryCategoriesResponse[]): void => {
            this._brands.set(data);
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

  public navigateByBrand(brand: PopularBrand): void {
    this._router.navigate(['vehicles'], {
      queryParams: {
        brandId: brand.id,
        brandName: brand.name,
        page: 1,
      },
    });
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

  public get brands(): QueryBrandResponse[] {
    return this._brands();
  }
}
