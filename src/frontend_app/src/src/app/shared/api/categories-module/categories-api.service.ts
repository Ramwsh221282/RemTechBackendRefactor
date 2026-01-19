import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { apiUrl } from '../api-endpoint';
import { finalize, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import { CategoryResponse } from './categories-responses';

@Injectable({
  providedIn: 'root',
})
export class CategoriesApiService {
  private readonly _httpClient: HttpClient = inject(HttpClient);
  private readonly _apiUrl: string = `${apiUrl}/categories`;

  private _fetchingCategory:
    | Observable<TypedEnvelope<CategoryResponse>>
    | undefined;
  private _fetchingCategories$:
    | Observable<TypedEnvelope<CategoryResponse[]>>
    | undefined;

  public fetchCategory(
    id?: string | null | undefined,
    name?: string | null | undefined,
    brandId?: string | null | undefined,
    brandName?: string | null | undefined,
    modelId?: string | null | undefined,
    modelName?: string | null | undefined,
  ): Observable<TypedEnvelope<CategoryResponse>> {
    return this.invokeFetchCategory(
      id,
      name,
      brandId,
      brandName,
      modelId,
      modelName,
    );
  }

  public fetchCategories(): Observable<TypedEnvelope<CategoryResponse[]>> {
    return this.invokeFetchCategories();
  }

  private invokeFetchCategories(): Observable<
    TypedEnvelope<CategoryResponse[]>
  > {
    if (this._fetchingCategories$) return this._fetchingCategories$;
    const requestUrl: string = `${this._apiUrl}/all`;
    this._fetchingCategories$ = this._httpClient
      .get<TypedEnvelope<CategoryResponse[]>>(requestUrl)
      .pipe(
        finalize((): void => (this._fetchingCategories$ = undefined)),
        shareReplay({ bufferSize: 1, refCount: true }),
      );
    return this._fetchingCategories$;
  }

  private invokeFetchCategory(
    id?: string | null | undefined,
    name?: string | null | undefined,
    brandId?: string | null | undefined,
    brandName?: string | null | undefined,
    modelId?: string | null | undefined,
    modelName?: string | null | undefined,
  ): Observable<TypedEnvelope<CategoryResponse>> {
    if (this._fetchingCategory) return this._fetchingCategory;
    let params: HttpParams = new HttpParams();
    if (id) params = params.append('id', id);
    if (name) params = params.append('name', name);
    if (brandId) params = params.append('brandId', brandId);
    if (brandName) params = params.append('brandName', brandName);
    if (modelId) params = params.append('modelId', modelId);
    if (modelName) params = params.append('modelName', modelName);
    this._fetchingCategory = this._httpClient
      .get<TypedEnvelope<CategoryResponse>>(this._apiUrl, { params })
      .pipe(
        finalize((): void => (this._fetchingCategory = undefined)),
        shareReplay({ bufferSize: 1, refCount: true }),
      );
    return this._fetchingCategory;
  }
}
