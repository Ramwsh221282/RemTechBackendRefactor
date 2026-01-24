import { Component, computed, DestroyRef, effect, inject, OnInit, Signal, signal, WritableSignal } from '@angular/core';
import { CatalogueVehicle } from './types/CatalogueVehicle';
import { ActivatedRoute, ParamMap, Params } from '@angular/router';
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
import { catchError, EMPTY, forkJoin, Observable, tap } from 'rxjs';
import { GetVehiclesQueryResponse } from '../../shared/api/vehicles-module/vehicles-api.responses';
import { LocationsApiService } from '../../shared/api/locations-module/locations-api.service';
import { LocationResponse } from '../../shared/api/locations-module/locations.responses';
import { CategoriesApiService } from '../../shared/api/categories-module/categories-api.service';
import { BrandsApiService } from '../../shared/api/brands-module/brands-api.service';
import { ModelsApiService } from '../../shared/api/models-module/models-api.service';
import { GetCategoriesQuery } from '../../shared/api/categories-module/categories-get-query';
import { GetBrandsQuery } from '../../shared/api/brands-module/brands-get-query';
import { GetVehiclesQuery } from '../../shared/api/vehicles-module/vehicles-get-query';
import { GetLocationsQuery } from '../../shared/api/locations-module/locations-get-query';
import { HttpErrorResponse } from '@angular/common/http';
import { GetModelsQuery } from '../../shared/api/models-module/models-get-query';
import { Title } from '@angular/platform-browser';

export class VehiclesCatalogueSelectFiltersBearer {
	private constructor(private readonly _model: VehiclesCatalogueFilterSeletions) {}

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
export class VehiclesPageComponent implements OnInit {
	private readonly _title: Title = inject(Title);
	private readonly _activatedRoute: ActivatedRoute = inject(ActivatedRoute);
	private readonly _locationsService: LocationsApiService = inject(LocationsApiService);
	private readonly _categoriesService: CategoriesApiService = inject(CategoriesApiService);
	private readonly _brandsService: BrandsApiService = inject(BrandsApiService);
	private readonly _modelsService: ModelsApiService = inject(ModelsApiService);
	private readonly _vehiclesService: VehiclesApiService = inject(VehiclesApiService);

	readonly vehiclesPageSize: number = 30;
	readonly locations: WritableSignal<LocationResponse[]> = signal([]);
	readonly vehicles: WritableSignal<GetVehiclesQueryResponse> = signal(defaultVehiclesQueryResponse());
	readonly categories: WritableSignal<CategoryResponse[]> = signal([]);
	readonly brands: WritableSignal<BrandResponse[]> = signal([]);
	readonly models: WritableSignal<ModelResponse[]> = signal([]);
	readonly statisticsInfo: WritableSignal<AggregatedStatisticsInfo> = signal(defaultAggregatedStatisticsInfo());
	readonly selectFilterParameters: WritableSignal<VehiclesCatalogueFilterSeletions> = signal(defaultCatalogueFilterSelections());
	readonly vehiclesQuery: WritableSignal<GetVehiclesQuery> = signal(defaultVehiclesQuery());
	readonly locationsQuery: WritableSignal<GetLocationsQuery> = signal(defaultLocationsQuery());
	readonly brandsQuery: WritableSignal<GetBrandsQuery> = signal(GetBrandsQuery.default());
	readonly modelsQuery: WritableSignal<GetModelsQuery> = signal(GetModelsQuery.default());

	readonly fetchLocationsOnQueryChange = effect((): void => {
		const query: GetLocationsQuery = this.locationsQuery();
		this._locationsService
			.fetchLocations(query)
			.pipe(
				tap((locations: LocationResponse[]): void => this.locations.set(locations)),
				catchError((): Observable<never> => EMPTY),
			)
			.subscribe();
	});

	readonly updateBrandsQueryOnFiltersSelect = effect((): void => {
		const filterSelections: VehiclesCatalogueFilterSeletions = this.selectFilterParameters();
		this.brandsQuery.update((state: GetBrandsQuery): GetBrandsQuery => {
			return state.useCategoryId(filterSelections.category?.Id).useModelId(filterSelections.model?.Id);
		});
	});

	readonly updateModelsQueryOnFiltersSelect = effect((): void => {
		const filterSelections: VehiclesCatalogueFilterSeletions = this.selectFilterParameters();
		this.modelsQuery.update((state: GetModelsQuery): GetModelsQuery => {
			return state.useCategoryId(filterSelections.category?.Id).useBrandId(filterSelections.brand?.Id);
		});
	});

	readonly fetchBrandsOnQueryChange = effect((): void => {
		const query: GetBrandsQuery = this.brandsQuery();
		this._brandsService
			.fetchBrands(query)
			.pipe(
				tap((brands: BrandResponse[]): void => this.brands.set(brands)),
				catchError((): Observable<never> => EMPTY),
			)
			.subscribe();
	});

	readonly fetchModelsOnQueryChange = effect((): void => {
		const query: GetModelsQuery = this.modelsQuery();
		this._modelsService
			.fetchModels(query)
			.pipe(
				tap((models: ModelResponse[]): void => this.models.set(models)),
				catchError((): Observable<never> => EMPTY),
			)
			.subscribe();
	});

	readonly fetchVehiclesOnQueryChange = effect((): void => {
		const query: GetVehiclesQuery = this.vehiclesQuery();
		this._vehiclesService
			.fetchVehicles(query)
			.pipe(
				tap((response: GetVehiclesQueryResponse): void => this.vehicles.set(response)),
				catchError((): Observable<never> => EMPTY),
			)
			.subscribe();
	});

	public ngOnInit(): void {
		this.initializePageTitle();
		this.readParametersFromActivatedRoute(this._activatedRoute);
		this.fetchVehiclesCategoryBrandModelsOptions();
	}

	private initializePageTitle(): void {
		this._title.setTitle('Техника');
	}

	private fetchVehiclesCategoryBrandModelsOptions(): void {
		const category: CategoryResponse | null | undefined = this.selectFilterParameters().category;
		const brand: BrandResponse | null | undefined = this.selectFilterParameters().brand;
		const categoryFetch: Observable<CategoryResponse[]> = this._categoriesService.fetchCategories(GetCategoriesQuery.default());
		const brandFetch: Observable<BrandResponse[]> = this._brandsService.fetchBrands(
			GetBrandsQuery.default().useCategoryId(category?.Id),
		);
		const modelFetch: Observable<ModelResponse[]> = this._modelsService.fetchModels(
			GetModelsQuery.default().useCategoryId(category?.Id).useBrandId(brand?.Id),
		);
		forkJoin([categoryFetch, brandFetch, modelFetch])
			.pipe(
				tap((response: [CategoryResponse[], BrandResponse[], ModelResponse[]]) => {
					this.categories.set(response[0]);
					this.brands.set(response[1]);
					this.models.set(response[2]);
				}),
			)
			.subscribe();
	}

	private readParametersFromActivatedRoute(route: ActivatedRoute): void {
		route.queryParamMap.subscribe((params: ParamMap): void => {
			const categoryInfo: CategoryResponse | null | undefined = extractCategoryFromQueryParams(params);
			const brandInfo: BrandResponse | null | undefined = extractBrandFromQueryParams(params);
			const page: number | null | undefined = extractPageFromQueryParams(params);
			this.selectFilterParameters.update((state: VehiclesCatalogueFilterSeletions): VehiclesCatalogueFilterSeletions => {
				return { ...state, brand: brandInfo, category: categoryInfo };
			});
			this.vehiclesQuery.update((state: GetVehiclesQuery): GetVehiclesQuery => {
				return state.usePage(page).useCategory(categoryInfo, categoryInfo?.Id).useBrand(brandInfo, brandInfo?.Id);
			});
			this.locationsQuery.update((state: GetLocationsQuery): GetLocationsQuery => {
				return state.useCategoryId(categoryInfo?.Id).useBrandId(brandInfo?.Id);
			});
		});
	}

	public handleUserPriceFilterInput(userPriceSubmit: PriceSubmitEvent): void {
		this.vehiclesQuery.update((state: GetVehiclesQuery): GetVehiclesQuery => {
			return state.useMinimalPrice(userPriceSubmit.priceFrom).useMaximalPrice(userPriceSubmit.priceTo);
		});
	}

	public handleUserFiltersLocations(userInput: string | null | undefined): void {
		this.locationsQuery.update((state: GetLocationsQuery): GetLocationsQuery => {
			return state.useTextSearch(userInput);
		});
	}

	public handleUserVehiclesTextSearchSubmit(userInput: string | undefined): void {}

	public handleUserVehiclePriceSortModeChange(sortMode: string | undefined): void {}
}

type AggregatedStatisticsInfo = {
	totalVehiclesCount: number;
	averagePrice: number;
	minimalPrice: number;
	maximalPrice: number;
};

const defaultAggregatedStatisticsInfo: () => AggregatedStatisticsInfo = () => {
	return {
		totalVehiclesCount: 0,
		averagePrice: 0,
		minimalPrice: 0,
		maximalPrice: 0,
	};
};

const defaultVehiclesQuery: () => GetVehiclesQuery = () => {
	return GetVehiclesQuery.default().usePageSize(30).usePage(1);
};

const defaultLocationsQuery: () => GetLocationsQuery = () => {
	return GetLocationsQuery.default().useAmount(20);
};

const defaultCatalogueFilterSelections: () => VehiclesCatalogueFilterSeletions = () => {
	return { brand: undefined, category: undefined, model: undefined, location: undefined };
};

const defaultVehiclesQueryResponse: () => GetVehiclesQueryResponse = () => {
	return { Vehicles: [], TotalCount: 0, AveragePrice: 0, MaximalPrice: 0, MinimalPrice: 0 };
};

type VehiclesCatalogueFilterSeletions = {
	category: CategoryResponse | null | undefined;
	brand: BrandResponse | null | undefined;
	model: ModelResponse | null | undefined;
	location: LocationResponse | null | undefined;
};

const extractCategoryFromQueryParams: (params: ParamMap) => CategoryResponse | null | undefined = (
	params: ParamMap,
): CategoryResponse | null => {
	const id: string | null | undefined = params.get('categoryId');
	const name: string | null | undefined = params.get('categoryName');
	if (!id || !name) return null;
	return { Id: id, Name: name };
};

const extractPageFromQueryParams: (params: ParamMap) => number | null | undefined = (params: ParamMap): number | null => {
	const pageStr: string | null | undefined = params.get('page');
	if (!pageStr) return null;
	const pageNum: number = Number(pageStr);
	if (Number.isNaN(pageNum) || pageNum < 1) return null;
	return pageNum;
};

const extractBrandFromQueryParams: (params: ParamMap) => BrandResponse | null | undefined = (params: ParamMap): BrandResponse | null => {
	const brandId: string | null | undefined = params.get('brandId');
	const brandName: string | null | undefined = params.get('brandName');
	if (!brandId || !brandName) return null;
	return { Id: brandId, Name: brandName };
};
