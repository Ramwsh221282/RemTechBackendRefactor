import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import { BrandResponse } from './brands-api.responses';
import { apiUrl } from '../api-endpoint';

@Injectable({
  providedIn: 'root',
})
export class BrandsApiService {
  private readonly _httpClient: HttpClient = inject(HttpClient);
  private readonly _apiUrl: string = `${apiUrl}/brands`;
  private _fetchingBrands$: Observable<BrandResponse[]> | undefined;

  public fetchBrands(
    categoryId?: string | null | undefined,
    categoryName?: string | null | undefined,
    modelId?: string | null | undefined,
    modelName?: string | null | undefined,
  ): Observable<BrandResponse[]> {
    return this.invokeBrandsFetch(categoryId, categoryName, modelId, modelName);
  }

  private invokeBrandsFetch(
    categoryId?: string | null | undefined,
    categoryName?: string | null | undefined,
    modelId?: string | null | undefined,
    modelName?: string | null | undefined,
  ): Observable<BrandResponse[]> {
    if (this._fetchingBrands$) return this._fetchingBrands$;
    const requestUrl: string = `${this._apiUrl}/list`;
    let params: HttpParams = new HttpParams();
    if (categoryId) params = params.append('categoryId', categoryId);
    if (categoryName) params = params.append('categoryName', categoryName);
    if (modelId) params = params.append('modelId', modelId);
    if (modelName) params = params.append('modelName', modelName);
    this._fetchingBrands$ = this._httpClient
      .get<TypedEnvelope<BrandResponse[]>>(requestUrl, { params })
      .pipe(
        map((envelope: TypedEnvelope<BrandResponse[]>): BrandResponse[] => {
          let response: BrandResponse[] = [];
          if (envelope.body) response = envelope.body;
          return response;
        }),
        finalize((): void => (this._fetchingBrands$ = undefined)),
        shareReplay({ refCount: true, bufferSize: 1 }),
      );
    return this._fetchingBrands$;
  }
}
