import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import { ModelResponse } from './models-responses';

@Injectable({
  providedIn: 'root',
})
export class ModelsApiService {
  private readonly _httpClient: HttpClient = inject(HttpClient);
  private readonly _apiUrl: string = `${apiUrl}/models`;
  private fetchingModels$: Observable<ModelResponse[]> | undefined;

  public fetchModels(
    id?: string | null | undefined,
    name?: string | null | undefined,
    brandId?: string | null | undefined,
    brandName?: string | null | undefined,
    categoryId?: string | null | undefined,
    categoryName?: string | null | undefined,
  ): Observable<ModelResponse[]> {
    return this.invokeFetchingModels(
      id,
      name,
      brandId,
      brandName,
      categoryId,
      categoryName,
    );
  }

  private invokeFetchingModels(
    id?: string | null | undefined,
    name?: string | null | undefined,
    brandId?: string | null | undefined,
    brandName?: string | null | undefined,
    categoryId?: string | null | undefined,
    categoryName?: string | null | undefined,
  ): Observable<ModelResponse[]> {
    if (this.fetchingModels$) return this.fetchingModels$;
    let params: HttpParams = new HttpParams();
    if (id) params = params.append('id', id);
    if (name) params = params.append('name', name);
    if (brandId) params = params.append('brandId', brandId);
    if (brandName) params = params.append('brandName', brandName);
    if (categoryId) params = params.append('categoryId', categoryId);
    if (categoryName) params = params.append('categoryName', categoryName);
    this.fetchingModels$ = this._httpClient
      .get<TypedEnvelope<ModelResponse[]>>(this._apiUrl, { params })
      .pipe(
        map((envelope): ModelResponse[] => {
          let response: ModelResponse[] = [];
          if (envelope.body) response = envelope.body;
          return response;
        }),
        finalize((): void => (this.fetchingModels$ = undefined)),
        shareReplay({ refCount: true, bufferSize: 1 }),
      );
    return this.fetchingModels$;
  }
}
