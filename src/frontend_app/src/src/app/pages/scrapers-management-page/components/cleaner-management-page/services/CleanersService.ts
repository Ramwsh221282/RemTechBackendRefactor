import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { apiUrl } from '../../../../../shared/api/api-endpoint';
import { Observable } from 'rxjs';
import { Cleaner } from '../types/Cleaner';

@Injectable({
  providedIn: 'root',
})
export class CleanersService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;
  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/cleaners`;
  }

  public fetch(): Observable<Cleaner> {
    return this._httpClient.get<Cleaner>(this._apiUrl);
  }

  public static default(): Cleaner {
    return {
      id: '',
      cleanedAmount: 0,
      lastRun: new Date(),
      nextRun: new Date(),
      state: '',
      waitDays: 0,
      hours: 0,
      minutes: 0,
      seconds: 0,
      itemsThreshold: 0,
    };
  }

  public changeThreshold(threshold: number): Observable<Cleaner> {
    const requestUrl: string = `${this._apiUrl}/treshold`;
    const body = { Threshold: threshold };
    return this._httpClient.patch<Cleaner>(requestUrl, body);
  }

  public changeWaitDays(waitDays: number): Observable<Cleaner> {
    const requestUrl: string = `${this._apiUrl}/wait-days`;
    const body = { Days: waitDays };
    return this._httpClient.patch<Cleaner>(requestUrl, body);
  }

  public enable(): Observable<Cleaner> {
    const requestUrl: string = `${this._apiUrl}/enabled`;
    return this._httpClient.patch<Cleaner>(requestUrl, null);
  }

  public instantlyEnable(): Observable<Cleaner> {
    const requestUrl: string = `${this._apiUrl}/instant`;
    return this._httpClient.patch<Cleaner>(requestUrl, null);
  }

  public disable(): Observable<Cleaner> {
    const requestUrl: string = `${this._apiUrl}/disabled`;
    return this._httpClient.patch<Cleaner>(requestUrl, null);
  }
}
