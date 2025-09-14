import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PopularBrand } from '../types/PopularBrand';
import { apiUrl } from '../../../shared/api/api-endpoint';

@Injectable({
  providedIn: 'root',
})
export class PopularBrandsService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;
  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/brands`;
  }

  public fetch(): Observable<PopularBrand[]> {
    const requestUrl: string = `${this._apiUrl}/popular`;
    return this._httpClient.get<PopularBrand[]>(requestUrl);
  }
}
