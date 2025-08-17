import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Spare } from '../types/Spare';
import { apiUrl } from '../../../shared/api/api-endpoint';

@Injectable({
  providedIn: 'root',
})
export class SparesService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;
  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/spares`;
  }

  public fetch(page: number, textSearch?: string | null): Observable<Spare[]> {
    let params: HttpParams = new HttpParams().set('page', page);
    if (textSearch) {
      params = new HttpParams().set('page', page).set('textSearch', textSearch);
    }
    return this._httpClient.get<Spare[]>(this._apiUrl, { params: params });
  }

  public count(textSearch?: string | null): Observable<number> {
    const requestUrl: string = `${this._apiUrl}/count`;
    let params: HttpParams = new HttpParams();
    if (textSearch) {
      params = params.set('textSearch', textSearch);
    }
    return this._httpClient.get<number>(requestUrl, { params: params });
  }

  public static default(): Spare {
    return {
      regionKind: '',
      description: '',
      city: '',
      cityId: '',
      id: '',
      isNds: false,
      photos: [],
      region: '',
      regionId: '',
      priceValue: 0,
      title: '',
      sourceUrl: '',
    };
  }
}
