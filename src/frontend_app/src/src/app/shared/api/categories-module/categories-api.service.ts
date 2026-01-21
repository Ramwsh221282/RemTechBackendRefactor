import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { apiUrl } from '../api-endpoint';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import { CategoryResponse } from './categories-responses';

@Injectable({
  providedIn: 'root',
})
export class CategoriesApiService {
  private readonly _httpClient: HttpClient = inject(HttpClient);
  private readonly _apiUrl: string = `${apiUrl}/categories`;
  private _fetchingCategories$: Observable<CategoryResponse[]> | undefined;

  public fetchCategories(
    id?: string | null | undefined,
    name?: string | null | undefined,
    brandId?: string | null | undefined,
    brandName?: string | null | undefined,
    modelId?: string | null | undefined,
    modelName?: string | null | undefined,
    page?: number | null | undefined,
    pageSize?: number | null | undefined,
    withVehiclesCount: boolean = false,
    textSearch?: string | null | undefined,
  ): Observable<CategoryResponse[]> {
    return this.invokeFetchCategories(
      id,
      name,
      brandId,
      brandName,
      modelId,
      modelName,
      page,
      pageSize,
      withVehiclesCount,
      textSearch,
    );
  }

  private invokeFetchCategories(
    id?: string | null | undefined,
    name?: string | null | undefined,
    brandId?: string | null | undefined,
    brandName?: string | null | undefined,
    modelId?: string | null | undefined,
    modelName?: string | null | undefined,
    page?: number | null | undefined,
    pageSize?: number | null | undefined,
    withVehiclesCount: boolean = false,
    textSearch?: string | null | undefined,
  ): Observable<CategoryResponse[]> {
    if (this._fetchingCategories$) return this._fetchingCategories$;
    const includes: string[] = [];
    let params: HttpParams = new HttpParams();
    if (id) params = params.append('id', id);
    if (name) params = params.append('name', name);
    if (brandId) params = params.append('brandId', brandId);
    if (brandName) params = params.append('brandName', brandName);
    if (modelId) params = params.append('modelId', modelId);
    if (modelName) params = params.append('modelName', modelName);
    if (page) params = params.append('page', page.toString());
    if (pageSize) params = params.append('pageSize', pageSize.toString());
    if (textSearch) {
      params = params.append('text-search', textSearch);
      includes.push('text-search-score');
    }
    if (withVehiclesCount) includes.push('vehicles-count');
    if (includes.length > 0) {
      for (const include of includes) {
        params = params.append('include', include);
      }
    }
    this._fetchingCategories$ = this._httpClient
      .get<TypedEnvelope<CategoryResponse[]>>(this._apiUrl, { params })
      .pipe(
        map(
          (envelope: TypedEnvelope<CategoryResponse[]>): CategoryResponse[] => {
            if (envelope.body) return envelope.body;
            throw new Error('No category found in the response body');
          },
        ),
        finalize((): void => (this._fetchingCategories$ = undefined)),
        shareReplay({ bufferSize: 1, refCount: true }),
      );
    return this._fetchingCategories$;
  }
}
