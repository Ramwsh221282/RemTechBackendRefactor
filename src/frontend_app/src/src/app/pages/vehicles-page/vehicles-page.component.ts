import { Component, effect, EffectRef, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { VehiclesTextSearchComponent } from './components/vehicles-text-search/vehicles-text-search.component';
import { VehiclesPriceSortComponent } from './components/vehicles-price-sort/vehicles-price-sort.component';
import {
	PriceChangeEvent,
	VehiclePriceFilterFormPartComponent,
} from './components/vehicle-price-filter-form-part/vehicle-price-filter-form-part.component';
import { VehicleCategoryFilterFormPartComponent } from './components/vehicle-category-filter-form-part/vehicle-category-filter-form-part.component';
import { VehicleBrandFilterFormPartComponent } from './components/vehicle-brand-filter-form-part/vehicle-brand-filter-form-part.component';
import { VehicleModelFilterFormPartComponent } from './components/vehicle-model-filter-form-part/vehicle-model-filter-form-part.component';
import { VehicleRegionsFilterFormPartComponent } from './components/vehicle-regions-filter-form-part/vehicle-regions-filter-form-part.component';
import { VehicleCardComponent } from './components/vehicle-card/vehicle-card.component';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { VehiclesApiService } from '../../shared/api/vehicles-module/vehicles-api.service';
import { CategoryResponse } from '../../shared/api/categories-module/categories-responses';
import { BrandResponse } from '../../shared/api/brands-module/brands-api.responses';
import { ModelResponse } from '../../shared/api/models-module/models-responses';
import { catchError, EMPTY, forkJoin, Observable, of, tap } from 'rxjs';
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
import { GetModelsQuery } from '../../shared/api/models-module/models-get-query';
import { Title } from '@angular/platform-browser';

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
	readonly queries: WritableSignal<GroupedVehicleCatalogueQueries> = signal(defaultVehicleCatalogueQueries());

	readonly refreshCatalogueDataOnQueryChange: EffectRef = effect((): void => {
		const queries: GroupedVehicleCatalogueQueries = this.queries();
		const brandsFetch: Observable<BrandResponse[]> = this._brandsService.fetchBrands(queries.brandQuery);
		const modelsFetch: Observable<ModelResponse[]> = this._modelsService.fetchModels(queries.modelQuery);
		const regionsFetch: Observable<LocationResponse[]> = this._locationsService.fetchLocations(queries.locationsQuery);
		const vehiclesFetch: Observable<GetVehiclesQueryResponse> = this._vehiclesService.fetchVehicles(queries.vehiclesQuery);
		forkJoin([brandsFetch, modelsFetch, regionsFetch, vehiclesFetch])
			.pipe(
				tap((response: [BrandResponse[], ModelResponse[], LocationResponse[], GetVehiclesQueryResponse]) => {
					const brands: BrandResponse[] = response[0];
					const models: ModelResponse[] = response[1];
					const locations: LocationResponse[] = response[2];
					const vehiclesResponse: GetVehiclesQueryResponse = response[3];
					this.brands.set(brands);
					this.models.set(models);
					this.models.set(models);
					this.locations.set(locations);
					this.vehicles.set(vehiclesResponse);
				}),
				catchError(() => EMPTY),
			)
			.subscribe();
	});

	public ngOnInit(): void {
		this.initializePageTitle();
		this.readParametersFromActivatedRoute(this._activatedRoute);
		this.queries.update((state: GroupedVehicleCatalogueQueries) => {
			return { ...state, vehiclesQuery: state.vehiclesQuery.usePageSize(this.vehiclesPageSize) };
		});
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
			this.queries.update((state: GroupedVehicleCatalogueQueries): GroupedVehicleCatalogueQueries => {
				return {
					...state,
					vehiclesQuery: state.vehiclesQuery
						.usePageSize(this.vehiclesPageSize)
						.usePage(page ?? 1)
						.useCategory(categoryInfo, categoryInfo?.Id)
						.useBrand(brandInfo, brandInfo?.Id),
				};
			});
		});
	}

	public handleUserPriceFilterInput(userPriceSubmit: PriceChangeEvent): void {
		this.queries.update((state: GroupedVehicleCatalogueQueries): GroupedVehicleCatalogueQueries => {
			return {
				...state,
				vehiclesQuery: state.vehiclesQuery
					.useMinimalPrice(userPriceSubmit.minimalPrice)
					.useMaximalPrice(userPriceSubmit.maximalPrice),
			};
		});
	}

	public handleUserVehiclesTextSearchSubmit(userInput: string | undefined): void {}

	public handleUserVehiclePriceSortModeChange(sortMode: string | undefined): void {}

	public handleUserCategorySelect(category: CategoryResponse | null | undefined): void {
		this.selectFilterParameters.update((state: VehiclesCatalogueFilterSeletions): VehiclesCatalogueFilterSeletions => {
			return { ...state, category, brand: undefined, model: undefined, location: undefined };
		});

		this.queries.update((state: GroupedVehicleCatalogueQueries): GroupedVehicleCatalogueQueries => {
			return {
				...state,
				brandQuery: state.brandQuery.useCategoryId(category?.Id, category),
				modelQuery: GetModelsQuery.default().useCategoryId(category?.Id, category),
				locationsQuery: defaultLocationsQuery().useCategoryId(category?.Id, category),
				vehiclesQuery: state.vehiclesQuery.useCategory(category, category?.Id).useBrand(null).useModel(null).useLocation(null),
			};
		});
	}

	public handleUserBrandSelect(brand: BrandResponse | null | undefined): void {
		this.selectFilterParameters.update((state: VehiclesCatalogueFilterSeletions): VehiclesCatalogueFilterSeletions => {
			return { ...state, brand, model: undefined };
		});
		this.queries.update((state: GroupedVehicleCatalogueQueries): GroupedVehicleCatalogueQueries => {
			return {
				...state,
				vehiclesQuery: state.vehiclesQuery.useBrand(brand).useModel(null),
				modelQuery: state.modelQuery.useBrandId(brand?.Id).useBrandName(brand?.Name),
				locationsQuery: state.locationsQuery.useBrandId(brand?.Id, brand).useModelId(null).useModelName(null),
			};
		});
	}

	public handleUserModelSelect(model: ModelResponse | null | undefined): void {
		this.selectFilterParameters.update((state: VehiclesCatalogueFilterSeletions): VehiclesCatalogueFilterSeletions => {
			return { ...state, model };
		});
		this.queries.update((state: GroupedVehicleCatalogueQueries): GroupedVehicleCatalogueQueries => {
			return {
				...state,
				locationsQuery: state.locationsQuery.useModelId(model?.Id, model),
				vehiclesQuery: state.vehiclesQuery.useModel(model),
			};
		});
	}

	public handleUserLocationSelect(location: LocationResponse | null | undefined): void {
		this.selectFilterParameters.update((state: VehiclesCatalogueFilterSeletions): VehiclesCatalogueFilterSeletions => {
			return { ...state, location };
		});
		this.queries.update((state: GroupedVehicleCatalogueQueries): GroupedVehicleCatalogueQueries => {
			return {
				...state,
				vehiclesQuery: state.vehiclesQuery.useLocation(location, location?.Id),
			};
		});
	}
}

type GroupedVehicleCatalogueQueries = {
	brandQuery: GetBrandsQuery;
	modelQuery: GetModelsQuery;
	vehiclesQuery: GetVehiclesQuery;
	locationsQuery: GetLocationsQuery;
};

function defaultVehicleCatalogueQueries(): GroupedVehicleCatalogueQueries {
	return {
		brandQuery: GetBrandsQuery.default(),
		modelQuery: GetModelsQuery.default(),
		vehiclesQuery: defaultVehiclesQuery(),
		locationsQuery: defaultLocationsQuery(),
	};
}

type AggregatedStatisticsInfo = {
	totalVehiclesCount: number;
	averagePrice: number;
	minimalPrice: number;
	maximalPrice: number;
};

function defaultAggregatedStatisticsInfo(): AggregatedStatisticsInfo {
	return {
		totalVehiclesCount: 0,
		averagePrice: 0,
		minimalPrice: 0,
		maximalPrice: 0,
	};
}

function defaultVehiclesQuery(): GetVehiclesQuery {
	return GetVehiclesQuery.default().usePageSize(30).usePage(1);
}

function defaultLocationsQuery(): GetLocationsQuery {
	return GetLocationsQuery.default().useAmount(20);
}

function defaultCatalogueFilterSelections(): VehiclesCatalogueFilterSeletions {
	return { brand: undefined, category: undefined, model: undefined, location: undefined };
}

function defaultVehiclesQueryResponse(): GetVehiclesQueryResponse {
	return { Vehicles: [], TotalCount: 0, AveragePrice: 0, MaximalPrice: 0, MinimalPrice: 0 };
}

type VehiclesCatalogueFilterSeletions = {
	category: CategoryResponse | null | undefined;
	brand: BrandResponse | null | undefined;
	model: ModelResponse | null | undefined;
	location: LocationResponse | null | undefined;
};

function extractCategoryFromQueryParams(params: ParamMap): CategoryResponse | null | undefined {
	const id: string | null | undefined = params.get('categoryId');
	const name: string | null | undefined = params.get('categoryName');
	if (!id || !name) return null;
	return { Id: id, Name: name };
}

function extractPageFromQueryParams(params: ParamMap): number | null | undefined {
	const pageStr: string | null | undefined = params.get('page');
	if (!pageStr) return null;
	const pageNum: number = Number(pageStr);
	if (Number.isNaN(pageNum) || pageNum < 1) return null;
	return pageNum;
}

function extractBrandFromQueryParams(params: ParamMap): BrandResponse | null | undefined {
	const brandId: string | null | undefined = params.get('brandId');
	const brandName: string | null | undefined = params.get('brandName');
	if (!brandId || !brandName) return null;
	return { Id: brandId, Name: brandName };
}
