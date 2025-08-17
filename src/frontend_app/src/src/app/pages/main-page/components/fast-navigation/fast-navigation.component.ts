import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { Select } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { CatalogueVehiclesService } from '../../../vehicles-page/services/CatalogueVehiclesService';
import { CatalogueCategory } from '../../../vehicles-page/types/CatalogueCategory';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CatalogueBrand } from '../../../vehicles-page/types/CatalogueBrand';
import { CatalogueModel } from '../../../vehicles-page/types/CatalogueModel';
import { Router } from '@angular/router';
import { InputText } from 'primeng/inputtext';

@Component({
  selector: 'app-fast-navigation',
  imports: [Select, FormsModule, InputText],
  templateUrl: './fast-navigation.component.html',
  styleUrl: './fast-navigation.component.scss',
})
export class FastNavigationComponent {
  private readonly _fastNavigationCategories: WritableSignal<string[]>;
  private readonly _currentNavigationCategory: WritableSignal<string>;
  private readonly _categories: WritableSignal<CatalogueCategory[]>;
  private readonly _brands: WritableSignal<CatalogueBrand[]>;
  private readonly _models: WritableSignal<CatalogueModel[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _router: Router;
  public selectedCategory: string;
  public selectedModel: WritableSignal<CatalogueModel | undefined> =
    signal(undefined);
  public selectedType: WritableSignal<CatalogueCategory | undefined> =
    signal(undefined);
  public selectedBrand: WritableSignal<CatalogueBrand | undefined> =
    signal(undefined);
  public vehicleTextInput: WritableSignal<string | undefined> =
    signal(undefined);

  constructor(service: CatalogueVehiclesService, router: Router) {
    this._router = router;
    this._brands = signal([]);
    this._models = signal([]);
    this._fastNavigationCategories = signal(['Техника', 'Запчасти']);
    this._currentNavigationCategory = signal(
      this._fastNavigationCategories()[0],
    );
    this._categories = signal([]);
    this.selectedCategory = this._fastNavigationCategories()[0];
    effect(() => {
      service
        .fetchCategories()
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: CatalogueCategory[]): void => {
            this._categories.set(data);
          },
        });
    });
    effect(() => {
      const selectedType: CatalogueCategory | undefined = this.selectedType();
      if (selectedType !== undefined) {
        service
          .fetchCategoryBrands(selectedType.id)
          .pipe(takeUntilDestroyed(this._destroyRef))
          .subscribe({
            next: (data: CatalogueBrand[]): void => {
              this._brands.set(data);
            },
          });
      }
    });
    effect(() => {
      const selectedBrand: CatalogueBrand | undefined = this.selectedBrand();
      const selectedType: CatalogueCategory | undefined = this.selectedType();
      if (selectedBrand && selectedType) {
        service
          .fetchModelsCategoryBrands(selectedType.id, selectedBrand.id)
          .pipe(takeUntilDestroyed(this._destroyRef))
          .subscribe({
            next: (data: CatalogueModel[]): void => {
              this._models.set(data);
            },
          });
      }
    });
  }

  public get vehicleBrands(): CatalogueBrand[] {
    return this._brands();
  }

  public get vehicleTypes(): CatalogueCategory[] {
    return this._categories();
  }

  public get models(): CatalogueModel[] {
    return this._models();
  }

  public get navigationCategories(): string[] {
    return this._fastNavigationCategories();
  }

  public get currentNavigationCategory(): string {
    return this._currentNavigationCategory();
  }

  public navigateVehicles(): void {
    const brand: CatalogueBrand | undefined = this.selectedBrand();
    const model: CatalogueModel | undefined = this.selectedModel();
    const type: CatalogueCategory | undefined = this.selectedType();
    if (brand && model && type) {
      this._router.navigate(['vehicles'], {
        queryParams: {
          brandId: brand.id,
          categoryId: type.id,
          modelId: model.id,
          page: 1,
        },
      });
    }
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
}
