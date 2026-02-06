import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { LocationResponse } from './locations.responses';
import { TypedEnvelope } from '../envelope';
import { GetLocationsQuery } from './locations-get-query';

@Injectable({
	providedIn: 'root',
})
export class LocationsApiService {
	private readonly _httpClient: HttpClient = inject(HttpClient);
	private readonly _apiUrl: string = `${apiUrl}`;
	private fetchingLocations$: Observable<LocationResponse[]> | undefined;

	public fetchLocations(query: GetLocationsQuery): Observable<LocationResponse[]> {
		return this.invokeLocationsFetch(query);
	}

	private invokeLocationsFetch(query: GetLocationsQuery): Observable<LocationResponse[]> {
		if (this.fetchingLocations$) return this.fetchingLocations$;
		const requestUrl: string = `${this._apiUrl}/locations`;
		const params: HttpParams = query.toHttpParams();
		this.fetchingLocations$ = this._httpClient.get<TypedEnvelope<LocationResponse[]>>(requestUrl, { params }).pipe(
			map((envelope: TypedEnvelope<LocationResponse[]>): LocationResponse[] => {
				return envelope.body ?? [];
			}),
			finalize((): void => (this.fetchingLocations$ = undefined)),
			shareReplay({ refCount: true, bufferSize: 1 }),
		);

		return this.fetchingLocations$;
	}
}
