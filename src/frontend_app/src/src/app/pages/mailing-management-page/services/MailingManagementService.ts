import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { apiUrl } from '../../../shared/api/api-endpoint';
import { Observable } from 'rxjs';
import { MailingSender } from '../models/MailingSender';
import { TokensService } from '../../../shared/services/TokensService';

@Injectable({
  providedIn: 'root',
})
export class MailingManagementService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;
  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/mailing`;
  }
  public create(email: string, password: string): Observable<MailingSender> {
    const body: object = { email: email, password: password };
    return this._httpClient.post<MailingSender>(this._apiUrl, {
      body,
    });
  }
  public read(): Observable<MailingSender[]> {
    return this._httpClient.get<MailingSender[]>(this._apiUrl, {});
  }
  public remove(email: string): Observable<MailingSender> {
    let params: HttpParams = new HttpParams().set('email', email);
    return this._httpClient.delete<MailingSender>(this._apiUrl, {
      params: params,
    });
  }

  public ping(sender: MailingSender, to: string): Observable<MailingSender> {
    const body: object = { email: sender.email, to: to };
    const requestUrl: string = `${this._apiUrl}/ping`;
    return this._httpClient.post<MailingSender>(requestUrl, {
      body,
    });
  }
}
