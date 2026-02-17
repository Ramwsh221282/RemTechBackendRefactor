import { Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { HttpClient, HttpParams } from '@angular/common/http';
import { finalize, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import { ParserLinkResponse, ParserResponse } from './parsers-responses';
import { AddLinksToParserRequest, AddLinksToParserRequestBody } from './parsers-requests';

@Injectable({ providedIn: 'root' })
export class ParsersControlApiService {
	private readonly _apiUrl: string = `${apiUrl}/parsers`;
	private _parsersFetch$?: Observable<TypedEnvelope<ParserResponse[]>>;
	private _parserFetch$?: Observable<TypedEnvelope<ParserResponse>>;
	private _changeWaitDays$?: Observable<TypedEnvelope<ParserResponse>>;
	private _addLinks$?: Observable<TypedEnvelope<ParserLinkResponse[]>>;
	private _changeLinkActivity$?: Observable<TypedEnvelope<ParserLinkResponse>>;
	private _removeLink$?: Observable<TypedEnvelope<ParserLinkResponse>>;
	private _updateLink$?: Observable<TypedEnvelope<ParserLinkResponse>>;
	private _isPermantlyStartingParser$?: Observable<TypedEnvelope<ParserResponse>>;
	private _isEnablingParser$?: Observable<TypedEnvelope<ParserResponse>>;
	private _isDisablingParser$?: Observable<TypedEnvelope<ParserResponse>>;
	private _isPermanentlyDisablingParser$?: Observable<TypedEnvelope<ParserResponse>>;

	constructor(private readonly _httpClient: HttpClient) {}

	public fetchParsers(): Observable<TypedEnvelope<ParserResponse[]>> {
		return this.startFetchingParsers();
	}

	public permantlyStartParser(parserId: string): Observable<TypedEnvelope<ParserResponse>> {
		return this.startPermantlyStartParser(parserId);
	}

	public enableParser(parserId: string): Observable<TypedEnvelope<ParserResponse>> {
		return this.startEnablingParser(parserId);
	}

	public disableParser(parserId: string): Observable<TypedEnvelope<ParserResponse>> {
		return this.startDisablingParser(parserId);
	}

	public permantlyDisableParser(parserId: string): Observable<TypedEnvelope<ParserResponse>> {
		return this.startPermantlyDisablingParser(parserId);
	}

	public removeLinkFromParser(parserId: string, linkId: string): Observable<TypedEnvelope<ParserLinkResponse>> {
		return this.startRemovingLink(parserId, linkId);
	}

	public addLinksToParser(parserId: string, links: { name: string; url: string }[]): Observable<TypedEnvelope<ParserLinkResponse[]>> {
		return this.startAddingLinks(parserId, links);
	}

	public updateLink(
		parserId: string,
		linkId: string,
		name?: string | null,
		url?: string | null,
	): Observable<TypedEnvelope<ParserLinkResponse>> {
		return this.startUpdatingLink(parserId, linkId, name, url);
	}

	public fetchParser(id: string): Observable<TypedEnvelope<ParserResponse>> {
		return this.startFetchingParser(id);
	}

	public changeWaitDays(id: string, days: number): Observable<TypedEnvelope<ParserResponse>> {
		return this.startChangingWaitDays(id, days);
	}

	public changeLinkActivity(parserId: string, linkId: string, activity: boolean): Observable<TypedEnvelope<ParserLinkResponse>> {
		return this.startChangingLinkActivity(parserId, linkId, activity);
	}

	private startPermantlyDisablingParser(id: string): Observable<TypedEnvelope<ParserResponse>> {
		if (this._isPermanentlyDisablingParser$) return this._isPermanentlyDisablingParser$;
		const requestUrl: string = `${this._apiUrl}/${id}/permantly-disable`;
		this._isPermanentlyDisablingParser$ = this._httpClient
			.patch<TypedEnvelope<ParserResponse>>(requestUrl, null, { withCredentials: true })
			.pipe(
				finalize((): void => (this._isPermanentlyDisablingParser$ = undefined)),
				shareReplay({ bufferSize: 1, refCount: true }),
			);
		return this._isPermanentlyDisablingParser$;
	}

	private startDisablingParser(id: string): Observable<TypedEnvelope<ParserResponse>> {
		if (this._isDisablingParser$) return this._isDisablingParser$;
		const requestUrl: string = `${this._apiUrl}/${id}/disabled`;
		this._isDisablingParser$ = this._httpClient.patch<TypedEnvelope<ParserResponse>>(requestUrl, null, { withCredentials: true }).pipe(
			finalize((): void => (this._isDisablingParser$ = undefined)),
			shareReplay({ bufferSize: 1, refCount: true }),
		);
		return this._isDisablingParser$;
	}

	private startEnablingParser(id: string): Observable<TypedEnvelope<ParserResponse>> {
		if (this._isEnablingParser$) return this._isEnablingParser$;
		const requestUrl: string = `${this._apiUrl}/${id}/start`;
		this._isEnablingParser$ = this._httpClient.post<TypedEnvelope<ParserResponse>>(requestUrl, null, { withCredentials: true }).pipe(
			finalize((): void => (this._isEnablingParser$ = undefined)),
			shareReplay({ bufferSize: 1, refCount: true }),
		);
		return this._isEnablingParser$;
	}

	private startFetchingParser(id: string): Observable<TypedEnvelope<ParserResponse>> {
		if (this._parserFetch$) return this._parserFetch$;
		this._parserFetch$ = this._httpClient.get<TypedEnvelope<ParserResponse>>(`${this._apiUrl}/${id}`, { withCredentials: true }).pipe(
			shareReplay(1),
			finalize((): void => (this._parserFetch$ = undefined)),
		);
		return this._parserFetch$;
	}

	private startChangingWaitDays(id: string, days: number): Observable<TypedEnvelope<ParserResponse>> {
		if (this._changeWaitDays$) return this._changeWaitDays$;
		const requestUrl: string = `${this._apiUrl}/${id}/wait-days?value=${days}`;
		this._changeWaitDays$ = this._httpClient.patch<TypedEnvelope<ParserResponse>>(requestUrl, null, { withCredentials: true }).pipe(
			finalize(() => (this._changeWaitDays$ = undefined)),
			shareReplay({ bufferSize: 1, refCount: true }),
		);

		return this._changeWaitDays$;
	}

	private startFetchingParsers(): Observable<TypedEnvelope<ParserResponse[]>> {
		if (this._parsersFetch$) return this._parsersFetch$;
		this._parsersFetch$ = this._httpClient.get<TypedEnvelope<ParserResponse[]>>(this._apiUrl, { withCredentials: true }).pipe(
			shareReplay(1),
			finalize((): void => (this._parsersFetch$ = undefined)),
		);
		return this._parsersFetch$;
	}

	private startPermantlyStartParser(id: string): Observable<TypedEnvelope<ParserResponse>> {
		if (this._isPermantlyStartingParser$) return this._isPermantlyStartingParser$;
		const requestUrl: string = `${this._apiUrl}/${id}/permantly-start`;
		this._isPermantlyStartingParser$ = this._httpClient
			.patch<TypedEnvelope<ParserResponse>>(requestUrl, null, { withCredentials: true })
			.pipe(
				finalize((): void => (this._isPermantlyStartingParser$ = undefined)),
				shareReplay({ bufferSize: 1, refCount: true }),
			);
		return this._isPermantlyStartingParser$;
	}

	private startAddingLinks(parserId: string, links: { name: string; url: string }[]): Observable<TypedEnvelope<ParserLinkResponse[]>> {
		if (this._addLinks$) return this._addLinks$;
		const requestUrl: string = `${this._apiUrl}/${parserId}/links`;
		const bodyLinksPayload: AddLinksToParserRequestBody[] = links.map(
			(l): AddLinksToParserRequestBody => ({ Name: l.name, Url: l.url }),
		);
		const body: AddLinksToParserRequest = { Links: bodyLinksPayload };
		this._addLinks$ = this._httpClient.post<TypedEnvelope<ParserLinkResponse[]>>(requestUrl, body, { withCredentials: true }).pipe(
			finalize((): void => (this._addLinks$ = undefined)),
			shareReplay({ bufferSize: 1, refCount: true }),
		);
		return this._addLinks$;
	}

	private startUpdatingLink(
		parserId: string,
		linkId: string,
		name?: string | null,
		url?: string | null,
	): Observable<TypedEnvelope<ParserLinkResponse>> {
		if (this._updateLink$) return this._updateLink$;
		const requestUrl: string = `${this._apiUrl}/${parserId}/links/${linkId}`;
		let params: HttpParams = new HttpParams();
		if (name) params = params.set('name', name);
		if (url) params = params.set('url', url);
		this._updateLink$ = this._httpClient
			.put<TypedEnvelope<ParserLinkResponse>>(requestUrl, null, { params, withCredentials: true })
			.pipe(
				finalize((): void => (this._updateLink$ = undefined)),
				shareReplay({ bufferSize: 1, refCount: true }),
			);
		return this._updateLink$;
	}

	private startChangingLinkActivity(parserId: string, linkId: string, activity: boolean): Observable<TypedEnvelope<ParserLinkResponse>> {
		if (this._changeLinkActivity$) return this._changeLinkActivity$;
		const requestUrl: string = `${this._apiUrl}/${parserId}/links/${linkId}/activity?value=${activity}`;
		this._changeLinkActivity$ = this._httpClient
			.patch<TypedEnvelope<ParserLinkResponse>>(requestUrl, null, { withCredentials: true })
			.pipe(
				finalize((): void => (this._changeLinkActivity$ = undefined)),
				shareReplay({ bufferSize: 1, refCount: true }),
			);
		return this._changeLinkActivity$;
	}

	private startRemovingLink(parserId: string, linkId: string): Observable<TypedEnvelope<ParserLinkResponse>> {
		if (this._removeLink$) return this._removeLink$;
		const requestUrl: string = `${this._apiUrl}/${parserId}/links/${linkId}`;
		this._removeLink$ = this._httpClient.delete<TypedEnvelope<ParserLinkResponse>>(requestUrl, { withCredentials: true }).pipe(
			finalize((): void => (this._removeLink$ = undefined)),
			shareReplay({ bufferSize: 1, refCount: true }),
		);
		return this._removeLink$;
	}
}
