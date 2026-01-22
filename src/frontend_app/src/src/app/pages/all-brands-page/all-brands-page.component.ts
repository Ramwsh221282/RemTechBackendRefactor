import { Component, effect, signal, WritableSignal } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { StringUtils } from '../../shared/utils/string-utils';
import { Button } from 'primeng/button';
import { NgForOf, NgIf } from '@angular/common';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { Router } from '@angular/router';
import { BrandsApiService } from '../../shared/api/brands-module/brands-api.service';
import { BrandResponse } from '../../shared/api/brands-module/brands-api.responses';
import { catchError, EMPTY, Observable, tap } from 'rxjs';

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
  constructor(
    private readonly _service: BrandsApiService,
    private readonly _router: Router,
  ) {
    this.page = signal(1);
    this.text = signal(null);
    this.brands = signal([]);
    this.totalCount = signal(0);
    effect(() => {
      const page: number = this.page();
      const text: string | null = this.text();
      this.fetchBrands(page, text);
    });
  }

  readonly page: WritableSignal<number>;
  readonly text: WritableSignal<string | null>;
  readonly brands: WritableSignal<BrandResponse[]>;
  readonly totalCount: WritableSignal<number>;
  readonly searchForm: FormGroup = new FormGroup({
    text: new FormControl(''),
  });

  public handlePageChange(page: number): void {
    console.log(page);
  }

  public resetSearchForm(): void {
    this.searchForm.reset();
    this.submitSearch(null);
  }

  public textSearchFormSubmit(): void {
    const input: string | null = this.readUserTextSearchInput();
    this.submitSearch(input);
  }

  public navigateByBrand(brand: BrandResponse): void {
    this._router.navigate(['vehicles'], {
      queryParams: {
        brandId: brand.Id,
        brandName: brand.Name,
        page: 1,
      },
    });
  }

  private readUserTextSearchInput(): string | null {
    const formValues = this.searchForm.value;
    const text: string = formValues.text;
    return StringUtils.isEmptyOrWhiteSpace(text) ? null : text;
  }

  private submitSearch(input: string | null): void {
    this.text.set(input);
  }

  private fetchBrands(page: number, text: string | null): void {
    this._service
      .fetchBrands(
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        page,
        30,
        text,
        true,
        true,
      )
      .pipe(
        tap((response: BrandResponse[]): void => {
          this.brands.set(response);
          if (response.length > 0 && response[0].TotalCount)
            this.totalCount.set(response[0].TotalCount);
        }),
        catchError((): Observable<never> => EMPTY),
      )
      .subscribe();
  }
}
