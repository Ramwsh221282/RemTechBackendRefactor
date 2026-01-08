import {Injectable} from '@angular/core';
import {apiUrl} from '../api-endpoint';
import {HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';
import {finalize, Observable, shareReplay} from 'rxjs';
import {AuthenticateRequest, GivePermissionsRequest, RegisterAccountRequest} from './identity-requests';
import {AccountResponse} from './identity-responses';
import {Envelope, TypedEnvelope} from '../envelope';

@Injectable({
  providedIn: 'root'
})
export class IdentityApiService {
  private readonly _url: string = `${apiUrl}/identity`
  private _refreshInProgress: boolean = false;
  private _refresh$?: Observable<Envelope>;

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

  fetchAccount(): Observable<TypedEnvelope<AccountResponse>> {
    return this._httpClient.get<TypedEnvelope<AccountResponse>>(`${this._url}/account`, { withCredentials: true });
  }

  // refreshToken(accessToken: string, refreshToken: string): Observable<Envelope> {
  //   const headers: HttpHeaders = new HttpHeaders()
  //     .set('access_token', accessToken)
  //     .set('refresh_token', refreshToken);
  //   return this._httpClient.put<Envelope>(`${this._url}/refresh`, null, { headers, withCredentials: true });
  // }

  refreshToken(): Observable<Envelope> {
    if (this._refreshInProgress && this._refresh$) {
      return this._refresh$;
    }

    this._refreshInProgress = true;
    this._refresh$ = this._httpClient.put<Envelope>(`${this._url}/refresh`, null, { withCredentials: true })
      .pipe(shareReplay(1), finalize(() => {
        this._refreshInProgress = false;
        this._refresh$ = undefined;
      }));
    return this._refresh$;
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
