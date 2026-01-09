import {Injectable} from '@angular/core';
import {apiUrl} from '../api-endpoint';
import {HttpClient} from '@angular/common/http';
import {finalize, Observable, shareReplay} from 'rxjs';
import {TypedEnvelope} from '../envelope';
import {ParserLinkResponse, ParserResponse} from './parsers-responses';
import {AddLinksToParserRequest, AddLinksToParserRequestBody} from './parsers-requests';

@Injectable({ providedIn: 'root' })
export class ParsersControlApiService {
  private readonly _apiUrl: string = `${apiUrl}/parsers`
  private _parsersFetch$?: Observable<TypedEnvelope<ParserResponse[]>>;
  private _parsersFetchInProgress: boolean = false;
  private _parserFetch$?: Observable<TypedEnvelope<ParserResponse>>;
  private _parserFetchInProgress: boolean = false;
  private _changeWaitDaysInProgress: boolean = false;
  private _changeWaitDays$?: Observable<TypedEnvelope<ParserResponse>>;
  private _isAddingLinks: boolean = false;
  private _addLinks$?: Observable<TypedEnvelope<ParserLinkResponse[]>>;
  private _isChangingLinkActivity: boolean = false;
  private _changeLinkActivity$?: Observable<TypedEnvelope<ParserLinkResponse>>;

  constructor(private readonly _httpClient: HttpClient) {
  }

  public fetchParsers(): Observable<TypedEnvelope<ParserResponse[]>> {
    if (this.isFetchingParsers())
      return this._parsersFetch$!;
    return this.startFetchingParsers();
  }

  public addLinksToParser(parserId: string, links: { name: string, url: string }[]): Observable<TypedEnvelope<ParserLinkResponse[]>> {
    if (this.isAddingLinks())
      return this._addLinks$!;
    return this.startAddingLinks(parserId, links);
  }

  public fetchParser(id: string): Observable<TypedEnvelope<ParserResponse>> {
    if (this.isFetchingParser())
      return this._parserFetch$!;
    return this.startFetchingParser(id);
  }

  public changeWaitDays(id: string, days: number): Observable<TypedEnvelope<ParserResponse>> {
    if (this.isChangingWaitDays())
      return this._changeWaitDays$!;
    return this.startChangingWaitDays(id, days);
  }

  public changeLinkActivity(parserId: string, linkId: string, activity: boolean): Observable<TypedEnvelope<ParserLinkResponse>> {
    if (this.isChangingLinkActivity())
      return this._changeLinkActivity$!;
    return this.startChangingLinkActivity(parserId, linkId, activity);
  }

  private startFetchingParser(id: string): Observable<TypedEnvelope<ParserResponse>> {
    this._parserFetchInProgress = true;
    this._parserFetch$ = this._httpClient.get<TypedEnvelope<ParserResponse>>(`${this._apiUrl}/${id}`, { withCredentials: true })
      .pipe(
        shareReplay(1),
        finalize((): void => {
          this._parserFetch$ = undefined;
          this._parserFetchInProgress = false;
        }));
    return this._parserFetch$;
  }

  private startChangingWaitDays(id: string, days: number): Observable<TypedEnvelope<ParserResponse>> {
    this._changeWaitDaysInProgress = true;
    const requestUrl: string = `${this._apiUrl}/${id}/wait-days?value=${days}`;
    this._changeWaitDays$ = this._httpClient.patch<TypedEnvelope<ParserResponse>>(requestUrl, null, { withCredentials: true })
      .pipe(
        finalize(() => {
          this._changeWaitDays$ = undefined;
          this._changeWaitDaysInProgress = false;
        }),
        shareReplay({ bufferSize: 1, refCount: true }),
      );

    return this._changeWaitDays$;
  }

  private startFetchingParsers(): Observable<TypedEnvelope<ParserResponse[]>> {
    this._parsersFetchInProgress = true;
    this._parsersFetch$ = this._httpClient.get<TypedEnvelope<ParserResponse[]>>(this._apiUrl, { withCredentials: true })
      .pipe(
        shareReplay(1),
        finalize((): void => {
          this._parsersFetchInProgress = false;
          this._parsersFetch$ = undefined;
        }));
    return this._parsersFetch$;
  }

  private startAddingLinks(parserId: string, links: { name: string, url: string }[]): Observable<TypedEnvelope<ParserLinkResponse[]>> {
    this._isAddingLinks = true;
    const requestUrl: string = `${this._apiUrl}/${parserId}/links`;
    const bodyLinksPayload: AddLinksToParserRequestBody[] = links.map((l): AddLinksToParserRequestBody => ({ Name: l.name, Url: l.url }));
    const body: AddLinksToParserRequest = { Links: bodyLinksPayload };
    this._addLinks$ = this._httpClient.post<TypedEnvelope<ParserLinkResponse[]>>(requestUrl, body, { withCredentials: true })
      .pipe(
        finalize(() => {
          this._isAddingLinks = false;
          this._addLinks$ = undefined;
        }),
        shareReplay({ bufferSize: 1, refCount: true }));
    return this._addLinks$;
  }

  private startChangingLinkActivity(parserId: string, linkId: string, activity: boolean): Observable<TypedEnvelope<ParserLinkResponse>> {
    this._isChangingLinkActivity = true;
    const requestUrl: string = `${this._apiUrl}/${parserId}/links/${linkId}/activity?value=${activity}`
    this._changeLinkActivity$ = this._httpClient.patch<TypedEnvelope<ParserLinkResponse>>(requestUrl, null, { withCredentials: true })
      .pipe(
        finalize(() => {
          this._changeLinkActivity$ = undefined;
          this._isChangingLinkActivity = false;
        }),
        shareReplay({ bufferSize: 1, refCount: true })
      )
    return this._changeLinkActivity$;
  }

  private isChangingLinkActivity(): boolean {
    return this._isChangingLinkActivity && !!this._changeLinkActivity$;
  }

  private isAddingLinks(): boolean {
    return this._isAddingLinks && !!this._addLinks$;
  }

  private isChangingWaitDays(): boolean {
    return this._changeWaitDaysInProgress && !!this._changeWaitDays$;
  }

  private isFetchingParser(): boolean {
    return this._parserFetchInProgress && !!this._parserFetch$;
  }

  private isFetchingParsers(): boolean {
    return this._parsersFetchInProgress && !!this._parsersFetch$;
  }
}
