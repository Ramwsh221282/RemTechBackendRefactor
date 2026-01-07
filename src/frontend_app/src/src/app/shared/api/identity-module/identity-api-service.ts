import {Injectable} from '@angular/core';
import {apiUrl} from '../api-endpoint';
import {HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {AuthenticateRequest, GivePermissionsRequest, RegisterAccountRequest} from './identity-requests';
import {AccountResponse} from './identity-responses';

@Injectable({
  providedIn: 'root'
})
export class IdentityApiService {
  private readonly _url: string = `${apiUrl}/identity`

  constructor(private readonly _httpClient: HttpClient) {
  }

  authenticate(password: string, email?: string | null | undefined, login?: string | null | undefined): Observable<any> {
    const payload: AuthenticateRequest = { email: email, login: login, password: password };
    return this._httpClient.post(`${this._url}/auth`, payload);
  }

  confirmTicket(accountId: string, ticketId: string): Observable<any> {
    const params: HttpParams = new HttpParams().set('account-id', accountId).set('ticket-id', ticketId);
    return this._httpClient.get(`${this._url}/confirmation`, { params });
  }

  givePermissions(accountId: string, permissionIds: string[]): Observable<AccountResponse> {
    const payload: GivePermissionsRequest = { permissionIds: permissionIds };
    const url: string = `${this._url}/account/${accountId}/permissions`;
    return this._httpClient.patch<AccountResponse>(url, payload);
  }

  refreshToken(accessToken: string, refreshToken: string): Observable<any> {
    const headers: HttpHeaders = new HttpHeaders()
      .set('access_token', accessToken)
      .set('refresh_token', refreshToken);
    return this._httpClient.put(`${this._url}/refresh`, null, { headers });
  }

  signUp(password: string, email: string, login: string): Observable<any> {
    const payload: RegisterAccountRequest = { password: password, email: email, login: login };
    return this._httpClient.post(`${this._url}/sign-up`, payload);
  }

  verifyToken(accessToken: string): Observable<any> {
    const headers: HttpHeaders = new HttpHeaders().set('access_token', accessToken);
    return this._httpClient.post(`${this._url}/verify`, null, { headers });
  }
}
