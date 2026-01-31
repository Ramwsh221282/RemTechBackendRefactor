import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { HttpClient, HttpParams } from '@angular/common/http';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { ActionRecordsPageResponse, PaginatedTelemetryRecordsResponse, TelemetryStatisticsResponse } from './telemetry-responses';
import { TypedEnvelope } from '../envelope';
import { ActionRecordsQuery } from './telemetry-fetch.request';
import { AccountPermissionsResponse } from '../identity-module/identity-responses';

@Injectable({
	providedIn: 'root',
})
export class TelemetryApiService {
	private readonly _apiUrl: string = `${apiUrl}/telemetry`;
	private readonly _httpClient: HttpClient = inject(HttpClient);
	private _dataFetch$: Observable<PaginatedTelemetryRecordsResponse> | null = null;
	private _statisticsFetch$: Observable<TelemetryStatisticsResponse[]> | null = null;
	private _permissionsFetch$: Observable<AccountPermissionsResponse[]> | null = null;
	private _fetchPageInfo$: Observable<ActionRecordsPageResponse> | null = null;

	public fetchTelemetryRecords(query: ActionRecordsQuery): Observable<PaginatedTelemetryRecordsResponse> {
		if (this._dataFetch$) return this._dataFetch$;

		const requestUrl: string = `${this._apiUrl}/records`;
		const params: HttpParams = query.buildHttpParams();

		this._dataFetch$ = this._httpClient.get<TypedEnvelope<PaginatedTelemetryRecordsResponse>>(requestUrl, { params }).pipe(
			map((envelope: TypedEnvelope<PaginatedTelemetryRecordsResponse>) => {
				const response: PaginatedTelemetryRecordsResponse = {
					HasNextPage: envelope.body?.HasNextPage ?? false,
					HasPreviousPage: envelope.body?.HasPreviousPage ?? false,
					MaxPage: envelope.body?.MaxPage ?? 0,
					PageNumber: envelope.body?.PageNumber ?? 0,
					PagesCount: envelope.body?.PagesCount ?? 0,
					PageSize: envelope.body?.PageSize ?? 0,
					TotalCount: envelope.body?.TotalCount ?? 0,
					Items: envelope.body?.Items ?? [],
				};
				return response;
			}),
			finalize(() => (this._dataFetch$ = null)),
			shareReplay({ bufferSize: 1, refCount: true }),
		);

		return this._dataFetch$;
	}

	public fetchTelemetryPageInfo(query: ActionRecordsQuery): Observable<ActionRecordsPageResponse> {
		if (this._fetchPageInfo$) return this._fetchPageInfo$;

		const requestUrl: string = `${this._apiUrl}`;
		const params: HttpParams = query.buildHttpParams();
		this._fetchPageInfo$ = this._httpClient.get<TypedEnvelope<ActionRecordsPageResponse>>(requestUrl, { params }).pipe(
			finalize(() => (this._fetchPageInfo$ = null)),
			shareReplay({ bufferSize: 1, refCount: true }),
			map((response: TypedEnvelope<ActionRecordsPageResponse>): ActionRecordsPageResponse => {
				return response.body!;
			}),
		);

		return this._fetchPageInfo$;
	}

	public fetchTelemetryRecordsStatistics(query: ActionRecordsQuery): Observable<TelemetryStatisticsResponse[]> {
		if (this._statisticsFetch$) return this._statisticsFetch$;

		const requestUrl: string = `${this._apiUrl}/records/statistics`;
		const params: HttpParams = query.buildHttpParams();
		this._statisticsFetch$ = this._httpClient.get<TypedEnvelope<TelemetryStatisticsResponse[]>>(requestUrl, { params }).pipe(
			map((response: TypedEnvelope<TelemetryStatisticsResponse[]>): TelemetryStatisticsResponse[] => {
				return response.body ?? [];
			}),
		);
		return this._statisticsFetch$;
	}

	public fetchPermissions(): Observable<AccountPermissionsResponse[]> {
		if (this._permissionsFetch$) return this._permissionsFetch$;
		const requestUrl: string = `${apiUrl}/identity/permissions`;
		this._permissionsFetch$ = this._httpClient.get<TypedEnvelope<AccountPermissionsResponse[]>>(requestUrl).pipe(
			map((response: TypedEnvelope<AccountPermissionsResponse[]>): AccountPermissionsResponse[] => {
				return response.body ?? [];
			}),
			finalize(() => (this._permissionsFetch$ = null)),
			shareReplay({ bufferSize: 1, refCount: true }),
		);
		return this._permissionsFetch$;
	}
}
