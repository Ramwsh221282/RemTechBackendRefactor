import {inject, Injectable} from '@angular/core';
import {finalize, Observable, shareReplay} from 'rxjs';
import {TypedEnvelope} from '../envelope';
import {MailerResponse} from './notifications-responses';
import {apiUrl} from '../api-endpoint';
import {HttpClient} from '@angular/common/http';
import {AddMailerRequest, ChangeMailerRequest} from './notifications-requests';

@Injectable({
  providedIn: 'root'
})
export class NotificationsApiService {

  public fetchMailers(): Observable<TypedEnvelope<MailerResponse[]>> {
    return this.startFetchingMailers();
  }

  public fetchMailer(id: string): Observable<TypedEnvelope<MailerResponse>> {
    return this.startFetchingMailer(id);
  }

  public addMailer(email: string, password: string): Observable<TypedEnvelope<MailerResponse>> {
    return this.startAddingMailer(email, password);
  }

  public changeMailer(id: string, email: string, password: string): Observable<TypedEnvelope<MailerResponse>> {
    return this.startChangingMailer(id, email, password);
  }

  private startFetchingMailers(): Observable<TypedEnvelope<MailerResponse[]>> {
    if (this._gettingMailers$) return this._gettingMailers$;
    this._gettingMailers$ = this._httpClient.get<TypedEnvelope<MailerResponse[]>>(this._apiUrl, { withCredentials: true })
      .pipe(
      finalize((): void => this._gettingMailers$ = undefined),
        shareReplay({ bufferSize: 1, refCount: true }),
        );
    return this._gettingMailers$;
  }

  private startFetchingMailer(id: string): Observable<TypedEnvelope<MailerResponse>> {
    if (this._gettingMailer$) return this._gettingMailer$;
    this._gettingMailer$ = this._httpClient.get<TypedEnvelope<MailerResponse>>(`${this._apiUrl}/${id}`, { withCredentials: true })
      .pipe(
        finalize((): void => this._gettingMailer$ = undefined),
        shareReplay({ bufferSize: 1, refCount: true }),
      );
    return this._gettingMailer$;
  }

  private startAddingMailer(email: string, password: string): Observable<TypedEnvelope<MailerResponse>> {
    if (this._addingMailer$) return this._addingMailer$;
    const request: AddMailerRequest = { Email: email, SmtpPassword: password };
    this._addingMailer$ = this._httpClient.post<TypedEnvelope<MailerResponse>>(this._apiUrl, request, { withCredentials: true })
      .pipe(
        finalize((): void => this._addingMailer$ = undefined),
        shareReplay({ bufferSize: 1, refCount: true }),
      );
    return this._addingMailer$;
  }

  private startChangingMailer(id: string, email: string, password: string): Observable<TypedEnvelope<MailerResponse>> {
    if (this._changingMailer$) return this._changingMailer$;
    const request: ChangeMailerRequest = { Email: email, SmtpPassword: password };
    this._changingMailer$ = this._httpClient.put<TypedEnvelope<MailerResponse>>(`${this._apiUrl}/${id}`, request, { withCredentials: true })
      .pipe(
        finalize((): void => this._changingMailer$ = undefined),
        shareReplay({ bufferSize: 1, refCount: true }),
      );
    return this._changingMailer$;
  }

  private readonly _httpClient: HttpClient = inject(HttpClient);
  private readonly _apiUrl: string = `${apiUrl}/mailers`;
  private _gettingMailers$: Observable<TypedEnvelope<MailerResponse[]>> | undefined;
  private _gettingMailer$: Observable<TypedEnvelope<MailerResponse>> | undefined;
  private _addingMailer$: Observable<TypedEnvelope<MailerResponse>> | undefined;
  private _changingMailer$: Observable<TypedEnvelope<MailerResponse>> | undefined;
}
