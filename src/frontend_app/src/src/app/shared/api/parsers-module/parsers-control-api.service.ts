import {Injectable} from '@angular/core';
import {apiUrl} from '../api-endpoint';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {finalize, Observable, retry, shareReplay, tap} from 'rxjs';
import {TypedEnvelope} from '../envelope';
import {ParserResponse} from './parsers-responses';

@Injectable({ providedIn: 'root' })
export class ParsersControlApiService {
  private readonly _apiUrl = `${apiUrl}/parsers`
  private _parsersFetch$?: Observable<TypedEnvelope<ParserResponse[]>>;
  private _parsersFetchInProgress: boolean = false;

  constructor(private readonly _httpClient: HttpClient) {
  }

  public fetchParsers(): Observable<TypedEnvelope<ParserResponse[]>> {
    if (this.isFetchingParsers())
      return this._parsersFetch$!;
    return this.startFetchingParsers();
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
        }),
        tap({
          error: (error: HttpErrorResponse): void => {
            if ([401].includes(error.status)) {
              retry({ count: 3, delay: 1000 })
            }
          }
        }));
    return this._parsersFetch$;
  }
}
