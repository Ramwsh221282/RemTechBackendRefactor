import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { GetVehiclesQueryResponse } from './vehicles-api.responses';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { GetVehiclesQueryParameters } from './vehicles-api.requests';
import { TypedEnvelope } from '../envelope';

@Injectable({
  providedIn: 'root',
})
export class VehiclesApiService {
  private readonly _httpClient: HttpClient = inject(HttpClient);
  private readonly _apiUrl: string = `${apiUrl}/vehicles`;
  private _fetchingVehicles$: Observable<GetVehiclesQueryResponse> | undefined;

  public fetchVehicles(
    queryParams: GetVehiclesQueryParameters,
  ): Observable<GetVehiclesQueryResponse> {
    return this.invokeFetchVehicles(queryParams);
  }

  private invokeFetchVehicles(
    queryParams: GetVehiclesQueryParameters,
  ): Observable<GetVehiclesQueryResponse> {
    if (this._fetchingVehicles$) return this._fetchingVehicles$;

    let params: HttpParams = new HttpParams();
    if (queryParams.BrandId)
      params = params.append('brand', queryParams.BrandId);

    if (queryParams.CategoryId)
      params = params.append('category', queryParams.CategoryId);

    if (queryParams.RegionId)
      params = params.append('region', queryParams.RegionId);

    if (queryParams.ModelId)
      params = params.append('model', queryParams.ModelId);

    if (queryParams.IsNds)
      params = params.append('nds', queryParams.IsNds.toString());

    if (queryParams.MinimalPrice)
      params = params.append('price-min', queryParams.MinimalPrice.toString());

    if (queryParams.MaximalPrice)
      params = params.append('price-max', queryParams.MaximalPrice.toString());

    if (queryParams.Sort) params = params.append('sort', queryParams.Sort);

    if (queryParams.SortFields) {
      queryParams.SortFields.forEach((field: string) => {
        params = params.append('sort-fields', field);
      });
    }

    if (queryParams.Page)
      params = params.append('page', queryParams.Page.toString());

    if (queryParams.PageSize)
      params = params.append('page-size', queryParams.PageSize.toString());

    if (queryParams.TextSearch)
      params = params.append('text-search', queryParams.TextSearch);

    if (queryParams.Characteristics) {
      for (const [key, value] of Object.entries(queryParams.Characteristics)) {
        params = params.append('characteristics', `${key}:${value}`);
      }
    }

    this._fetchingVehicles$ = this._httpClient
      .get<TypedEnvelope<GetVehiclesQueryResponse>>(this._apiUrl, { params })
      .pipe(
        map(
          (
            envelope: TypedEnvelope<GetVehiclesQueryResponse>,
          ): GetVehiclesQueryResponse => {
            let response: GetVehiclesQueryResponse = {
              Vehicles: [],
              TotalCount: 0,
              AveragePrice: 0,
              MaximalPrice: 0,
              MinimalPrice: 0,
            };
            if (envelope.body) response = envelope.body;
            return response;
          },
        ),
        shareReplay({ refCount: true, bufferSize: 1 }),
        finalize((): void => (this._fetchingVehicles$ = undefined)),
      );
    return this._fetchingVehicles$;
  }
}
