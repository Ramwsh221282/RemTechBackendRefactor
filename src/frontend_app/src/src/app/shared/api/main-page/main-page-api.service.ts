import { inject, Injectable } from '@angular/core';
import { finalize, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import {
  MainPageItemStatsResponse,
  MainPageLastAddedItemsResponse,
} from './main-page-responses';
import { HttpClient } from '@angular/common/http';
import { apiUrl } from '../api-endpoint';

@Injectable({
  providedIn: 'root',
})
export class MainPageApiService {
  public fetchItemStatistics(): Observable<
    TypedEnvelope<MainPageItemStatsResponse>
  > {
    return this.startFetchingStatistics();
  }

  public fetchLastAddedItems(): Observable<
    TypedEnvelope<MainPageLastAddedItemsResponse>
  > {
    return this.startFetchingLastAddedItems();
  }

  private startFetchingStatistics(): Observable<
    TypedEnvelope<MainPageItemStatsResponse>
  > {
    if (this._statisticsFetch$) return this._statisticsFetch$;

    const requestUrl: string = `${this._apiUrl}/main-page/statistics`;
    this._statisticsFetch$ = this._httpClient
      .get<TypedEnvelope<MainPageItemStatsResponse>>(requestUrl)
      .pipe(
        finalize((): void => (this._statisticsFetch$ = undefined)),
        shareReplay({ refCount: true, bufferSize: 1 }),
      );
    return this._statisticsFetch$;
  }

  private startFetchingLastAddedItems(): Observable<
    TypedEnvelope<MainPageLastAddedItemsResponse>
  > {
    if (this._lastAddedItemsFetch$) return this._lastAddedItemsFetch$;

    const requestUrl: string = `${this._apiUrl}/main-page/last-added`;
    this._lastAddedItemsFetch$ = this._httpClient
      .get<TypedEnvelope<MainPageLastAddedItemsResponse>>(requestUrl)
      .pipe(
        finalize((): void => (this._lastAddedItemsFetch$ = undefined)),
        shareReplay({ refCount: true, bufferSize: 1 }),
      );
    return this._lastAddedItemsFetch$;
  }

  private _statisticsFetch$:
    | Observable<TypedEnvelope<MainPageItemStatsResponse>>
    | undefined;

  private _lastAddedItemsFetch$:
    | Observable<TypedEnvelope<MainPageLastAddedItemsResponse>>
    | undefined;

  private readonly _httpClient: HttpClient = inject(HttpClient);
  private readonly _apiUrl: string = `${apiUrl}/contained-items`;
}
