import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { apiUrl } from '../../../shared/api/api-endpoint';
import { Observable } from 'rxjs';
import { QueryBrandResponse } from '../types/QueryBrandResponse';

@Injectable({
  providedIn: 'root',
})
export class AllBrandsService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;
  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/brands`;
  }

  public fetchCount(text?: string | null): Observable<number> {
    const requestUrl: string = `${this._apiUrl}/count`;
    let params: HttpParams = new HttpParams();
    if (text) {
      params = params.set('text', text);
    }
    return this._httpClient.get<number>(requestUrl, { params: params });
  }

  public fetchBrands(
    page: number,
    text?: string | null,
  ): Observable<QueryBrandResponse[]> {
    const requestUrl: string = `${this._apiUrl}/all`;
    let params: HttpParams = new HttpParams().set('page', page);
    if (text) {
      params = params.set('text', text);
    }
    return this._httpClient.get<QueryBrandResponse[]>(requestUrl, {
      params: params,
    });
  }
}
