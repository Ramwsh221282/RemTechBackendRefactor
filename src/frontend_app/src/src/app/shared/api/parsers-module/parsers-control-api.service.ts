import {Injectable} from '@angular/core';
import {apiUrl} from '../api-endpoint';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {TypedEnvelope} from '../envelope';
import {ParserResponse} from './parsers-responses';

@Injectable({ providedIn: 'root' })
export class ParsersControlApiService {
  private readonly _apiUrl = `${apiUrl}/parsers`

  constructor(private readonly _httpClient: HttpClient) {
  }

  public fetchParsers(): Observable<TypedEnvelope<ParserResponse[]>> {
    return this._httpClient.get<TypedEnvelope<ParserResponse[]>>(this._apiUrl, { withCredentials: true });
  }
}
