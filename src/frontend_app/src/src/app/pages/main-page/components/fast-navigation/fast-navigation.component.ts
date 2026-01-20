import {
  Component,
  computed,
  DestroyRef,
  effect,
  inject,
  Signal,
  signal,
  WritableSignal,
} from '@angular/core';
import { Select, SelectChangeEvent } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { CatalogueVehiclesService } from '../../../vehicles-page/services/CatalogueVehiclesService';
import { CatalogueBrand } from '../../../vehicles-page/types/CatalogueBrand';
import { CatalogueModel } from '../../../vehicles-page/types/CatalogueModel';
import { Router } from '@angular/router';
import { InputText } from 'primeng/inputtext';
import { BrandsApiService } from '../../../../shared/api/brands-module/brands-api.service';
import { CategoriesApiService } from '../../../../shared/api/categories-module/categories-api.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, EMPTY, map, tap } from 'rxjs';
import { CategoryResponse } from '../../../../shared/api/categories-module/categories-responses';
import { TypedEnvelope } from '../../../../shared/api/envelope';
import { BrandResponse } from '../../../../shared/api/brands-module/brands-api.responses';

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
    private readonly _modelsService: CatalogueVehiclesService,
    router: Router,
  ) {
    this._router = router;
    this.brands = signal([]);
    this.models = signal([]);
    this.categories = signal([]);
    this.selectedBrand = signal(undefined);
    this.selectedModel = signal(undefined);
    this.selectedCategory = signal(undefined);
    this.selectedItemType = signal(undefined);
    this.itemTypes = signal(['Техника', 'Запчасти']);
    this.fetchCategories();

    // effect(() => {
    //   service
    //     .fetchCategories()
    //     .pipe(takeUntilDestroyed(this._destroyRef))
    //     .subscribe({
    //       next: (data: CatalogueCategory[]): void => {
    //         this._categories.set(data);
    //       },
    //     });
    // });
    // effect(() => {
    //   const selectedType: CatalogueCategory | undefined = this.selectedType();
    //   if (selectedType !== undefined) {
    //     service
    //       .fetchCategoryBrands(selectedType.id)
    //       .pipe(takeUntilDestroyed(this._destroyRef))
    //       .subscribe({
    //         next: (data: CatalogueBrand[]): void => {
    //           this._brands.set(data);
    //         },
    //       });
    //   }
    // });
    // effect(() => {
    //   const selectedBrand: CatalogueBrand | undefined = this.selectedBrand();
    //   const selectedType: CatalogueCategory | undefined = this.selectedType();
    //   if (selectedBrand && selectedType) {
    //     service
    //       .fetchModelsCategoryBrands(selectedType.id, selectedBrand.id)
    //       .pipe(takeUntilDestroyed(this._destroyRef))
    //       .subscribe({
    //         next: (data: CatalogueModel[]): void => {
    //           this._models.set(data);
    //         },
    //       });
    //   }
    // });
  }

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

  readonly models: WritableSignal<CatalogueModel[]>;
  readonly selectedModel: WritableSignal<CatalogueModel | undefined>;
  readonly modelIsSelected: Signal<boolean> = computed(() => {
    const selected: CatalogueModel | undefined = this.selectedModel();
    return selected !== undefined;
  });

  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _router: Router;

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
    if (selectedItemType === 'Техника' || selectedItemType === 'Запчасти')
      this.selectedItemType.set(selectedItemType);
  }

  public handleCategoryClear(): void {
    this.selectedCategory.set(undefined);
  }

  public handleCategoryChange($event: SelectChangeEvent): void {
    const selectedCategory: CategoryResponse = $event.value as CategoryResponse;
    this.selectedCategory.set(selectedCategory);
  }

  public handleBrandChange($event: SelectChangeEvent): void {
    const selectedBrand: BrandResponse = $event.value as BrandResponse;
    this.selectedBrand.set(selectedBrand);
  }

  public handleBrandClear(): void {
    this.selectedBrand.set(undefined);
  }

  public handleModelChange($event: SelectChangeEvent): void {
    const selectedModel: CatalogueModel = $event.value as CatalogueModel;
    this.selectedModel.set(selectedModel);
  }

  public handleModelClear(): void {
    this.selectedModel.set(undefined);
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

  private fetchBrands(): void {
    effect(() => {});
  }

  private fetchCategories(): void {
    effect(() => {
      if (this.itemTypeIsVehicle()) {
        console.log('fetching categories for vehicles');
        this._categoriesService
          .fetchCategories()
          .pipe(
            takeUntilDestroyed(this._destroyRef),
            map((data: TypedEnvelope<CategoryResponse[]>) => {
              let response: CategoryResponse[] = [];
              if (data.body) response = data.body;
              return response;
            }),
            tap((categories: CategoryResponse[]): void =>
              this.categories.set(categories),
            ),
            catchError(() => {
              return EMPTY;
            }),
          )
          .subscribe();
      }
    });
  }
}
