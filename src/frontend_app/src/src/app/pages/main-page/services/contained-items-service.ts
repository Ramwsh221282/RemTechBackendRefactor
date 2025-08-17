import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { apiUrl } from '../../../shared/api/api-endpoint';
import { Observable } from 'rxjs';
import { GetContainedItemsByTypeResponse } from '../types/GetContainedItemsByTypeResponse';
import { SomeRecentItem } from '../types/SomeRecentItem';

@Injectable({
  providedIn: 'root',
})
export class ContainedItemsService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;
  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/contained-items`;
  }

  public fetchCount(): Observable<GetContainedItemsByTypeResponse[]> {
    return this._httpClient.get<GetContainedItemsByTypeResponse[]>(
      `${this._apiUrl}/count`,
    );
  }

  public fetchRecent(page: number): Observable<SomeRecentItem[]> {
    const requestUrl = `${this._apiUrl}/recent`;
    let params: HttpParams = new HttpParams().set('page', page);
    return this._httpClient.get<SomeRecentItem[]>(requestUrl, {
      params: params,
    });
  }

  public fetchRecentCount(): Observable<number> {
    const requestUrl = `${this._apiUrl}/recent/count`;
    return this._httpClient.get<number>(requestUrl);
  }

  public static default(): SomeRecentItem {
    return {
      city: '',
      id: '',
      description: '',
      IsNds: false,
      photos: [],
      price: 0,
      region: '',
      regionKind: '',
      sourceUrl: '',
    };
  }
}
