import {Injectable} from '@angular/core';
import {apiUrl} from '../api-endpoint';
import {HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {AuthenticateRequest, GivePermissionsRequest, RegisterAccountRequest} from './identity-requests';
import {AccountResponse} from './identity-responses';
import {Envelope, TypedEnvelope} from '../envelope';

@Injectable({
  providedIn: 'root'
})
export class IdentityApiService {
  private readonly _url: string = `${apiUrl}/identity`

  constructor(private readonly _httpClient: HttpClient) {
  }

  authenticate(password: string, email?: string | null | undefined, login?: string | null | undefined): Observable<Envelope> {
    const payload: AuthenticateRequest = { email: email, login: login, password: password };
    return this._httpClient.post<Envelope>(`${this._url}/auth`, payload, { withCredentials: true });
  }

  confirmTicket(accountId: string, ticketId: string): Observable<Envelope> {
    const params: HttpParams = new HttpParams().set('account-id', accountId).set('ticket-id', ticketId);
    return this._httpClient.get<Envelope>(`${this._url}/confirmation`, { params });
  }

  givePermissions(accountId: string, permissionIds: string[]): Observable<TypedEnvelope<AccountResponse>> {
    const payload: GivePermissionsRequest = { permissionIds: permissionIds };
    const url: string = `${this._url}/account/${accountId}/permissions`;
    return this._httpClient.patch<TypedEnvelope<AccountResponse>>(url, payload);
  }

  // refreshToken(accessToken: string, refreshToken: string): Observable<Envelope> {
  //   const headers: HttpHeaders = new HttpHeaders()
  //     .set('access_token', accessToken)
  //     .set('refresh_token', refreshToken);
  //   return this._httpClient.put<Envelope>(`${this._url}/refresh`, null, { headers, withCredentials: true });
  // }

  refreshToken(): Observable<Envelope> {
    return this._httpClient.put<Envelope>(`${this._url}/refresh`, null, { withCredentials: true });
  }

  signUp(password: string, email: string, login: string): Observable<Envelope> {
    const payload: RegisterAccountRequest = { password: password, email: email, login: login };
    return this._httpClient.post<Envelope>(`${this._url}/sign-up`, payload);
  }

  // verifyToken(accessToken: string): Observable<Envelope> {
  //   const headers: HttpHeaders = new HttpHeaders().set('access_token', accessToken);
  //   return this._httpClient.post<Envelope>(`${this._url}/verify`, null, { headers, withCredentials: true });
  // }

  verifyToken(): Observable<Envelope> {
    return this._httpClient.post<Envelope>(`${this._url}/verify`, null, { withCredentials: true });
  }
}
