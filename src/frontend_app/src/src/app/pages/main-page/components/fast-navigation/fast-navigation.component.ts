import {
  Component,
  computed,
  effect,
  EventEmitter,
  Input,
  Output,
  Signal,
  signal,
  WritableSignal,
} from '@angular/core';
import { Select } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { InputText } from 'primeng/inputtext';
import { tap } from 'rxjs';
import { CategoryResponse } from '../../../../shared/api/categories-module/categories-responses';
import { BrandResponse } from '../../../../shared/api/brands-module/brands-api.responses';
import { ModelResponse } from '../../../../shared/api/models-module/models-responses';
import { VehicleResponse } from '../../../../shared/api/vehicles-module/vehicles-api.responses';
import { SparesApiService } from '../../../../shared/api/spares-module/spares-api.service';
import {
  GetSparesQueryResponse,
  SpareResponse,
} from '../../../../shared/api/spares-module/spares-api.responses';
import { GetSparesQueryParameters } from '../../../../shared/api/spares-module/spares-api.requests';
import { DefaultGetSpareParameters } from '../../../../shared/api/spares-module/spares-api.factories';
import { FastNavigationVehiclesComponent } from './fast-navigation-vehicles/fast-navigation-vehicles.component';
import { FastNavigationSparesComponent } from './fast-navigation-spares/fast-navigation-spares.component';

@Component({
  selector: 'app-fast-navigation',
  imports: [
    Select,
    FormsModule,
    InputText,
    FastNavigationVehiclesComponent,
    FastNavigationSparesComponent,
  ],
  templateUrl: './fast-navigation.component.html',
  styleUrl: './fast-navigation.component.scss',
})
export class FastNavigationComponent {
  constructor(private readonly _sparesService: SparesApiService) {
    this.sparesFetched = new EventEmitter<{
      spares: SpareResponse[];
      totalCount: number;
    }>();
    this.watchingChanged = new EventEmitter<{
      vehicles: boolean;
      spares: boolean;
    }>();
    this.vehiclesFetched = new EventEmitter<{
      vehicles: VehicleResponse[];
      totalCount: number;
    }>();
    this.brandSelected = new EventEmitter<BrandResponse | undefined>();
    this.categorySelected = new EventEmitter<CategoryResponse | undefined>();
    this.modelSelected = new EventEmitter<ModelResponse | undefined>();
    this.spareTextSearchChanged = new EventEmitter<string | undefined>();

    this.sparesFetchProps = signal({
      isWatching: false,
      page: undefined,
      pageSize: undefined,
    });
    this.vehiclesFetchProps = signal({
      page: undefined,
      brand: undefined,
      category: undefined,
      model: undefined,
      isWatching: false,
      pageSize: undefined,
    });

    this.itemTypes = ['Техника', 'Запчасти'];
    this.fetchSparesOnQueryChange();
  }

  readonly sparesFetchProps: WritableSignal<{
    page?: number | undefined;
    pageSize?: number | undefined;
    isWatching: boolean;
    textSearch?: string | undefined;
  }>;

  readonly vehiclesFetchProps: WritableSignal<{
    page?: number | undefined;
    pageSize?: number | undefined;
    isWatching: boolean;
    category?: CategoryResponse | undefined;
    brand?: BrandResponse | undefined;
    model?: ModelResponse | undefined;
    textSearch?: string | undefined;
  }>;

  @Input({ required: true }) set spares_fetch_props(value: {
    page?: number | undefined;
    pageSize?: number | undefined;
    isWatching: boolean;
    textSearch?: string | undefined;
  }) {
    this.sparesFetchProps.set(value);
  }

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
  @Output() spareTextSearchChanged: EventEmitter<string | undefined>;

  @Output() vehiclesFetched: EventEmitter<{
    vehicles: VehicleResponse[];
    totalCount: number;
  }>;
  @Output() sparesFetched: EventEmitter<{
    spares: SpareResponse[];
    totalCount: number;
  }>;
  @Output() watchingChanged: EventEmitter<{
    vehicles: boolean;
    spares: boolean;
  }>;

  readonly itemTypes: string[];

  readonly userIsWatchingVehicles: Signal<boolean> = computed((): boolean => {
    return this.vehiclesFetchProps().isWatching;
  });

  readonly userIsWatchingSpares: Signal<boolean> = computed((): boolean => {
    return this.sparesFetchProps().isWatching;
  });

  readonly readSelectedItemType: Signal<string> = computed((): string => {
    const vehicleProps = this.vehiclesFetchProps();
    return vehicleProps.isWatching ? 'Техника' : 'Запчасти';
  });

  public spareTextInput: WritableSignal<string | undefined> = signal(undefined);

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

  public navigateSpares(): void {}

  public handleUserItemTypeSelect($event: Event): void {
    const selectedItemType: string | unknown = $event as unknown as string;
    if (selectedItemType === 'Техника')
      this.watchingChanged.emit({ vehicles: true, spares: false });
    if (selectedItemType === 'Запчасти')
      this.watchingChanged.emit({ vehicles: false, spares: true });
  }

  public handleUserSpareTextInput($event: Event): void {
    const inputText: string = ($event.target as HTMLInputElement).value;
    this.spareTextSearchChanged.emit(inputText);
    this.spareTextInput.set(inputText);
  }

  private fetchSparesOnQueryChange(): void {
    effect(() => {
      const props: {
        page?: number | undefined;
        pageSize?: number | undefined;
        isWatching: boolean;
      } = this.sparesFetchProps();
      if (!props.isWatching) return;
      const query: GetSparesQueryParameters = {
        ...DefaultGetSpareParameters(),
        Page: props.page,
        PageSize: props.pageSize,
        TextSearch: this.spareTextInput(),
      };
      this.invokeFetchSpares(query);
    });
  }

  private invokeFetchSpares(query: GetSparesQueryParameters): void {
    this._sparesService
      .fetchSpares(query)
      .pipe(
        tap((response: GetSparesQueryResponse): void => {
          this.sparesFetched.emit({
            spares: response.Spares,
            totalCount: response.TotalCount,
          });
        }),
      )
      .subscribe();
  }
}
