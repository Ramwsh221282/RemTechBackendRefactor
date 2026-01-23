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
import { GetVehiclesQueryParameters } from '../../shared/api/vehicles-module/vehicles-api.requests';
import { DefaultGetVehiclesQueryParameters } from '../../shared/api/vehicles-module/vehicles-api.factories';
import { CategoryResponse } from '../../shared/api/categories-module/categories-responses';
import { BrandResponse } from '../../shared/api/brands-module/brands-api.responses';
import { ModelResponse } from '../../shared/api/models-module/models-responses';
import { catchError, EMPTY, tap } from 'rxjs';
import { GetVehiclesQueryResponse } from '../../shared/api/vehicles-module/vehicles-api.responses';
import { LocationsApiService } from '../../shared/api/locations-module/locations-api.service';
import { LocationResponse } from '../../shared/api/locations-module/locations.responses';
import { GetLocationsQueryParameters } from '../../shared/api/locations-module/locations.requests';
import {
    DefaultLocationQuery,
    LocationsQueryWithOrderByName,
} from '../../shared/api/locations-module/locations.factories';

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

export type VehiclesCatalogueVehicleFiltersBearerModel = {
    brandId: string | null | undefined;
    categoryId: string | null | undefined;
    regionId: string | null | undefined;
    modelId: string | null | undefined;
    isNds: boolean | null | undefined;
    minimalPrice: number | null | undefined;
    maximalPrice: number | null | undefined;
    sortFields: string[] | null | undefined;
    sort: string | null | undefined;
    page: number | null | undefined;
    pageSize: number | null | undefined;
    textSearch: string | null | undefined;
};

export class VehiclesCatalogueVehicleFiltersBearer {
    private constructor(private readonly _model: VehiclesCatalogueVehicleFiltersBearerModel) {}

    public useBrand(
        brand: BrandResponse | null | undefined = undefined,
        brandId: string | null | undefined = undefined,
    ): VehiclesCatalogueVehicleFiltersBearer {
        if (brand) return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, brandId: brand.Id });
        if (brandId) return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, brandId: brandId });
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, brandId: undefined });
    }

    public useCategory(
        category: CategoryResponse | null | undefined = undefined,
        categoryId: string | null | undefined = undefined,
    ) {
        if (category) return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, categoryId: category.Id });
        if (categoryId) return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, categoryId: categoryId });
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, categoryId: undefined });
    }

    public useLocation(
        location: LocationResponse | null | undefined = undefined,
        locationId: string | null | undefined = undefined,
    ) {
        if (location) return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, regionId: location.Id });
        if (locationId) return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, regionId: locationId });
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, regionId: undefined });
    }

    public convertToQueryParameters(): GetVehiclesQueryParameters {
        return {
            BrandId: this._model.brandId,
            CategoryId: this._model.categoryId,
            RegionId: this._model.regionId,
            ModelId: this._model.modelId,
            IsNds: this._model.isNds,
            MinimalPrice: this._model.minimalPrice,
            MaximalPrice: this._model.maximalPrice,
            SortFields: this._model.sortFields,
            Sort: this._model.sort,
            Page: this._model.page,
            PageSize: this._model.pageSize,
            TextSearch: this._model.textSearch,
        };
    }

    public useNds(isNds: boolean | null | undefined): VehiclesCatalogueVehicleFiltersBearer {
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, isNds });
    }

    public useMinimalPrice(minimalPrice: number | null | undefined): VehiclesCatalogueVehicleFiltersBearer {
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, minimalPrice });
    }

    public useMaximalPrice(maximalPrice: number | null | undefined): VehiclesCatalogueVehicleFiltersBearer {
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, maximalPrice });
    }

    public useSortFields(sortFields: string[] | null | undefined): VehiclesCatalogueVehicleFiltersBearer {
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, sortFields });
    }

    public useSort(sort: string | null | undefined): VehiclesCatalogueVehicleFiltersBearer {
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, sort });
    }

    public usePage(page: number | null | undefined): VehiclesCatalogueVehicleFiltersBearer {
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, page });
    }

    public usePageSize(pageSize: number | null | undefined): VehiclesCatalogueVehicleFiltersBearer {
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, pageSize });
    }

    public useTextSearch(textSearch: string | null | undefined): VehiclesCatalogueVehicleFiltersBearer {
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, textSearch });
    }

    public useModel(
        model: ModelResponse | null | undefined = undefined,
        modelId: string | null | undefined = undefined,
    ) {
        if (model) return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, modelId: model.Id });
        if (modelId) return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, modelId: modelId });
        return new VehiclesCatalogueVehicleFiltersBearer({ ...this._model, modelId: undefined });
    }

    public static default(): VehiclesCatalogueVehicleFiltersBearer {
        return new VehiclesCatalogueVehicleFiltersBearer({
            brandId: undefined,
            categoryId: undefined,
            regionId: undefined,
            modelId: undefined,
            isNds: undefined,
            minimalPrice: undefined,
            maximalPrice: undefined,
            sortFields: undefined,
            sort: undefined,
            page: undefined,
            pageSize: undefined,
            textSearch: undefined,
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
    readonly activatedRouteParams: WritableSignal<ActivatedVehicleRouteParams> = signal(
        defaultActivatedVehicleRouteParams(),
    );
    readonly vehicles: WritableSignal<CatalogueVehicle[]> = signal([]);
    readonly locations: WritableSignal<LocationResponse[]> = signal([]);
    readonly totalAmount: WritableSignal<number> = signal(0);
    readonly destroyRef: DestroyRef = inject(DestroyRef);
    readonly vehiclesQuery: WritableSignal<GetVehiclesQueryParameters> = signal(DefaultGetVehiclesQueryParameters());
    readonly locationsQuery: WritableSignal<GetLocationsQueryParameters> = signal(
        LocationsQueryWithOrderByName(true, DefaultLocationQuery()),
    );

    readonly selectFiltersBearer: WritableSignal<VehiclesCatalogueSelectFiltersBearer> = signal(
        VehiclesCatalogueSelectFiltersBearer.default(),
    );

    readonly vehiclesFiltersBearer: WritableSignal<VehiclesCatalogueVehicleFiltersBearer> = signal(
        VehiclesCatalogueVehicleFiltersBearer.default(),
    );

    readonly itemsPerPage: number = 20;
    readonly aggregatedStatistics: WritableSignal<AggregatedStatisticsInfo> = signal(defaultAggregatedStatisticsInfo());
    private readonly _service: VehiclesApiService = inject(VehiclesApiService);
    private readonly _activatedRoute: ActivatedRoute = inject(ActivatedRoute);
    private readonly _locationsService: LocationsApiService = inject(LocationsApiService);

    readonly routeActivatedEffect = effect(() => {
        this.initializeSelectBearerFromActivatedRoute();
    });

    readonly fetchVehiclesOnQueryChangeEffect = effect(() => {
        const query: GetVehiclesQueryParameters = this.vehiclesQuery();
        this.fetchVehicles(query);
    });

    readonly locationsFilterProps: Signal<VehicleRegionsFilterFormPartProps> = computed(
        (): VehicleRegionsFilterFormPartProps => {
            return { locations: this.locations(), selectedLocation: this.activatedRouteParams().location };
        },
    );

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
            const selectsBearer: VehiclesCatalogueSelectFiltersBearer = this.selectFiltersBearer()
                .useCategory(category)
                .useBrand(brand);
            const vehiclesBearer: VehiclesCatalogueVehicleFiltersBearer = this.vehiclesFiltersBearer()
                .useBrand(brand)
                .useCategory(category);
            this.selectFiltersBearer.set(selectsBearer);
            this.vehiclesFiltersBearer.set(vehiclesBearer);
            this.locationsQuery.update((state: GetLocationsQueryParameters): GetLocationsQueryParameters => {
                return { ...state, BrandId: brand?.Id, CategoryId: category?.Id };
            });
            this.vehiclesFiltersBearer.update((state: VehiclesCatalogueVehicleFiltersBearer) => {
                return state.usePage(page).useBrand(brand).useCategory(category);
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

    private fetchVehicles(query: GetVehiclesQueryParameters): void {
        this._service
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

const extractBrandFromQueryParams: (params: Params) => BrandResponse | null | undefined = (
    params: Params,
): BrandResponse | null => {
    const brandId: string | undefined = params['brandId'];
    const brandName: string | undefined = params['brandName'];
    if (!brandId || !brandName) return null;
    return { Id: brandId, Name: brandName };
};
