import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { apiUrl } from '../../../shared/api/api-endpoint';
import { Observable } from 'rxjs';
import { PopularCategory } from '../types/PopularCategories';

@Injectable({
  providedIn: 'root',
})
export class PopularCategoriesService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;
  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/categories`;
  }

  public fetch(): Observable<PopularCategory[]> {
    const requestUrl: string = `${this._apiUrl}/popular`;
    return this._httpClient.get<PopularCategory[]>(requestUrl);
  }
}
