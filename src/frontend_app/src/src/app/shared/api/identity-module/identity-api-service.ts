import {Injectable} from '@angular/core';
import {apiUrl} from '../api-endpoint';
import {HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';
import {finalize, Observable, shareReplay} from 'rxjs';
import {
  AuthenticateRequest,
  ChangePasswordRequest,
  GivePermissionsRequest,
  RegisterAccountRequest
} from './identity-requests';
import {AccountResponse} from './identity-responses';
import {Envelope, TypedEnvelope} from '../envelope';
import {CreateChangePasswordRequest} from './identity-factories';

@Injectable({
  providedIn: 'root'
})
export class IdentityApiService {
  private readonly _url: string = `${apiUrl}/identity`
  private _refreshInProgress: boolean = false;
  private _refresh$?: Observable<Envelope>;
  private _fetchAccountInProgress: boolean = false;
  private _fetch$?: Observable<TypedEnvelope<AccountResponse>>;
  private _logout$?: Observable<Envelope>;
  private _changePassword$?: Observable<Envelope>;

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
    if (this.fetchAccountInProgress())
      return this._fetch$!;
    return this.startFetchingAccount();
  }

  changePassword(id: string, password: string): Observable<Envelope> {
    return this.startChangingPassword(id, password);
  }

  logout(): Observable<Envelope> {
    return this.startLogout();
  }

  refreshToken(): Observable<Envelope> {
    if (this.refreshTokenInProgress())
      return this._refresh$!;
    return this.startRefreshingToken();
  }

  signUp(password: string, email: string, login: string): Observable<Envelope> {
    const payload: RegisterAccountRequest = { password: password, email: email, login: login };
    return this._httpClient.post<Envelope>(`${this._url}/sign-up`, payload);
  }

  verifyToken(): Observable<Envelope> {
    return this._httpClient.post<Envelope>(`${this._url}/verify`, null, { withCredentials: true });
  }

  private startLogout(): Observable<Envelope> {
    if (this._logout$) return this._logout$;
    const requestUrl: string = `${this._url}/logout`;
    this._logout$ = this._httpClient.post<Envelope>(requestUrl, null, { withCredentials: true })
      .pipe(finalize(() => {
        this._logout$ = undefined;
      }), shareReplay({ bufferSize: 1, refCount: true }));
    return this._logout$;
  }

  private startChangingPassword(id: string, password: string): Observable<Envelope> {
    if (this._changePassword$) return this._changePassword$;
    const requestUrl: string = `${this._url}/${id}/password`;
    const payload: ChangePasswordRequest = CreateChangePasswordRequest(password);
    this._changePassword$ = this._httpClient.patch<Envelope>(requestUrl, payload, { withCredentials: true })
      .pipe(finalize(() => {
        this._changePassword$ = undefined;
      }), shareReplay({ bufferSize: 1, refCount: true }));
    return this._changePassword$;
  }

  private startFetchingAccount(): Observable<TypedEnvelope<AccountResponse>> {
    this._fetchAccountInProgress = true;
    this._fetch$ = this._httpClient.get<TypedEnvelope<AccountResponse>>(`${this._url}/account`, { withCredentials: true })
      .pipe(shareReplay(1), finalize(() => {
        this._refreshInProgress = false;
        this._fetch$ = undefined;
      }));
    return this._fetch$;
  }

  private startRefreshingToken(): Observable<Envelope> {
    this._refreshInProgress = true;
    this._refresh$ = this._httpClient.put<Envelope>(`${this._url}/refresh`, null, { withCredentials: true })
      .pipe(shareReplay(1), finalize(() => {
        this._refreshInProgress = false;
        this._refresh$ = undefined;
      }));
    return this._refresh$;
  }

  private fetchAccountInProgress(): boolean {
    return this._fetchAccountInProgress && !!this._fetch$;
  }

  private refreshTokenInProgress(): boolean {
    return this._refreshInProgress && !!this._refresh$;
  }
}
