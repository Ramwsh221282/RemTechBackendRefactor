import { Component, computed, effect, inject, Signal, signal, WritableSignal } from '@angular/core';
import { NgForOf, NgIf } from '@angular/common';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { Button } from 'primeng/button';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StringUtils } from '../../shared/utils/string-utils';
import { Router } from '@angular/router';
import { CategoryResponse } from '../../shared/api/categories-module/categories-responses';
import { CategoriesApiService } from '../../shared/api/categories-module/categories-api.service';
import { catchError, EMPTY, tap } from 'rxjs';
import { GetCategoriesQuery } from '../../shared/api/categories-module/categories-get-query';

@Component({
	selector: 'app-all-categories-page',
	imports: [NgForOf, PaginationComponent, NgIf, Button, ReactiveFormsModule],
	templateUrl: './all-categories-page.component.html',
	styleUrl: './all-categories-page.component.scss',
})
export class AllCategoriesPageComponent {
	private readonly _service: CategoriesApiService = inject(CategoriesApiService);
	private readonly _router: Router = inject(Router);

	readonly pageSize: number = 10;
	readonly page: WritableSignal<number> = signal(1);
	readonly text: WritableSignal<string | null> = signal(null);
	readonly categories: WritableSignal<CategoryResponse[]> = signal([]);
	readonly totalCount: WritableSignal<number> = signal(0);

	readonly categoriesQuery: Signal<GetCategoriesQuery> = computed((): GetCategoriesQuery => {
		const page: number = this.page();
		const text: string | null = this.text();
		return GetCategoriesQuery.default().usePage(page).usePageSize(this.pageSize).useTextSearch(text);
	});

	readonly fetchCategoriesOnQueryChange = effect((): void => {
		const query: GetCategoriesQuery = this.categoriesQuery();
		this.fetchCategories(query);
	});

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

	private fetchCategories(query: GetCategoriesQuery): void {
		this._service
			.fetchCategories(query)
			.pipe(
				tap((categories: CategoryResponse[]): void => this.categories.set(categories)),
				catchError(() => EMPTY),
			)
			.subscribe();
	}
}
