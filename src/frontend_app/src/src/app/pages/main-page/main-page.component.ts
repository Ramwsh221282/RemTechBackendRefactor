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
  MainPageLastAddedItemsResponse,
  SpareData,
  VehicleData,
} from '../../shared/api/main-page/main-page-responses';
import { catchError, EMPTY, forkJoin, map, Observable, tap } from 'rxjs';
import { TypedEnvelope } from '../../shared/api/envelope';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../shared/utils/message-service-utils';

type StatisticsResponseData = {
  categories: CategoriesPopularity[];
  brands: BrandsPopularity[];
  stats: ItemStats[];
};

type LastAddedItemsResponseData = {
  vehicles: VehicleData[];
  spares: SpareData[];
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
    this.spares = signal([]);
    effect(() => {
      this.fetchMainPageInformation();
    });
  }

  readonly categoriesPopularity: WritableSignal<CategoriesPopularity[]>;
  readonly brandsPopularity: WritableSignal<BrandsPopularity[]>;
  readonly itemStats: WritableSignal<ItemStats[]>;
  readonly vehicles: WritableSignal<VehicleData[]>;
  readonly spares: WritableSignal<SpareData[]>;

  private fetchMainPageInformation(): void {
    forkJoin([this.fetchLastAddedItems(), this.fetchItemsStatistics()])
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        tap((result): void => {
          const items: LastAddedItemsResponseData = result[0];
          const stats: StatisticsResponseData = result[1];
          this.itemStats.set(stats.stats);
          this.categoriesPopularity.set(stats.categories);
          this.brandsPopularity.set(stats.brands);
          this.vehicles.set(items.vehicles);
          this.spares.set(items.spares);
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message = error.error.message;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        }),
      )
      .subscribe();
  }

  private fetchLastAddedItems(): Observable<LastAddedItemsResponseData> {
    return this._apiService.fetchLastAddedItems().pipe(
      takeUntilDestroyed(this._destroyRef),
      map(
        (
          envelope: TypedEnvelope<MainPageLastAddedItemsResponse>,
        ): LastAddedItemsResponseData => {
          let response: LastAddedItemsResponseData = {
            vehicles: [],
            spares: [],
          };
          if (envelope.body) {
            for (const item of envelope.body.Items) {
              if (item.Spare) response.spares.push(item.Spare);
              if (item.Vehicle) response.vehicles.push(item.Vehicle);
            }
          }
          return response;
        },
      ),
    );
  }

  private fetchItemsStatistics(): Observable<StatisticsResponseData> {
    return this._apiService.fetchItemStatistics().pipe(
      takeUntilDestroyed(this._destroyRef),
      map((envelope: TypedEnvelope<MainPageItemStatsResponse>) => {
        let response: StatisticsResponseData = {
          brands: [],
          categories: [],
          stats: [],
        };
        if (envelope.body) {
          response.brands = envelope.body.BrandsPopularity;
          response.categories = envelope.body.CategoriesPopularity;
          response.stats = envelope.body.ItemStats;
        }
        return response;
      }),
    );
  }
}
