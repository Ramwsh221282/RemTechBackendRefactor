import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { apiUrl } from '../../../shared/api/api-endpoint';
import { Observable } from 'rxjs';
import { QueryCategoriesResponse } from '../types/QueryCategoriesResponse';

@Injectable({
  providedIn: 'root',
})
export class AllCategoriesService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;
  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/categories`;
  }

  public fetchCount(text?: string | null): Observable<number> {
    let params: HttpParams = new HttpParams();
    if (text) {
      params = params.set('text', text);
    }
    const requestUrl: string = `${this._apiUrl}/count`;
    return this._httpClient.get<number>(requestUrl, { params: params });
  }

  public fetchCategories(
    page: number,
    text?: string | null,
  ): Observable<QueryCategoriesResponse[]> {
    let params: HttpParams = new HttpParams().set('page', page);
    if (text) {
      params = params.set('text', text);
    }
    const requestUrl: string = `${this._apiUrl}/all`;
    return this._httpClient.get<QueryCategoriesResponse[]>(requestUrl, {
      params: params,
    });
  }
}
