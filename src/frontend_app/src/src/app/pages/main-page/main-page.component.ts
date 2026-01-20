import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { MessageService } from 'primeng/api';
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
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly _apiService: MainPageApiService,
    private readonly _messageService: MessageService,
  ) {
    this.categoriesPopularity = signal([]);
    this.brandsPopularity = signal([]);
    this.itemStats = signal([]);
    this.vehicles = signal([]);
    this.fetchItemsStatistics();
  }

  readonly categoriesPopularity: WritableSignal<CategoriesPopularity[]>;
  readonly brandsPopularity: WritableSignal<BrandsPopularity[]>;
  readonly itemStats: WritableSignal<ItemStats[]>;
  readonly vehicles: WritableSignal<VehicleResponse[]>;

  public handleVehiclesFetched($event: VehicleResponse[]): void {
    console.log('Vehicles fetched event received in MainPageComponent');
    this.vehicles.set($event);
    console.log(this.vehicles());
  }

  private fetchItemsStatistics(): void {
    effect(() => {
      this._apiService
        .fetchItemStatistics()
        .pipe(
          map((response: MainPageItemStatsResponse): StatisticsResponseData => {
            return {
              brands: response.BrandsPopularity,
              categories: response.CategoriesPopularity,
              stats: response.ItemStats,
            };
          }),
          tap((data: StatisticsResponseData): void => {
            this.itemStats.set(data.stats);
            this.categoriesPopularity.set(data.categories);
            this.brandsPopularity.set(data.brands);
          }),
          catchError((): Observable<never> => EMPTY),
        )
        .subscribe();
    });
  }
}
