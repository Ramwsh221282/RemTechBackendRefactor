import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { apiUrl } from '../../../../../shared/api/api-endpoint';
import { Scraper } from '../types/Scraper';
import { Observable } from 'rxjs';
import { ParserStateToChangeRequest } from '../types/ParserStateToChangeRequest';
import { ParserStateChangeResult } from '../types/ParserStateChangedResult';
import { ParserWaitDaysUpdateRequest } from '../types/ParserWaitDaysUpdateRequest';
import { ParserWaitDaysUpdateResult } from '../types/ParserWaitDaysUpdateResult';
import { CreateNewParserLinkRequest } from '../types/CreateNewParserLinkRequest';
import { CreateNewParserLinkResponse } from '../types/CreateNewParserLinkResponse';
import { RemoveParserLinkRequest } from '../types/RemoveParserLinkRequest';
import { RemoveParserLinkResponse } from '../types/RemoveParserLinkResponse';
import { ScraperLink } from '../types/ScraperLink';
import { UpdateParserLinkEndpointResponse } from '../types/UpdateParserLinkEndpointResponse';
import { UpdateParserLinkEndpointRequest } from '../types/UpdateParserLinkEndpointRequest';
import { LinkWithChangedActivityResponse } from '../types/LinkWithChangedActivityResponse';
import { LinkWithChangedActivityRequest } from '../types/LinkWithChangedActivityRequest';
import { InstantlyEnabledParserResponse } from '../types/InstantlyEnabledParserResponse';
import { TokensService } from '../../../../../shared/services/TokensService';
import { ScraperJournalResponse } from '../../scrapers-management-journals-page/types/ScraperJournalResponse';
import { ScraperJournalRecordResponse } from '../../scrapers-management-journals-page/types/ScraperJournalRecordResponse';

@Injectable({
  providedIn: 'root',
})
export class VehicleScrapersService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;

  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/scrapers`;
  }

  public fetch(): Observable<Scraper[]> {
    return this._httpClient.get<Scraper[]>(this._apiUrl, {});
  }

  public fetchConcrete(name: string, type: string): Observable<Scraper> {
    const requestUrl = `${this._apiUrl}/${name}/${type}`;
    return this._httpClient.get<Scraper>(requestUrl, {});
  }

  public changeState(
    parser: Scraper,
    request: ParserStateToChangeRequest,
  ): Observable<ParserStateChangeResult> {
    const requestUrl = `${this._apiUrl}/state`;
    let params: HttpParams = new HttpParams()
      .set('name', parser.name)
      .set('type', parser.type);
    return this._httpClient.patch<ParserStateChangeResult>(
      requestUrl,
      request,
      { params: params },
    );
  }

  public changeWaitDays(
    scraper: Scraper,
    request: ParserWaitDaysUpdateRequest,
  ): Observable<ParserWaitDaysUpdateResult> {
    const requestUrl = `${this._apiUrl}/wait-days`;
    let params: HttpParams = new HttpParams()
      .set('name', scraper.name)
      .set('type', scraper.type);
    return this._httpClient.patch<ParserWaitDaysUpdateResult>(
      requestUrl,
      request,
      { params: params },
    );
  }

  public addParserLink(
    scraper: Scraper,
    request: CreateNewParserLinkRequest,
  ): Observable<CreateNewParserLinkResponse> {
    const requestUrl = `${this._apiUrl}/scraper-link`;
    let params: HttpParams = new HttpParams()
      .set('name', scraper.name)
      .set('type', scraper.type);
    return this._httpClient.post<CreateNewParserLinkResponse>(
      requestUrl,
      request,
      { params: params },
    );
  }

  public removeParserLink(
    scraper: Scraper,
    request: RemoveParserLinkRequest,
  ): Observable<RemoveParserLinkResponse> {
    const requestUrl = `${this._apiUrl}/scraper-link`;
    let params: HttpParams = new HttpParams()
      .set('name', scraper.name)
      .set('type', scraper.type);
    return this._httpClient.delete<RemoveParserLinkResponse>(requestUrl, {
      params: params,
      body: request,
    });
  }

  public changeLinkActivity(
    link: ScraperLink,
    request: LinkWithChangedActivityRequest,
  ): Observable<LinkWithChangedActivityResponse> {
    const requestUrl = `${this._apiUrl}/scraper-link/activity`;
    let params: HttpParams = new HttpParams()
      .set('linkName', link.name)
      .set('parserName', link.parserName)
      .set('parserType', link.parserType);
    return this._httpClient.patch<LinkWithChangedActivityResponse>(
      requestUrl,
      request,
      { params: params },
    );
  }

  public updateScraperLink(
    scraperLink: ScraperLink,
    request: UpdateParserLinkEndpointRequest,
  ): Observable<UpdateParserLinkEndpointResponse> {
    const requestUrl = `${this._apiUrl}/scraper-link`;
    let params: HttpParams = new HttpParams()
      .set('name', scraperLink.parserName)
      .set('type', scraperLink.parserType)
      .set('linkName', scraperLink.name)
      .set('linkUrl', scraperLink.url);
    return this._httpClient.put<UpdateParserLinkEndpointResponse>(
      requestUrl,
      request,
      { params: params },
    );
  }

  public enableInstantly(
    scraper: Scraper,
  ): Observable<InstantlyEnabledParserResponse> {
    const requestUrl = `${this._apiUrl}/instant`;
    let params: HttpParams = new HttpParams()
      .set('name', scraper.name)
      .set('type', scraper.type);
    return this._httpClient.patch<InstantlyEnabledParserResponse>(
      requestUrl,
      null,
      { params: params },
    );
  }

  public fetchJournals(
    name: string,
    type: string,
    page: number,
    from: Date | null,
    to: Date | null,
  ): Observable<ScraperJournalResponse[]> {
    let params: HttpParams = new HttpParams()
      .set('name', name)
      .set('type', type)
      .set('page', page);
    const body: object = {
      from: from === null ? null : from,
      to: to === null ? null : to,
    };
    const requestUrl = `${this._apiUrl}/journals`;
    return this._httpClient.post<ScraperJournalResponse[]>(requestUrl, body, {
      params: params,
    });
  }

  public fetchJournalRecordsCount(journalId: string): Observable<number> {
    const requestUrl = `${this._apiUrl}/journals/records/count`;
    const params: HttpParams = new HttpParams().set('journalId', journalId);
    return this._httpClient.get<number>(requestUrl, { params });
  }

  public fetchJournalsCount(name: string, type: string): Observable<number> {
    const requestUrl = `${this._apiUrl}/journals/count`;
    const params: HttpParams = new HttpParams()
      .set('name', name)
      .set('type', type);
    return this._httpClient.get<number>(requestUrl, { params });
  }

  public removeJournal(
    id: string,
    name: string,
    type: string,
  ): Observable<string> {
    const requestUrl = `${this._apiUrl}/journals/${id}`;
    let params: HttpParams = new HttpParams()
      .set('name', name)
      .set('type', type);
    return this._httpClient.delete<string>(requestUrl, { params });
  }

  public fetchJournalRecords(
    journalId: string,
    page: number,
    text: string | null,
  ): Observable<ScraperJournalRecordResponse[]> {
    const requestUrl = `${this._apiUrl}/journals/records`;
    let params: HttpParams = new HttpParams()
      .set('page', page)
      .set('journalId', journalId);
    if (text) params = params.set('text', text);
    return this._httpClient.get<ScraperJournalRecordResponse[]>(requestUrl, {
      params: params,
    });
  }

  public static defaultJournal(): ScraperJournalResponse {
    return {
      name: '',
      id: '',
      type: '',
      completedAt: null,
      createdAt: new Date(),
    };
  }

  public static defaultJournalRecord(): ScraperJournalRecordResponse {
    return {
      createdAt: new Date(),
      journalId: '',
      id: '',
      action: '',
      text: '',
    };
  }

  public static defaultScraper(): Scraper {
    return {
      name: '',
      domain: '',
      hours: 0,
      lastRun: new Date(),
      nextRun: new Date(),
      links: [],
      minutes: 0,
      processed: 0,
      seconds: 0,
      state: '',
      totalSeconds: 0,
      type: '',
      waitDays: 0,
    };
  }

  public static defaultScraperLink(): ScraperLink {
    return {
      name: '',
      hours: 0,
      url: '',
      totalSeconds: 0,
      seconds: 0,
      processed: 0,
      parserName: '',
      parserType: '',
      minutes: 0,
      activity: false,
    };
  }
}
