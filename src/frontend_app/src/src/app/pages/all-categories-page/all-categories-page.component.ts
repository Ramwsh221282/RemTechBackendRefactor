import { Component, effect, signal, WritableSignal } from '@angular/core';
import { NgForOf, NgIf } from '@angular/common';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { Button } from 'primeng/button';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StringUtils } from '../../shared/utils/string-utils';
import { Router } from '@angular/router';
import { CategoryResponse } from '../../shared/api/categories-module/categories-responses';
import { CategoriesApiService } from '../../shared/api/categories-module/categories-api.service';
import { catchError, EMPTY, tap } from 'rxjs';

@Component({
  selector: 'app-all-categories-page',
  imports: [NgForOf, PaginationComponent, NgIf, Button, ReactiveFormsModule],
  templateUrl: './all-categories-page.component.html',
  styleUrl: './all-categories-page.component.scss',
})
export class AllCategoriesPageComponent {
  constructor(
    private readonly _service: CategoriesApiService,
    private readonly _router: Router,
  ) {
    this.page = signal(1);
    this.text = signal(null);
    this.categories = signal([]);
    this.totalCount = signal(0);
    effect(() => {
      const page: number = this.page();
      const text: string | null = this.text();
      this.fetchCategories(page, 10, text);
    });
  }

  readonly page: WritableSignal<number>;
  readonly text: WritableSignal<string | null>;
  readonly categories: WritableSignal<CategoryResponse[]>;
  readonly totalCount: WritableSignal<number>;
  readonly searchForm: FormGroup = new FormGroup({
    text: new FormControl(''),
  });

  public navigateByCategory(category: CategoryResponse): void {
    this._router.navigate(['vehicles'], {
      queryParams: {
        categoryId: category.Id,
        categoryName: category.Name,
        page: 1,
      },
    });
  }

  public resetSearchForm(): void {
    this.searchForm.reset();
    this.applyUserTextSearchInput(null);
  }

  public textSearchFormSubmit(): void {
    const input: string | null = this.readUserTextSearchInputFromForm();
    this.applyUserTextSearchInput(input);
  }

  public changePage(page: number): void {
    this.page.set(page);
  }

  private readUserTextSearchInputFromForm(): string | null {
    const formValues = this.searchForm.value;
    const text: string = formValues.text;
    return StringUtils.isEmptyOrWhiteSpace(text) ? null : text;
  }

  private applyUserTextSearchInput(input: string | null): void {
    this.text.set(input);
  }

  private fetchCategories(
    page: number,
    pageSize: number,
    text: string | null,
  ): void {
    this._service
      .fetchCategories(
        null,
        null,
        null,
        null,
        null,
        null,
        page,
        pageSize,
        true,
        text,
      )
      .pipe(
        tap((categories: CategoryResponse[]): void =>
          this.categories.set(categories),
        ),
        catchError(() => EMPTY),
      )
      .subscribe();
  }
}
