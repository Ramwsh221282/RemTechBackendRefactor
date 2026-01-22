import { Component, DestroyRef, effect, inject, signal, WritableSignal } from '@angular/core';
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
import { VehicleRegionsFilterFormPartComponent } from './components/vehicle-regions-filter-form-part/vehicle-regions-filter-form-part.component';
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
import { DefaultLocationQuery } from '../../shared/api/locations-module/locations.factories';

type ActivatedVehicleRouteParams = {
    category: CategoryResponse | null | undefined;
    brand: BrandResponse | null | undefined;
    model: ModelResponse | null | undefined;
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
    return { brand: undefined, category: undefined, model: undefined };
};

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
    readonly locationsQuery: WritableSignal<GetLocationsQueryParameters> = signal(DefaultLocationQuery());

    readonly itemsPerPage: number = 20;
    readonly aggregatedStatistics: WritableSignal<AggregatedStatisticsInfo> = signal(defaultAggregatedStatisticsInfo());
    private readonly _service: VehiclesApiService = inject(VehiclesApiService);
    private readonly _activatedRoute: ActivatedRoute = inject(ActivatedRoute);
    private readonly _locationsService: LocationsApiService = inject(LocationsApiService);

    readonly routeActivatedEffect = effect(() => {
        this.readVehicleQueryParametersFromActivatedRouteParameters();
    });

    readonly fetchVehiclesOnQueryChangeEffect = effect(() => {
        const query: GetVehiclesQueryParameters = this.vehiclesQuery();
        this.fetchVehicles(query);
    });

    readonly fetchLocationsOnQueryChangeEffect = effect(() => {
        const query: GetLocationsQueryParameters = this.locationsQuery();
        this.fetchLocations(query);
    });

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

    private readVehicleQueryParametersFromActivatedRouteParameters(): void {
        this._activatedRoute.queryParams.subscribe((params: Params) => {
            const category: CategoryResponse | null = extractCategoryFromQueryParams(params);
            const brand: BrandResponse | null = extractBrandFromQueryParams(params);
            const page: number | null = extractPageFromQueryParams(params);
            this.updateActivatedVehicleRouteParams(category, brand);
            this.updateVehiclesQuery(page, category, brand);
            this.updateLocationsQuery(category, brand);
        });
    }

    private updateActivatedVehicleRouteParams(category: CategoryResponse | null, brand: BrandResponse | null): void {
        this.activatedRouteParams.update((state: ActivatedVehicleRouteParams) => {
            return { ...state, brand, category };
        });
    }

    private updateLocationsQuery(category: CategoryResponse | null, brand: BrandResponse | null): void {
        this.locationsQuery.update((state: GetLocationsQueryParameters): GetLocationsQueryParameters => {
            return { ...state, CategoryId: category?.Id, BrandId: brand?.Id };
        });
    }

    private updateVehiclesQuery(
        page: number | null,
        category: CategoryResponse | null,
        brand: BrandResponse | null,
    ): void {
        this.vehiclesQuery.update((state: GetVehiclesQueryParameters) => {
            return {
                ...state,
                Page: page,
                CategoryId: category?.Id,
                BrandId: brand?.Id,
            };
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

const extractCategoryFromQueryParams: (params: Params) => CategoryResponse | null = (
    params: Params,
): CategoryResponse | null => {
    const categoryId: string | undefined = params['categoryId'];
    const categoryName: string | undefined = params['categoryName'];
    if (!categoryId || !categoryName) return null;
    return { Id: categoryId, Name: categoryName };
};

const extractPageFromQueryParams: (params: Params) => number | null = (params: Params): number | null => {
    const pageStr: string | undefined = params['page'];
    if (!pageStr) return null;
    const pageNum = Number(pageStr);
    if (Number.isNaN(pageNum) || pageNum < 1) return null;
    return pageNum;
};

const extractBrandFromQueryParams: (params: Params) => BrandResponse | null = (
    params: Params,
): BrandResponse | null => {
    const brandId: string | undefined = params['brandId'];
    const brandName: string | undefined = params['brandName'];
    if (!brandId || !brandName) return null;
    return { Id: brandId, Name: brandName };
};
