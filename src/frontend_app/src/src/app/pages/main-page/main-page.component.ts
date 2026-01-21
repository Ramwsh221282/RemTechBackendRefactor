import { Component, effect, signal, WritableSignal } from '@angular/core';
import { ContainedItemsInfoComponent } from './components/contained-items-info/contained-items-info.component';
import { PopularCategoriesBlockComponent } from './components/popular-categories-block/popular-categories-block.component';
import { PopularBrandsBlockComponent } from './components/popular-brands-block/popular-brands-block.component';
import { RecentItemsListComponent } from './components/recent-items-list/recent-items-list.component';
import { FastNavigationComponent } from './components/fast-navigation/fast-navigation.component';
import { MainPageApiService } from '../../shared/api/main-page/main-page-api.service';
import {
  BrandsPopularity,
  CategoriesPopularity,
  ItemStats,
  MainPageItemStatsResponse,
} from '../../shared/api/main-page/main-page-responses';
import { catchError, EMPTY, map, Observable, tap } from 'rxjs';
import { VehicleResponse } from '../../shared/api/vehicles-module/vehicles-api.responses';
import { SpareResponse } from '../../shared/api/spares-module/spares-api.responses';
import { CategoryResponse } from '../../shared/api/categories-module/categories-responses';
import { BrandResponse } from '../../shared/api/brands-module/brands-api.responses';
import { ModelResponse } from '../../shared/api/models-module/models-responses';

type StatisticsResponseData = {
  categories: CategoriesPopularity[];
  brands: BrandsPopularity[];
  stats: ItemStats[];
};

@Component({
  selector: 'app-main-page',
  imports: [
    ContainedItemsInfoComponent,
    PopularCategoriesBlockComponent,
    PopularBrandsBlockComponent,
    RecentItemsListComponent,
    FastNavigationComponent,
  ],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.scss',
})
export class MainPageComponent {
  constructor(private readonly _apiService: MainPageApiService) {
    this.categoriesPopularity = signal([]);
    this.brandsPopularity = signal([]);
    this.itemStats = signal([]);
    this.vehicles = signal([]);
    this.spares = signal([]);
    this.watchingSpares = signal(false);
    this.watchingVehicles = signal(false);
    this.selectedBrand = signal(undefined);
    this.selectedCategory = signal(undefined);
    this.selectedModel = signal(undefined);
    this.vehiclesTextSearch = signal(undefined);
    this.sparesTextSearch = signal(undefined);
    this.fetchItemsStatistics();
  }

  readonly categoriesPopularity: WritableSignal<CategoriesPopularity[]>;
  readonly brandsPopularity: WritableSignal<BrandsPopularity[]>;
  readonly vehicles: WritableSignal<VehicleResponse[]>;
  readonly watchingVehicles: WritableSignal<boolean>;
  readonly watchingSpares: WritableSignal<boolean>;
  readonly spares: WritableSignal<SpareResponse[]>;
  readonly itemStats: WritableSignal<ItemStats[]>;
  readonly sparesPaginationState: WritableSignal<number> = signal(1);
  readonly vehiclesPaginationState: WritableSignal<number> = signal(1);
  readonly totalSparesCount: WritableSignal<number> = signal(0);
  readonly totalVehiclesCount: WritableSignal<number> = signal(0);
  readonly itemsPerPage: number = 10;
  readonly selectedCategory: WritableSignal<CategoryResponse | undefined>;
  readonly selectedBrand: WritableSignal<BrandResponse | undefined>;
  readonly selectedModel: WritableSignal<ModelResponse | undefined>;
  readonly vehiclesTextSearch: WritableSignal<string | undefined>;
  readonly sparesTextSearch: WritableSignal<string | undefined>;

  public handleSparesPageChange($event: number): void {
    this.sparesPaginationState.set($event);
  }

  public handleVehiclesPageChange($event: number): void {
    this.vehiclesPaginationState.set($event);
  }

  public handleVehiclesFetched($event: {
    vehicles: VehicleResponse[];
    totalCount: number;
  }): void {
    this.vehicles.set($event.vehicles);
    this.totalVehiclesCount.set($event.totalCount);
  }

  public handleSparesFetched($event: {
    spares: SpareResponse[];
    totalCount: number;
  }): void {
    this.spares.set($event.spares);
    this.totalSparesCount.set($event.totalCount);
  }

  public handleWatchSet($event: { vehicles: boolean; spares: boolean }): void {
    this.watchingVehicles.set($event.vehicles);
    this.watchingSpares.set($event.spares);
  }

  private fetchItemsStatistics(): void {
    effect(() => {
      this._apiService
        .fetchItemStatistics()
        .pipe(
          map(
            (response: MainPageItemStatsResponse): StatisticsResponseData =>
              this.mapToCurrentComponentStatisticsProps(response),
          ),
          tap((data: StatisticsResponseData): void =>
            this.refreshStatisticsSignals(data),
          ),
          catchError((): Observable<never> => EMPTY),
        )
        .subscribe();
    });
  }

  private mapToCurrentComponentStatisticsProps(
    response: MainPageItemStatsResponse,
  ): StatisticsResponseData {
    return {
      brands: response.BrandsPopularity,
      categories: response.CategoriesPopularity,
      stats: response.ItemStats,
    };
  }

  private refreshStatisticsSignals(data: StatisticsResponseData): void {
    this.itemStats.set(data.stats);
    this.categoriesPopularity.set(data.categories);
    this.brandsPopularity.set(data.brands);
  }
}
