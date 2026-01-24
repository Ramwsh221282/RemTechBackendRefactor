import { Component, computed, DestroyRef, effect, inject, Signal, signal, WritableSignal } from '@angular/core';
import { CatalogueVehicle } from './types/CatalogueVehicle';
import { ActivatedRoute, Params } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { VehiclesTextSearchComponent } from './components/vehicles-text-search/vehicles-text-search.component';
import { VehiclesPriceSortComponent } from './components/vehicles-price-sort/vehicles-price-sort.component';
import {
	PriceSubmitEvent,
	VehiclePriceFilterFormPartComponent,
} from './components/vehicle-price-filter-form-part/vehicle-price-filter-form-part.component';
import { VehicleCategoryFilterFormPartComponent } from './components/vehicle-category-filter-form-part/vehicle-category-filter-form-part.component';
import { VehicleBrandFilterFormPartComponent } from './components/vehicle-brand-filter-form-part/vehicle-brand-filter-form-part.component';
import { VehicleModelFilterFormPartComponent } from './components/vehicle-model-filter-form-part/vehicle-model-filter-form-part.component';
import {
	VehicleRegionsFilterFormPartComponent,
	VehicleRegionsFilterFormPartProps,
} from './components/vehicle-regions-filter-form-part/vehicle-regions-filter-form-part.component';
import { VehicleCardComponent } from './components/vehicle-card/vehicle-card.component';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { VehiclesApiService } from '../../shared/api/vehicles-module/vehicles-api.service';
import { CategoryResponse } from '../../shared/api/categories-module/categories-responses';
import { BrandResponse } from '../../shared/api/brands-module/brands-api.responses';
import { ModelResponse } from '../../shared/api/models-module/models-responses';
import { catchError, EMPTY, tap } from 'rxjs';
import { GetVehiclesQueryResponse } from '../../shared/api/vehicles-module/vehicles-api.responses';
import { LocationsApiService } from '../../shared/api/locations-module/locations-api.service';
import { LocationResponse } from '../../shared/api/locations-module/locations.responses';
import { GetLocationsQueryParameters } from '../../shared/api/locations-module/locations.requests';
import { DefaultLocationQuery, LocationsQueryWithOrderByName } from '../../shared/api/locations-module/locations.factories';
import { CategoriesApiService } from '../../shared/api/categories-module/categories-api.service';
import { BrandsApiService } from '../../shared/api/brands-module/brands-api.service';
import { ModelsApiService } from '../../shared/api/models-module/models-api.service';
import { GetCategoriesQuery } from '../../shared/api/categories-module/categories-get-query';
import { GetBrandsQuery } from '../../shared/api/brands-module/brands-get-query';
import { GetVehiclesQuery } from '../../shared/api/vehicles-module/vehicles-get-query';

type ActivatedVehicleRouteParams = {
	category: CategoryResponse | null | undefined;
	brand: BrandResponse | null | undefined;
	model: ModelResponse | null | undefined;
	location: LocationResponse | null | undefined;
};

type AggregatedStatisticsInfo = {
	totalVehiclesCount: number;
	averagePrice: number;
	minimalPrice: number;
	maximalPrice: number;
};

export const defaultAggregatedStatisticsInfo: () => AggregatedStatisticsInfo = () => {
	return {
		totalVehiclesCount: 0,
		averagePrice: 0,
		minimalPrice: 0,
		maximalPrice: 0,
	};
};

export const defaultActivatedVehicleRouteParams: () => ActivatedVehicleRouteParams = () => {
	return { brand: undefined, category: undefined, model: undefined, location: undefined };
};

export type VehiclesCatalogueFilterSeletionsModel = {
	category: CategoryResponse | null | undefined;
	brand: BrandResponse | null | undefined;
	model: ModelResponse | null | undefined;
	location: LocationResponse | null | undefined;
};

export class VehiclesCatalogueSelectFiltersBearer {
	private constructor(private readonly _model: VehiclesCatalogueFilterSeletionsModel) {}

	public useCategory(category: CategoryResponse | null | undefined): VehiclesCatalogueSelectFiltersBearer {
		return new VehiclesCatalogueSelectFiltersBearer({ ...this._model, category });
	}

	public useBrand(brand: BrandResponse | null | undefined): VehiclesCatalogueSelectFiltersBearer {
		return new VehiclesCatalogueSelectFiltersBearer({ ...this._model, brand });
	}

	public useModel(model: ModelResponse | null | undefined): VehiclesCatalogueSelectFiltersBearer {
		return new VehiclesCatalogueSelectFiltersBearer({ ...this._model, model });
	}

	public useLocation(location: LocationResponse | null | undefined): VehiclesCatalogueSelectFiltersBearer {
		return new VehiclesCatalogueSelectFiltersBearer({ ...this._model, location });
	}

	public static default(): VehiclesCatalogueSelectFiltersBearer {
		return new VehiclesCatalogueSelectFiltersBearer({
			category: undefined,
			brand: undefined,
			model: undefined,
			location: undefined,
		});
	}
}

@Component({
	selector: 'app-vehicles-page',
	imports: [
		FormsModule,
		VehiclesTextSearchComponent,
		VehiclesPriceSortComponent,
		VehiclePriceFilterFormPartComponent,
		VehicleCategoryFilterFormPartComponent,
		VehicleBrandFilterFormPartComponent,
		VehicleModelFilterFormPartComponent,
		VehicleRegionsFilterFormPartComponent,
		VehicleCardComponent,
		PaginationComponent,
	],
	templateUrl: './vehicles-page.component.html',
	styleUrl: './vehicles-page.component.scss',
})
export class VehiclesPageComponent {
	readonly activatedRouteParams: WritableSignal<ActivatedVehicleRouteParams> = signal(defaultActivatedVehicleRouteParams());
	readonly vehicles: WritableSignal<CatalogueVehicle[]> = signal([]);
	readonly categories: WritableSignal<CategoryResponse[]> = signal([]);
	readonly brands: WritableSignal<BrandResponse[]> = signal([]);
	readonly models: WritableSignal<ModelResponse[]> = signal([]);
	readonly locations: WritableSignal<LocationResponse[]> = signal([]);
	readonly totalAmount: WritableSignal<number> = signal(0);
	readonly destroyRef: DestroyRef = inject(DestroyRef);
	readonly vehiclesQuery: WritableSignal<GetVehiclesQuery> = signal(GetVehiclesQuery.default());
	readonly locationsQuery: WritableSignal<GetLocationsQueryParameters> = signal(
		LocationsQueryWithOrderByName(true, DefaultLocationQuery()),
	);

	readonly selectFiltersBearer: WritableSignal<VehiclesCatalogueSelectFiltersBearer> = signal(
		VehiclesCatalogueSelectFiltersBearer.default(),
	);

	readonly itemsPerPage: number = 20;
	readonly aggregatedStatistics: WritableSignal<AggregatedStatisticsInfo> = signal(defaultAggregatedStatisticsInfo());
	private readonly _vehiclesService: VehiclesApiService = inject(VehiclesApiService);
	private readonly _categoriesService: CategoriesApiService = inject(CategoriesApiService);
	private readonly _brandsService: BrandsApiService = inject(BrandsApiService);
	private readonly _modelsService: ModelsApiService = inject(ModelsApiService);
	private readonly _activatedRoute: ActivatedRoute = inject(ActivatedRoute);
	private readonly _locationsService: LocationsApiService = inject(LocationsApiService);

	readonly routeActivatedEffect = effect(() => {
		this.initializeSelectBearerFromActivatedRoute();
	});

	readonly fetchVehiclesOnQueryChangeEffect = effect(() => {
		const query: GetVehiclesQuery = this.vehiclesQuery();
		this.fetchVehicles(query);
	});

	readonly locationsFilterProps: Signal<VehicleRegionsFilterFormPartProps> = computed((): VehicleRegionsFilterFormPartProps => {
		return { locations: this.locations(), selectedLocation: this.activatedRouteParams().location };
	});

	readonly fetchLocationsOnQueryChangeEffect = effect(() => {
		const query: GetLocationsQueryParameters = this.locationsQuery();
		this.fetchLocations(query);
	});

	public handleLocationFilterTyped($event: string | null | undefined): void {
		const current: GetLocationsQueryParameters = this.locationsQuery();
		this.updateLocationsQuery(current.CategoryId, current.BrandId, $event);
	}

	public acceptTextSearch($event: string | undefined): void {
		// const part: TextSearchPart = new TextSearchPart($event);
		// this._textSearchPart.set(part);
		// this.resetPage();
	}

	public acceptSortMode($event: string | undefined): void {
		// const part: SortModePart = new SortModePart($event);
		// this._sortModePart.set(part);
	}

	public acceptCategory($event: string | undefined): void {
		// const part: CategoryIdPart = new CategoryIdPart($event);
		// this._categoryIdPart.set(part);
		// this.resetPage();
	}

	public acceptModel($event: string | undefined): void {
		// const part: ModelIdPart = new ModelIdPart($event);
		// this._modelIdPart.set(part);
		// this.resetPage();
	}

	public acceptRegion($event: string | undefined): void {
		// const part: LocationIdPart = new LocationIdPart($event);
		// this._locationIdPart.set(part);
		// this.resetPage();
	}

	public acceptPrice($event: PriceSubmitEvent): void {
		// const part: PriceFilterPart = new PriceFilterPart(
		//   $event.priceFrom,
		//   $event.priceTo,
		// );
		// this._priceFilterPart.set(part);
		// this.resetPage();
	}

	public changePage($event: number): void {
		// const part: PaginationPart = new PaginationPart($event);
		// this._paginationPart.set(part);
		// window.scroll(0, 0);
	}

	public acceptBrand($event: string | undefined): void {
		// const part: BrandIdPart = new BrandIdPart($event);
		// this._brandIdPart.set(part);
		// this.resetPage();
	}

	private resetPage(): void {
		// const part: PaginationPart = new PaginationPart(1);
		// this._paginationPart.set(part);
	}

	private initializeSelectBearerFromActivatedRoute(): void {
		this._activatedRoute.queryParams.subscribe((params: Params) => {
			const page: number | null | undefined = extractPageFromQueryParams(params);
			const brand: BrandResponse | null | undefined = extractBrandFromQueryParams(params);
			const category: CategoryResponse | null | undefined = extractCategoryFromQueryParams(params);
			const selectsBearer: VehiclesCatalogueSelectFiltersBearer = this.selectFiltersBearer().useCategory(category).useBrand(brand);
			this.selectFiltersBearer.set(selectsBearer);
			this.vehiclesQuery.update(
				(state: GetVehiclesQuery): GetVehiclesQuery => state.usePage(page).useBrand(brand).useCategory(category),
			);
			this.locationsQuery.update((state: GetLocationsQueryParameters): GetLocationsQueryParameters => {
				return { ...state, BrandId: brand?.Id, CategoryId: category?.Id };
			});
		});
	}

	private updateLocationsQuery(
		categoryId: string | null | undefined = undefined,
		brandId: string | null | undefined = undefined,
		textSearch: string | null | undefined = undefined,
	): void {
		this.locationsQuery.update((state: GetLocationsQueryParameters): GetLocationsQueryParameters => {
			return { ...state, CategoryId: categoryId, BrandId: brandId, TextSearch: textSearch };
		});
	}

	private fetchBrands(query: GetBrandsQuery): void {
		this._brandsService
			.fetchBrands(query)
			.pipe(
				tap((response: BrandResponse[]): void => {
					this.brands.set(response);
				}),
				catchError(() => EMPTY),
			)
			.subscribe();
	}

	private fetchCategories(query: GetCategoriesQuery): void {
		this._categoriesService
			.fetchCategories(query)
			.pipe(
				tap((response: CategoryResponse[]): void => this.categories.set(response)),
				catchError(() => EMPTY),
			)
			.subscribe();
	}

	private fetchLocations(query: GetLocationsQueryParameters): void {
		this._locationsService
			.fetchLocations(query)
			.pipe(
				tap((response: LocationResponse[]): void => {
					this.locations.set(response);
				}),
				catchError(() => EMPTY),
			)
			.subscribe();
	}

	private fetchVehicles(query: GetVehiclesQuery): void {
		this._vehiclesService
			.fetchVehicles(query)
			.pipe(
				tap((response: GetVehiclesQueryResponse) => {
					this.aggregatedStatistics.update((old: AggregatedStatisticsInfo): AggregatedStatisticsInfo => {
						return {
							...old,
							totalVehiclesCount: response.TotalCount,
							averagePrice: response.AveragePrice,
							minimalPrice: response.MinimalPrice,
							maximalPrice: response.MaximalPrice,
						};
					});
				}),
				catchError(() => EMPTY),
			)
			.subscribe();
	}
}

const extractCategoryFromQueryParams: (params: Params) => CategoryResponse | null | undefined = (
	params: Params,
): CategoryResponse | null => {
	const categoryId: string | undefined = params['categoryId'];
	const categoryName: string | undefined = params['categoryName'];
	if (!categoryId || !categoryName) return null;
	return { Id: categoryId, Name: categoryName };
};

const extractPageFromQueryParams: (params: Params) => number | null | undefined = (params: Params): number | null => {
	const pageStr: string | undefined = params['page'];
	if (!pageStr) return null;
	const pageNum = Number(pageStr);
	if (Number.isNaN(pageNum) || pageNum < 1) return null;
	return pageNum;
};

const extractBrandFromQueryParams: (params: Params) => BrandResponse | null | undefined = (params: Params): BrandResponse | null => {
	const brandId: string | undefined = params['brandId'];
	const brandName: string | undefined = params['brandName'];
	if (!brandId || !brandName) return null;
	return { Id: brandId, Name: brandName };
};
