import {Injectable} from '@angular/core';
import {apiUrl} from '../api-endpoint';
import {HttpClient} from '@angular/common/http';
import {finalize, Observable, shareReplay} from 'rxjs';
import {TypedEnvelope} from '../envelope';
import {ParserResponse} from './parsers-responses';

@Injectable({ providedIn: 'root' })
export class ParsersControlApiService {
  private readonly _apiUrl: string = `${apiUrl}/parsers`
  private _parsersFetch$?: Observable<TypedEnvelope<ParserResponse[]>>;
  private _parsersFetchInProgress: boolean = false;
  private _parserFetch$?: Observable<TypedEnvelope<ParserResponse>>;
  private _parserFetchInProgress: boolean = false;

  constructor(private readonly _httpClient: HttpClient) {
  }

  public fetchParsers(): Observable<TypedEnvelope<ParserResponse[]>> {
    if (this.isFetchingParsers())
      return this._parsersFetch$!;
    return this.startFetchingParsers();
  }

  public fetchParser(id: string): Observable<TypedEnvelope<ParserResponse>> {
    if (this.isFetchingParser())
      return this._parserFetch$!;
    return this.startFetchingParser(id);
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

  private isFetchingParser(): boolean {
    return this._parserFetchInProgress && !!this._parserFetch$;
  }

  private isFetchingParsers(): boolean {
    return this._parsersFetchInProgress && !!this._parsersFetch$;
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
}
