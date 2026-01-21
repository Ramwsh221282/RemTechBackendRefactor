import {
  Component,
  computed,
  effect,
  EventEmitter,
  Input,
  Output,
  signal,
  Signal,
  WritableSignal,
} from '@angular/core';
import { Select, SelectChangeEvent, SelectModule } from 'primeng/select';
import { tap, catchError, Observable, EMPTY } from 'rxjs';
import { BrandResponse } from '../../../../../shared/api/brands-module/brands-api.responses';
import { BrandsApiService } from '../../../../../shared/api/brands-module/brands-api.service';
import { CategoriesApiService } from '../../../../../shared/api/categories-module/categories-api.service';
import { CategoryResponse } from '../../../../../shared/api/categories-module/categories-responses';
import { ModelsApiService } from '../../../../../shared/api/models-module/models-api.service';
import { ModelResponse } from '../../../../../shared/api/models-module/models-responses';
import { DefaultGetVehiclesQueryParameters } from '../../../../../shared/api/vehicles-module/vehicles-api.factories';
import { GetVehiclesQueryParameters } from '../../../../../shared/api/vehicles-module/vehicles-api.requests';
import {
  VehicleResponse,
  GetVehiclesQueryResponse,
} from '../../../../../shared/api/vehicles-module/vehicles-api.responses';
import { VehiclesApiService } from '../../../../../shared/api/vehicles-module/vehicles-api.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-fast-navigation-vehicles',
  imports: [Select, SelectModule, FormsModule],
  templateUrl: './fast-navigation-vehicles.component.html',
  styleUrl: './fast-navigation-vehicles.component.css',
})
export class FastNavigationVehiclesComponent {
  constructor(
    private readonly _categoriesService: CategoriesApiService,
    private readonly _brandsService: BrandsApiService,
    private readonly _modelsService: ModelsApiService,
    private readonly _vehiclesService: VehiclesApiService,
  ) {
    this.brands = signal([]);
    this.models = signal([]);
    this.categories = signal([]);
    this.vehiclesFetched = new EventEmitter<{
      vehicles: VehicleResponse[];
      totalCount: number;
    }>();
    this.brandSelected = new EventEmitter<BrandResponse | undefined>();
    this.categorySelected = new EventEmitter<CategoryResponse | undefined>();
    this.modelSelected = new EventEmitter<ModelResponse | undefined>();

    this.vehiclesFetchProps = signal({
      page: undefined,
      brand: undefined,
      category: undefined,
      model: undefined,
      isWatching: false,
      pageSize: undefined,
    });

    this.itemTypes = ['Техника', 'Запчасти'];
    this.fetchCategoriesOnItemTypeChange();
    this.fetchBrandsOnCategoryChange();
    this.fetchModelsOnBrandChange();
    this.fetchVehiclesOnQueryChange();
  }

  readonly vehiclesFetchProps: WritableSignal<{
    page?: number | undefined;
    pageSize?: number | undefined;
    isWatching: boolean;
    category?: CategoryResponse | undefined;
    brand?: BrandResponse | undefined;
    model?: ModelResponse | undefined;
    textSearch?: string | undefined;
  }>;

  @Input({ required: true }) set vehicles_fetch_props(value: {
    page?: number | undefined;
    pageSize?: number | undefined;
    isWatching: boolean;
    category?: CategoryResponse | undefined;
    brand?: BrandResponse | undefined;
    model?: ModelResponse | undefined;
    textSearch?: string | undefined;
  }) {
    this.vehiclesFetchProps.set(value);
  }

  @Output() brandSelected: EventEmitter<BrandResponse | undefined>;
  @Output() categorySelected: EventEmitter<CategoryResponse | undefined>;
  @Output() modelSelected: EventEmitter<ModelResponse | undefined>;

  @Output() vehiclesFetched: EventEmitter<{
    vehicles: VehicleResponse[];
    totalCount: number;
  }>;

  readonly itemTypes: string[];
  readonly categories: WritableSignal<CategoryResponse[]>;
  readonly brands: WritableSignal<BrandResponse[]>;
  readonly models: WritableSignal<ModelResponse[]>;

  readonly readSelectedItemType: Signal<string> = computed((): string => {
    const vehicleProps = this.vehiclesFetchProps();
    return vehicleProps.isWatching ? 'Техника' : 'Запчасти';
  });

  readonly userIsWatchingVehicles: Signal<boolean> = computed((): boolean => {
    return this.vehiclesFetchProps().isWatching;
  });

  readonly userSelectedCategory: Signal<boolean> = computed((): boolean => {
    return !!this.vehiclesFetchProps().category;
  });

  readonly userSelectedBrand: Signal<boolean> = computed((): boolean => {
    return !!this.vehiclesFetchProps().brand;
  });

  readonly userSelectedModel: Signal<boolean> = computed((): boolean => {
    return !!this.vehiclesFetchProps().model;
  });

  public navigateVehicles(): void {
    // const brand: CatalogueBrand | undefined = this.selectedBrand();
    // const model: CatalogueModel | undefined = this.selectedModel();
    // const type: CatalogueCategory | undefined = this.selectedType();
    // if (brand && model && type) {
    //   this._router.navigate(['vehicles'], {
    //     queryParams: {
    //       brandId: brand.id,
    //       categoryId: type.id,
    //       modelId: model.id,
    //       page: 1,
    //     },
    //   });
    // }
  }

  public handleUserCategoryClear(): void {
    this.categorySelected.emit(undefined);
  }

  public handleUserCategoryChange($event: SelectChangeEvent): void {
    const selectedCategory: CategoryResponse = $event.value as CategoryResponse;
    this.categorySelected.emit(selectedCategory);
  }

  public handleUserBrandChange($event: SelectChangeEvent): void {
    const selectedBrand: BrandResponse = $event.value as BrandResponse;
    this.brandSelected.emit(selectedBrand);
  }

  public handleUserBrandClear(): void {
    this.brandSelected.emit(undefined);
  }

  public handleUserModelChange($event: SelectChangeEvent): void {
    const selectedModel: ModelResponse = $event.value as ModelResponse;
    this.modelSelected.emit(selectedModel);
  }

  public handleUserModelClear(): void {
    this.modelSelected.emit(undefined);
  }

  private fetchBrandsOnCategoryChange(): void {
    effect(() => {
      const props = this.vehiclesFetchProps();
      if (props.category) {
        this._brandsService
          .fetchBrands(
            null,
            null,
            props.category.Id,
            props.category.Name,
            props.model?.Id,
            props.model?.Name,
          )
          .pipe(
            tap((brands: BrandResponse[]): void => this.brands.set(brands)),
            catchError((): Observable<never> => EMPTY),
          )
          .subscribe();
      }
    });
  }

  private fetchCategoriesOnItemTypeChange(): void {
    effect(() => {
      const props = this.vehiclesFetchProps();
      if (!props.isWatching) return;
      this._categoriesService
        .fetchCategories()
        .pipe(
          tap((categories: CategoryResponse[]): void =>
            this.categories.set(categories),
          ),
          catchError((): Observable<never> => EMPTY),
        )
        .subscribe();
    });
  }

  private fetchModelsOnBrandChange(): void {
    effect(() => {
      const props = this.vehiclesFetchProps();
      if (!props.brand) return;
      this._modelsService
        .fetchModels(
          null,
          null,
          props.brand.Id,
          props.brand.Name,
          props.category?.Id,
          props.category?.Name,
        )
        .pipe(
          tap((models: ModelResponse[]): void => this.models.set(models)),
          catchError((): Observable<never> => EMPTY),
        )
        .subscribe();
    });
  }

  private fetchVehiclesOnQueryChange(): void {
    effect(() => {
      const props: {
        page?: number | undefined;
        pageSize?: number | undefined;
        isWatching: boolean;
        category?: CategoryResponse | undefined;
        brand?: BrandResponse | undefined;
        model?: ModelResponse | undefined;
      } = this.vehiclesFetchProps();
      if (!props.isWatching) return;
      if (!props.category || !props.brand || !props.model) return;
      const query: GetVehiclesQueryParameters = {
        ...DefaultGetVehiclesQueryParameters(),
        Page: props.page,
        PageSize: props.pageSize,
        BrandId: props.brand?.Id,
        CategoryId: props.category?.Id,
        ModelId: props.model?.Id,
      };
      this.invokeFetchVehicles(query);
    });
  }

  private invokeFetchVehicles(query: GetVehiclesQueryParameters): void {
    this._vehiclesService
      .fetchVehicles(query)
      .pipe(
        tap((response: GetVehiclesQueryResponse): void => {
          this.vehiclesFetched.emit({
            vehicles: response.Vehicles,
            totalCount: response.TotalCount,
          });
        }),
        catchError((): Observable<never> => EMPTY),
      )
      .subscribe();
  }
}
