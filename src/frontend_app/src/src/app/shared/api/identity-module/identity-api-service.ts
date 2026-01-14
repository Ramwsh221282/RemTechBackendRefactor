import { Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { finalize, Observable, share, shareReplay } from 'rxjs';
import {
  AuthenticateRequest,
  ChangePasswordRequest,
  CommitPasswordResetRequest,
  GivePermissionsRequest,
  RegisterAccountRequest,
  ResetPasswordRequest,
} from './identity-requests';
import { AccountResponse } from './identity-responses';
import { Envelope, TypedEnvelope } from '../envelope';
import {
  CreateChangePasswordRequest,
  CreateCommitPasswordResetRequest,
  CreateResetPasswordRequest,
} from './identity-factories';

@Injectable({
  providedIn: 'root',
})
export class IdentityApiService {
  private readonly _url: string = `${apiUrl}/identity`;
  private _refresh$?: Observable<Envelope> | undefined;
  private _fetch$?: Observable<TypedEnvelope<AccountResponse>> | undefined;
  private _logout$?: Observable<Envelope> | undefined;
  private _changePassword$?: Observable<Envelope> | undefined;
  private _resetPassword$?: Observable<Envelope> | undefined;
  private _confirmResetPassword$?: Observable<Envelope> | undefined;

  constructor(private readonly _httpClient: HttpClient) {}

  authenticate(
    password: string,
    email?: string | null | undefined,
    login?: string | null | undefined
  ): Observable<Envelope> {
    const payload: AuthenticateRequest = {
      email: email,
      login: login,
      password: password,
    };
    return this._httpClient.post<Envelope>(`${this._url}/auth`, payload, {
      withCredentials: true,
    });
  }

  resetPassword(
    login?: string | null | undefined,
    email?: string | null | undefined
  ): Observable<Envelope> {
    return this.startResetingPassword(login, email);
  }

  confirmTicket(accountId: string, ticketId: string): Observable<Envelope> {
    const params: HttpParams = new HttpParams()
      .set('account-id', accountId)
      .set('ticket-id', ticketId);
    return this._httpClient.get<Envelope>(`${this._url}/confirmation`, {
      params,
    });
  }

  givePermissions(
    accountId: string,
    permissionIds: string[]
  ): Observable<TypedEnvelope<AccountResponse>> {
    const payload: GivePermissionsRequest = { permissionIds: permissionIds };
    const url: string = `${this._url}/account/${accountId}/permissions`;
    return this._httpClient.patch<TypedEnvelope<AccountResponse>>(url, payload);
  }

  fetchAccount(): Observable<TypedEnvelope<AccountResponse>> {
    return this.startFetchingAccount();
  }

  changePassword(
    id: string,
    newPassword: string,
    currentPassword: string
  ): Observable<Envelope> {
    return this.startChangingPassword(id, newPassword, currentPassword);
  }

  logout(): Observable<Envelope> {
    return this.startLogout();
  }

  refreshToken(): Observable<Envelope> {
    return this.startRefreshingToken();
  }

  signUp(password: string, email: string, login: string): Observable<Envelope> {
    const payload: RegisterAccountRequest = {
      password: password,
      email: email,
      login: login,
    };
    return this._httpClient.post<Envelope>(`${this._url}/sign-up`, payload);
  }

  verifyToken(): Observable<Envelope> {
    return this._httpClient.post<Envelope>(`${this._url}/verify`, null, {
      withCredentials: true,
    });
  }

  confirmPasswordReset(
    accountId: string,
    ticketId: string,
    newPassword: string
  ): Observable<Envelope> {
    return this.startConfirmingPasswordReset(accountId, ticketId, newPassword);
  }

  private startLogout(): Observable<Envelope> {
    if (this._logout$) return this._logout$;
    const requestUrl: string = `${this._url}/logout`;
    this._logout$ = this._httpClient
      .post<Envelope>(requestUrl, null, { withCredentials: true })
      .pipe(
        finalize(() => {
          this._logout$ = undefined;
        }),
        shareReplay({ bufferSize: 1, refCount: true })
      );
    return this._logout$;
  }

  private startChangingPassword(
    id: string,
    newPassword: string,
    currentPassword: string
  ): Observable<Envelope> {
    if (this._changePassword$) return this._changePassword$;
    const requestUrl: string = `${this._url}/${id}/password`;
    const payload: ChangePasswordRequest = CreateChangePasswordRequest(
      newPassword,
      currentPassword
    );
    this._changePassword$ = this._httpClient
      .patch<Envelope>(requestUrl, payload, { withCredentials: true })
      .pipe(
        finalize(() => {
          this._changePassword$ = undefined;
        }),
        shareReplay({ bufferSize: 1, refCount: true })
      );
    return this._changePassword$;
  }

  private startFetchingAccount(): Observable<TypedEnvelope<AccountResponse>> {
    if (this._fetch$) return this._fetch$;
    this._fetch$ = this._httpClient
      .get<TypedEnvelope<AccountResponse>>(`${this._url}/account`, {
        withCredentials: true,
      })
      .pipe(
        shareReplay(1),
        finalize((): void => (this._fetch$ = undefined))
      );
    return this._fetch$;
  }

  private startConfirmingPasswordReset(
    accountId: string,
    ticketId: string,
    newPassword: string
  ): Observable<Envelope> {
    if (this._confirmResetPassword$) return this._confirmResetPassword$;
    const requestUrl: string = `${this._url}/accounts/${accountId}/tickets/${ticketId}/commit-password-reset`;
    const payload: CommitPasswordResetRequest =
      CreateCommitPasswordResetRequest(newPassword);
    this._confirmResetPassword$ = this._httpClient
      .post<Envelope>(requestUrl, payload)
      .pipe(
        shareReplay(1),
        finalize((): void => (this._confirmResetPassword$ = undefined))
      );
    return this._confirmResetPassword$;
  }

  private startRefreshingToken(): Observable<Envelope> {
    if (this._refresh$) return this._refresh$;
    this._refresh$ = this._httpClient
      .put<Envelope>(`${this._url}/refresh`, null, { withCredentials: true })
      .pipe(
        finalize((): void => (this._refresh$ = undefined)),
        shareReplay(1)
      );
    return this._refresh$;
  }

  private startResetingPassword(
    login?: string | null | undefined,
    email?: string | null | undefined
  ): Observable<Envelope> {
    if (this._resetPassword$) return this._resetPassword$;
    const requestUrl: string = `${this._url}/reset-password`;
    const payload: ResetPasswordRequest = CreateResetPasswordRequest(
      login,
      email
    );
    this._resetPassword$ = this._httpClient
      .post<Envelope>(requestUrl, payload)
      .pipe(
        finalize((): void => (this._resetPassword$ = undefined)),
        shareReplay({ bufferSize: 1, refCount: true })
      );
    return this._resetPassword$;
  }
}
