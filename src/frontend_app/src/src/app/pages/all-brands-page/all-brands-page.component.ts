import { Component, computed, effect, inject, Signal, signal, WritableSignal } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { StringUtils } from '../../shared/utils/string-utils';
import { Button } from 'primeng/button';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { Router } from '@angular/router';
import { BrandsApiService } from '../../shared/api/brands-module/brands-api.service';
import { BrandResponse } from '../../shared/api/brands-module/brands-api.responses';
import { catchError, EMPTY, Observable, tap } from 'rxjs';
import { GetBrandsQuery } from '../../shared/api/brands-module/brands-get-query';

@Component({
	selector: 'app-all-brands-page',
	imports: [Button, FormsModule, PaginationComponent, ReactiveFormsModule],
	templateUrl: './all-brands-page.component.html',
	styleUrl: './all-brands-page.component.scss',
})
export class AllBrandsPageComponent {
	private readonly _service: BrandsApiService = inject(BrandsApiService);
	private readonly _router: Router = inject(Router);

	readonly pageSize: number = 10;
	readonly page: WritableSignal<number> = signal(1);
	readonly text: WritableSignal<string | null> = signal(null);
	readonly brands: WritableSignal<BrandResponse[]> = signal([]);
	readonly usesSortByName: WritableSignal<boolean> = signal(true);
	readonly usesSortByVehiclesCount: WritableSignal<boolean> = signal(false);
	readonly sortDirection: WritableSignal<'ASC' | 'DESC'> = signal('ASC');
	readonly totalCount: WritableSignal<number> = signal(0);
	readonly searchForm: FormGroup = new FormGroup({
		text: new FormControl(''),
	});

	readonly brandsQuery: Signal<GetBrandsQuery> = computed((): GetBrandsQuery => {
		return GetBrandsQuery.default()
			.usePage(this.page())
			.usePageSize(this.pageSize)
			.useTextSearch(this.text())
			.useVehiclesCount(true)
			.useSortByName(this.usesSortByName())
			.useSortByVehiclesCount(this.usesSortByVehiclesCount())
			.useSortDirection(this.sortDirection())
			.useTotalBrandsCount(true);
	});

	readonly fetchBrandsOnQueryChange = effect((): void => {
		const query: GetBrandsQuery = this.brandsQuery();
		this.fetchBrands(query);
	});

	public swapSortMode(): void {
		const current: 'ASC' | 'DESC' = this.sortDirection();
		const next: 'ASC' | 'DESC' = current === 'ASC' ? 'DESC' : 'ASC';
		this.sortDirection.set(next);
	}

	public sortModeLabel(): string {
		const current: 'ASC' | 'DESC' = this.sortDirection();
		return current === 'ASC' ? 'Возрастание' : 'Убывание';
	}

	public severityByUsing(uses: boolean): 'primary' | 'success' {
		return uses ? 'success' : 'primary';
	}

	public handlePageChange(page: number): void {
		this.page.set(page);
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

	private fetchBrands(query: GetBrandsQuery): void {
		this._service
			.fetchBrands(query)
			.pipe(
				tap((response: BrandResponse[]): void => {
					this.brands.set(response);
					if (response.length > 0 && response[0].TotalCount) this.totalCount.set(response[0].TotalCount);
				}),
				catchError((): Observable<never> => EMPTY),
			)
			.subscribe();
	}
}
