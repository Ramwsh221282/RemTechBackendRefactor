import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { HttpClient, HttpParams } from '@angular/common/http';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { FetchTelemetryRecordsResponse, TelemetryStatisticsResponse } from './telemetry-responses';
import { TypedEnvelope } from '../envelope';
import { FetchAnalyticsTelemetryRecordsQuery } from './telemetry-fetch.request';

@Injectable({
	providedIn: 'root',
})
export class TelemetryApiService {
	private readonly _apiUrl: string = `${apiUrl}/telemetry`;
	private readonly _httpClient: HttpClient = inject(HttpClient);
	private _dataFetch$: Observable<FetchTelemetryRecordsResponse> | null = null;
	private _statisticsFetch$: Observable<TelemetryStatisticsResponse[]> | null = null;

	public fetchTelemetryRecords(query: FetchAnalyticsTelemetryRecordsQuery): Observable<FetchTelemetryRecordsResponse> {
		if (this._dataFetch$) return this._dataFetch$;

		const requestUrl: string = `${this._apiUrl}/records`;
		const params: HttpParams = query.buildHttpParams();

		this._dataFetch$ = this._httpClient.get<TypedEnvelope<FetchTelemetryRecordsResponse>>(requestUrl, { params }).pipe(
			map((envelope: TypedEnvelope<FetchTelemetryRecordsResponse>) => {
				const response: FetchTelemetryRecordsResponse = {
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

	public fetchTelemetryRecordsStatistics(query: FetchAnalyticsTelemetryRecordsQuery): Observable<TelemetryStatisticsResponse[]> {
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
}
