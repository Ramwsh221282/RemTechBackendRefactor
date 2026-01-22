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
    id?: string | null | undefined,
    name?: string | null | undefined,
    categoryId?: string | null | undefined,
    categoryName?: string | null | undefined,
    modelId?: string | null | undefined,
    modelName?: string | null | undefined,
    page?: number | null | undefined,
    pageSize?: number | null | undefined,
    textSearch?: string | null | undefined,
    useVehiclesCount: boolean = false,
    useBrandsCount: boolean = false,
  ): Observable<BrandResponse[]> {
    return this.invokeBrandsFetch(
      id,
      name,
      categoryId,
      categoryName,
      modelId,
      modelName,
      page,
      pageSize,
      textSearch,
      useVehiclesCount,
      useBrandsCount,
    );
  }

  private invokeBrandsFetch(
    id?: string | null | undefined,
    name?: string | null | undefined,
    categoryId?: string | null | undefined,
    categoryName?: string | null | undefined,
    modelId?: string | null | undefined,
    modelName?: string | null | undefined,
    page?: number | null | undefined,
    pageSize?: number | null | undefined,
    textSearch?: string | null | undefined,
    useVehiclesCount: boolean = false,
    useBrandsCount: boolean = false,
  ): Observable<BrandResponse[]> {
    if (this._fetchingBrands$) return this._fetchingBrands$;

    let params: HttpParams = new HttpParams();
    const includes: string[] = [];

    if (id) params = params.append('id', id);
    if (name) params = params.append('name', name);
    if (categoryId) params = params.append('categoryId', categoryId);
    if (categoryName) params = params.append('categoryName', categoryName);
    if (modelId) params = params.append('modelId', modelId);
    if (modelName) params = params.append('modelName', modelName);
    if (page) params = params.append('page', page.toString());
    if (pageSize) params = params.append('pageSize', pageSize.toString());
    if (useVehiclesCount) params = params.append('include', 'vehicles-count');
    if (useBrandsCount) params = params.append('include', 'brands-count');
    if (textSearch) {
      params = params.append('text-search', textSearch);
      params = params.append('include', 'text-search-score');
    }
    if (includes) {
      for (const include of includes) {
        params = params.append('include', include);
      }
    }

    this._fetchingBrands$ = this._httpClient
      .get<TypedEnvelope<BrandResponse[]>>(this._apiUrl, { params })
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
