import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { apiUrl } from '../../../shared/api/api-endpoint';
import { Observable } from 'rxjs';
import { StringUtils } from '../../../shared/utils/string-utils';
import { TokensService } from '../../../shared/services/TokensService';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;

  constructor(
    httpClient: HttpClient,
    private readonly tokensService: TokensService,
  ) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/users`;
  }

  public register(
    email: string,
    password: string,
    name: string,
  ): Observable<any> {
    const requestUrl = `${this._apiUrl}/sign-up`;
    const body = { email: email, password: password, name: name };
    return this._httpClient.post(requestUrl, body);
  }

  public signOut(): Observable<any> {
    const requestUrl = `${this._apiUrl}/sign-out`;
    return this._httpClient.get(requestUrl);
  }

  public verify(token: string): Observable<any> {
    const requestUrl = `${this._apiUrl}/verify`;
    const headers = new HttpHeaders().set('RemTechAccessTokenId', token);
    return this._httpClient.get<any>(requestUrl, { headers: headers });
  }

  public checkRoot(): Observable<boolean> {
    const requestUrl = `${this._apiUrl}/root-get`;
    return this._httpClient.get<boolean>(requestUrl);
  }

  public refreshSession(): Observable<any> {
    const requestUrl = `${this._apiUrl}/refresh-session`;
    let headers: HttpHeaders = new HttpHeaders();
    headers = this.tokensService.addToken(headers);
    return this._httpClient.get<any>(requestUrl, { headers: headers });
  }

  public upRoot(
    email: string,
    password: string,
    name: string,
  ): Observable<any> {
    const requestUrl = `${this._apiUrl}/root-up`;
    const body = { email: email, password: password, name: name };
    return this._httpClient.post(requestUrl, body);
  }

  public verifyAdminAccess(tokenId: string): Observable<any> {
    const requestUrl = `${this._apiUrl}/verify-admin`;
    const headers: HttpHeaders = new HttpHeaders().set(
      'RemTechAccessTokenId',
      tokenId,
    );
    return this._httpClient.get<any>(requestUrl, { headers: headers });
  }

  public authenticate(
    password: string,
    email?: string | null,
    name?: string | null,
  ): Observable<any> {
    const requestUrl: string = `${this._apiUrl}/sign-in`;
    const headers: HttpHeaders = new HttpHeaders().set('password', password);
    let params: HttpParams = new HttpParams();
    if (email && !StringUtils.isEmptyOrWhiteSpace(email))
      params = params.set('email', email);
    if (name && !StringUtils.isEmptyOrWhiteSpace(name))
      params = params.set('name', name);
    return this._httpClient.post<Observable<any>>(requestUrl, null, {
      headers: headers,
      params: params,
    });
  }
}
