import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { HttpClient } from '@angular/common/http';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { FetchTelemetryRecordsResponse } from './telemetry-responses';
import { TypedEnvelope } from '../envelope';

@Injectable({
	providedIn: 'root',
})
export class TelemetryApiService {
	private readonly _apiUrl: string = `${apiUrl}/telemetry`;
	private readonly _httpClient: HttpClient = inject(HttpClient);
	private _fetch$: Observable<FetchTelemetryRecordsResponse> | null = null;

	public fetchTelemetryRecords(): Observable<FetchTelemetryRecordsResponse> {
		if (this._fetch$) return this._fetch$;

		return this._httpClient.get<TypedEnvelope<FetchTelemetryRecordsResponse>>(`${this._apiUrl}/records`).pipe(
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
			finalize(() => (this._fetch$ = null)),
			shareReplay({ bufferSize: 1, refCount: true }),
		);
	}
}
