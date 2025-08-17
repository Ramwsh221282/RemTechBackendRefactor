import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { CatalogueVehicle } from './types/CatalogueVehicle';
import { QueryPipeLineParamsFactory } from './queries/QueryParamsPipeline/QueryPipeLineParamsFactory';
import { CatalogueVehiclesService } from './services/CatalogueVehiclesService';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TextSearchPart } from './queries/QueryParamsPipeline/TextSearchPart';
import { BrandIdPart } from './queries/QueryParamsPipeline/BrandIdPart';
import { CategoryIdPart } from './queries/QueryParamsPipeline/CategoryIdPart';
import { PaginationPart } from './queries/QueryParamsPipeline/PaginationPart';
import { FormsModule } from '@angular/forms';
import { NgForOf, NgIf } from '@angular/common';
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
import { LocationIdPart } from './queries/QueryParamsPipeline/LocationIdPart';
import { ModelIdPart } from './queries/QueryParamsPipeline/ModelIdPart';
import { SortModePart } from './queries/QueryParamsPipeline/SortModePart';
import { PriceFilterPart } from './queries/QueryParamsPipeline/PriceFilterPart';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-vehicles-page',
  imports: [
    FormsModule,
    NgForOf,
    VehiclesTextSearchComponent,
    VehiclesPriceSortComponent,
    VehiclePriceFilterFormPartComponent,
    VehicleCategoryFilterFormPartComponent,
    VehicleBrandFilterFormPartComponent,
    VehicleModelFilterFormPartComponent,
    VehicleRegionsFilterFormPartComponent,
    VehicleCardComponent,
    NgIf,
    PaginationComponent,
  ],
  templateUrl: './vehicles-page.component.html',
  styleUrl: './vehicles-page.component.scss',
})
export class VehiclesPageComponent {
  private readonly _currentCategoryId: WritableSignal<string | undefined> =
    signal(undefined);
  private readonly _currentBrandId: WritableSignal<string | undefined> =
    signal(undefined);
  private readonly _currentModelId: WritableSignal<string | undefined> =
    signal(undefined);
  private readonly _brandIdPart: WritableSignal<BrandIdPart>;
  private readonly _categoryIdPart: WritableSignal<CategoryIdPart>;
  private readonly _locationIdPart: WritableSignal<LocationIdPart>;
  private readonly _modelIdPart: WritableSignal<ModelIdPart>;
  private readonly _paginationPart: WritableSignal<PaginationPart>;
  private readonly _sortModePart: WritableSignal<SortModePart>;
  private readonly _textSearchPart: WritableSignal<TextSearchPart>;
  private readonly _vehicles: WritableSignal<CatalogueVehicle[]>;
  private readonly _priceFilterPart: WritableSignal<PriceFilterPart>;
  private readonly _totalAmount: WritableSignal<number>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    service: CatalogueVehiclesService,
    activatedRoute: ActivatedRoute,
  ) {
    this._priceFilterPart = signal(new PriceFilterPart(null, null));
    this._vehicles = signal([]);
    this._brandIdPart = signal(new BrandIdPart(undefined));
    this._categoryIdPart = signal(new CategoryIdPart(undefined));
    this._locationIdPart = signal(new LocationIdPart(undefined));
    this._modelIdPart = signal(new ModelIdPart(undefined));
    this._paginationPart = signal(new PaginationPart(1));
    this._sortModePart = signal(new SortModePart(undefined));
    this._textSearchPart = signal(new TextSearchPart(undefined));
    this._totalAmount = signal(0);
    effect(() => {
      activatedRoute.queryParams
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (params: any): void => {
            const brandId: string | undefined = params['brandId'];
            const categoryId: string | undefined = params['categoryId'];
            const textSearch: string | undefined = params['textSearch'];
            const modelId: string | undefined = params['modelId'];
            const page: number | undefined = params['page'];
            this._currentCategoryId.set(categoryId);
            this._currentModelId.set(modelId);
            this._currentBrandId.set(brandId);
            this._modelIdPart.set(new ModelIdPart(modelId));
            this._brandIdPart.set(new BrandIdPart(brandId));
            this._categoryIdPart.set(new CategoryIdPart(categoryId));
            this._textSearchPart.set(new TextSearchPart(textSearch));
            this._paginationPart.set(
              new PaginationPart(page === undefined ? 1 : page),
            );
          },
        });
    });
    effect(() => {
      const brandIdPart = this._brandIdPart();
      const categoryIdPart = this._categoryIdPart();
      const modelIdPart = this._modelIdPart();
      const regionIdPart = this._locationIdPart();
      const paginationPart = this._paginationPart();
      const sortPart = this._sortModePart();
      const textSearchPart = this._textSearchPart();
      const priceFilterPart = this._priceFilterPart();
      const factory = new QueryPipeLineParamsFactory([])
        .with(brandIdPart)
        .with(categoryIdPart)
        .with(modelIdPart)
        .with(regionIdPart)
        .with(paginationPart)
        .with(sortPart)
        .with(textSearchPart)
        .with(priceFilterPart);

      service
        .fetch(factory)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: CatalogueVehicle[]): void => {
            this._vehicles.set(data);
          },
        });
    });
    effect(() => {
      const brandIdPart = this._brandIdPart();
      const categoryIdPart = this._categoryIdPart();
      const modelIdPart = this._modelIdPart();
      const regionIdPart = this._locationIdPart();
      const textSearchPart = this._textSearchPart();
      const priceFilterPart = this._priceFilterPart();
      const factory = new QueryPipeLineParamsFactory([])
        .with(brandIdPart)
        .with(categoryIdPart)
        .with(modelIdPart)
        .with(regionIdPart)
        .with(textSearchPart)
        .with(priceFilterPart);

      service
        .count(factory)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: number): void => {
            this._totalAmount.set(data);
          },
        });
    });
  }

  public get currentPage(): number {
    return this._paginationPart().page;
  }

  public get totalAmount(): number {
    return this._totalAmount();
  }

  public get vehicles(): CatalogueVehicle[] {
    return this._vehicles();
  }

  public acceptTextSearch($event: string | undefined): void {
    const part: TextSearchPart = new TextSearchPart($event);
    this._textSearchPart.set(part);
    this.resetPage();
  }

  public acceptSortMode($event: string | undefined): void {
    const part: SortModePart = new SortModePart($event);
    this._sortModePart.set(part);
  }

  public acceptCategory($event: string | undefined): void {
    const part: CategoryIdPart = new CategoryIdPart($event);
    this._categoryIdPart.set(part);
    this.resetPage();
  }

  public acceptModel($event: string | undefined): void {
    const part: ModelIdPart = new ModelIdPart($event);
    this._modelIdPart.set(part);
    this.resetPage();
  }

  public acceptRegion($event: string | undefined): void {
    const part: LocationIdPart = new LocationIdPart($event);
    this._locationIdPart.set(part);
    this.resetPage();
  }

  public acceptPrice($event: PriceSubmitEvent): void {
    const part: PriceFilterPart = new PriceFilterPart(
      $event.priceFrom,
      $event.priceTo,
    );
    this._priceFilterPart.set(part);
    this.resetPage();
  }

  public changePage($event: number): void {
    const part: PaginationPart = new PaginationPart($event);
    this._paginationPart.set(part);
    window.scroll(0, 0);
  }

  public acceptBrand($event: string | undefined): void {
    const part: BrandIdPart = new BrandIdPart($event);
    this._brandIdPart.set(part);
    this.resetPage();
  }

  public get currentCategoryId(): string | undefined {
    return this._currentCategoryId();
  }

  public get currentBrandId(): string | undefined {
    return this._currentBrandId();
  }

  public get currentModelId(): string | undefined {
    return this._currentModelId();
  }

  private resetPage(): void {
    const part: PaginationPart = new PaginationPart(1);
    this._paginationPart.set(part);
  }
}
