import {
  Component,
  computed,
  effect,
  EventEmitter,
  Output,
  Signal,
  signal,
  WritableSignal,
} from '@angular/core';
import { Select, SelectChangeEvent } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { InputText } from 'primeng/inputtext';
import { BrandsApiService } from '../../../../shared/api/brands-module/brands-api.service';
import { CategoriesApiService } from '../../../../shared/api/categories-module/categories-api.service';
import { catchError, EMPTY, Observable, tap } from 'rxjs';
import { CategoryResponse } from '../../../../shared/api/categories-module/categories-responses';
import { BrandResponse } from '../../../../shared/api/brands-module/brands-api.responses';
import { ModelResponse } from '../../../../shared/api/models-module/models-responses';
import { ModelsApiService } from '../../../../shared/api/models-module/models-api.service';
import { VehiclesApiService } from '../../../../shared/api/vehicles-module/vehicles-api.service';
import { DefaultGetVehiclesQueryParameters } from '../../../../shared/api/vehicles-module/vehicles-api.factories';
import { GetVehiclesQueryParameters } from '../../../../shared/api/vehicles-module/vehicles-api.requests';
import {
  GetVehiclesQueryResponse,
  VehicleResponse,
} from '../../../../shared/api/vehicles-module/vehicles-api.responses';
import { SparesApiService } from '../../../../shared/api/spares-module/spares-api.service';
import { SpareResponse } from '../../../../shared/api/spares-module/spares-api.responses';
import { GetSparesQueryParameters } from '../../../../shared/api/spares-module/spares-api.requests';

@Component({
  selector: 'app-fast-navigation',
  imports: [Select, FormsModule, InputText],
  templateUrl: './fast-navigation.component.html',
  styleUrl: './fast-navigation.component.scss',
})
export class FastNavigationComponent {
  constructor(
    private readonly _categoriesService: CategoriesApiService,
    private readonly _brandsService: BrandsApiService,
    private readonly _modelsService: ModelsApiService,
    private readonly _vehiclesService: VehiclesApiService,
    private readonly _sparesService: SparesApiService,
    private readonly _router: Router,
  ) {
    this.brands = signal([]);
    this.models = signal([]);
    this.categories = signal([]);
    this.selectedBrand = signal(undefined);
    this.selectedModel = signal(undefined);
    this.selectedCategory = signal(undefined);
    this.selectedItemType = signal(undefined);
    this.sparesFetched = new EventEmitter<SpareResponse[]>();
    this.watchingChanged = new EventEmitter<{
      vehicles: boolean;
      spares: boolean;
    }>();
    this.query = signal({
      ...DefaultGetVehiclesQueryParameters(),
      Page: 1,
      PageSize: 10,
    });
    this.sparesQuery = signal({ Page: 1, PageSize: 10 });
    this.itemTypes = signal(['Техника', 'Запчасти']);
    this.fetchCategoriesOnItemTypeChange();
    this.fetchBrandsOnCategoryChange();
    this.fetchModelsOnBrandChange();
    this.fetchVehiclesOnQueryChange();
    this.fetchSparesIfItemTypeIsSpare();
    this.vehiclesFetched = new EventEmitter<VehicleResponse[]>();
  }

  private readonly query: WritableSignal<GetVehiclesQueryParameters>;
  private readonly sparesQuery: WritableSignal<GetSparesQueryParameters>;

  @Output() vehiclesFetched: EventEmitter<VehicleResponse[]>;
  @Output() sparesFetched: EventEmitter<SpareResponse[]>;
  @Output() watchingChanged: EventEmitter<{
    vehicles: boolean;
    spares: boolean;
  }>;

  readonly itemTypes: WritableSignal<string[]>;
  readonly selectedItemType: WritableSignal<string | undefined>;

  readonly itemTypeIsVehicle: Signal<boolean> = computed(() => {
    const selected: string | undefined = this.selectedItemType();
    return !!selected && selected === 'Техника';
  });

  readonly itemTypeIsSpare: Signal<boolean> = computed(() => {
    const selected: string | undefined = this.selectedItemType();
    return !!selected && selected === 'Запчасти';
  });

  readonly categories: WritableSignal<CategoryResponse[]>;
  readonly selectedCategory: WritableSignal<CategoryResponse | undefined>;
  readonly categoryIsSelected: Signal<boolean> = computed(() => {
    const selected: CategoryResponse | undefined = this.selectedCategory();
    return selected !== undefined;
  });

  readonly brands: WritableSignal<BrandResponse[]>;
  readonly selectedBrand: WritableSignal<BrandResponse | undefined>;
  readonly brandIsSelected: Signal<boolean> = computed(() => {
    const selected: BrandResponse | undefined = this.selectedBrand();
    return selected !== undefined;
  });

  readonly models: WritableSignal<ModelResponse[]>;
  readonly selectedModel: WritableSignal<ModelResponse | undefined>;
  readonly modelIsSelected: Signal<boolean> = computed(() => {
    const selected: ModelResponse | undefined = this.selectedModel();
    return selected !== undefined;
  });

  public vehicleTextInput: WritableSignal<string | undefined> =
    signal(undefined);

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

  public handleItemTypeChange($event: Event): void {
    const selectedItemType: string | unknown = $event as unknown as string;

    if (selectedItemType === 'Техника') {
      this.selectedItemType.set(selectedItemType);
      this.watchingChanged.emit({ vehicles: true, spares: false });
    }

    if (selectedItemType === 'Запчасти') {
      this.selectedItemType.set(selectedItemType);
      this.watchingChanged.emit({ vehicles: false, spares: true });
    }
  }

  public handleCategoryClear(): void {
    this.selectedCategory.set(undefined);
    this.query.update(
      (q: GetVehiclesQueryParameters): GetVehiclesQueryParameters => {
        return { ...q, CategoryId: null };
      },
    );
  }

  public handleCategoryChange($event: SelectChangeEvent): void {
    const selectedCategory: CategoryResponse = $event.value as CategoryResponse;
    this.selectedCategory.set(selectedCategory);
    this.query.update(
      (q: GetVehiclesQueryParameters): GetVehiclesQueryParameters => {
        return { ...q, CategoryId: selectedCategory.Id };
      },
    );
  }

  public handleBrandChange($event: SelectChangeEvent): void {
    const selectedBrand: BrandResponse = $event.value as BrandResponse;
    this.selectedBrand.set(selectedBrand);
    this.query.update(
      (q: GetVehiclesQueryParameters): GetVehiclesQueryParameters => {
        return { ...q, BrandId: selectedBrand.Id };
      },
    );
  }

  public handleBrandClear(): void {
    this.selectedBrand.set(undefined);
    this.query.update(
      (q: GetVehiclesQueryParameters): GetVehiclesQueryParameters => {
        return { ...q, BrandId: null };
      },
    );
  }

  public handleModelChange($event: SelectChangeEvent): void {
    const selectedModel: ModelResponse = $event.value as ModelResponse;
    this.selectedModel.set(selectedModel);
    this.query.update(
      (q: GetVehiclesQueryParameters): GetVehiclesQueryParameters => {
        return { ...q, ModelId: selectedModel.Id };
      },
    );
  }

  public handleModelClear(): void {
    this.selectedModel.set(undefined);
    this.query.update(
      (q: GetVehiclesQueryParameters): GetVehiclesQueryParameters => {
        return { ...q, ModelId: null };
      },
    );
  }

  public navigateSpares(): void {
    const input: string | undefined = this.vehicleTextInput();
    if (input === '<empty string>') {
      this._router.navigate(['spares']);
    } else {
      this._router.navigate(['spares'], {
        queryParams: {
          textSearch: input,
          page: 1,
        },
      });
    }
  }

  private fetchBrandsOnCategoryChange(): void {
    effect(() => {
      const selectedCategory: CategoryResponse | undefined =
        this.selectedCategory();
      if (selectedCategory) {
        const selectedModel: ModelResponse | undefined = this.selectedModel();
        this._brandsService
          .fetchBrands(
            null,
            null,
            selectedCategory.Id,
            selectedCategory.Name,
            selectedModel?.Id,
            selectedModel?.Name,
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
      if (this.itemTypeIsVehicle()) {
        this._categoriesService
          .fetchCategories()
          .pipe(
            tap((categories: CategoryResponse[]): void =>
              this.categories.set(categories),
            ),
            catchError((): Observable<never> => EMPTY),
          )
          .subscribe();
      }
    });
  }

  private fetchModelsOnBrandChange(): void {
    effect(() => {
      const selectedBrand: BrandResponse | undefined = this.selectedBrand();
      if (!selectedBrand) return;
      const selectedCategory: CategoryResponse | undefined =
        this.selectedCategory();
      this._modelsService
        .fetchModels(
          null,
          null,
          selectedBrand.Id,
          selectedBrand.Name,
          selectedCategory?.Id,
          selectedCategory?.Name,
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
      const category: CategoryResponse | undefined = this.selectedCategory();
      const brand: BrandResponse | undefined = this.selectedBrand();
      const model: ModelResponse | undefined = this.selectedModel();
      if (!category && !brand && !model) return;
      const queryParams: GetVehiclesQueryParameters = this.query();
      this._vehiclesService
        .fetchVehicles(queryParams)
        .pipe(
          tap((response: GetVehiclesQueryResponse): void => {
            this.vehiclesFetched.emit(response.Vehicles);
          }),
          catchError((): Observable<never> => EMPTY),
        )
        .subscribe();
    });
  }

  private fetchSparesIfItemTypeIsSpare(): void {
    effect(() => {
      const type: string | undefined = this.selectedItemType();
      if (type !== 'Запчасти') return;
      const query: GetSparesQueryParameters = this.sparesQuery();
      this._sparesService
        .fetchSpares(query)
        .pipe(
          tap((spares: SpareResponse[]): void => {
            this.sparesFetched.emit(spares);
          }),
        )
        .subscribe();
    });
  }
}
