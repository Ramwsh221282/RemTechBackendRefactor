import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { HttpClient, HttpParams } from '@angular/common/http';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { GetSparesQueryResponse, SpareResponse } from './spares-api.responses';
import { TypedEnvelope } from '../envelope';
import { GetSparesQueryParameters } from './spares-api.requests';

@Injectable({
  providedIn: 'root',
})
export class SparesApiService {
  private readonly _apiUrl: string = `${apiUrl}/spares`;
  private readonly _httpClient: HttpClient = inject(HttpClient);
  private fetchingSpares$: Observable<SpareResponse[]> | undefined;

  public fetchSpares(
    query: GetSparesQueryParameters,
  ): Observable<SpareResponse[]> {
    return this.invokeFetchingSpares(query);
  }

  private invokeFetchingSpares(
    query: GetSparesQueryParameters,
  ): Observable<SpareResponse[]> {
    if (this.fetchingSpares$) return this.fetchingSpares$;
    let params: HttpParams = new HttpParams();
    if (query.RegionId) params = params.append('region-id', query.RegionId);
    if (query.MinimalPrice)
      params = params.append('price-min', query.MinimalPrice.toString());
    if (query.MaximalPrice)
      params = params.append('price-max', query.MaximalPrice.toString());
    if (query.TextSearch)
      params = params.append('text-search', query.TextSearch);
    if (query.Page) params = params.append('page', query.Page.toString());
    if (query.PageSize)
      params = params.append('page-size', query.PageSize.toString());
    if (query.OrderMode) params = params.append('sort-mode', query.OrderMode);
    if (query.Oem) params = params.append('oem', query.Oem);
    this.fetchingSpares$ = this._httpClient
      .get<TypedEnvelope<GetSparesQueryResponse>>(this._apiUrl, { params })
      .pipe(
        map(
          (
            envelope: TypedEnvelope<GetSparesQueryResponse>,
          ): SpareResponse[] => {
            let response: SpareResponse[] = [];
            if (envelope.body) response = [...envelope.body.Spares];
            return response;
          },
        ),
        shareReplay({ refCount: true, bufferSize: 1 }),
        finalize((): void => (this.fetchingSpares$ = undefined)),
      );
    return this.fetchingSpares$;
  }
}
