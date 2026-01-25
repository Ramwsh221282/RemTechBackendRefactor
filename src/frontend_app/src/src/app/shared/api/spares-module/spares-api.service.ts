import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { HttpClient, HttpParams } from '@angular/common/http';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { GetSparesQueryResponse, SpareLocationResponse } from './spares-api.responses';
import { TypedEnvelope } from '../envelope';
import { ConvertSparesQueryToHttpParams, GetSpareLocationsQuery, GetSparesQueryParameters } from './spares-api.requests';
import { DefaultGetSparesQueryResponse } from './spares-api.factories';

@Injectable({
	providedIn: 'root',
})
export class SparesApiService {
	private readonly _apiUrl: string = `${apiUrl}/spares`;
	private readonly _httpClient: HttpClient = inject(HttpClient);
	private fetchingSpares$: Observable<GetSparesQueryResponse> | undefined;
	private fetchingSpareLocations$: Observable<SpareLocationResponse[]> | undefined;

	public fetchSpares(query: GetSparesQueryParameters): Observable<GetSparesQueryResponse> {
		return this.invokeFetchingSpares(query);
	}

	public fetchSpareLocations(query: GetSpareLocationsQuery): Observable<SpareLocationResponse[]> {
		return this.invokeFetchingSpareLocations(query);
	}

	private invokeFetchingSpareLocations(query: GetSpareLocationsQuery): Observable<SpareLocationResponse[]> {
		if (this.fetchingSpareLocations$) return this.fetchingSpareLocations$;
		const params: HttpParams = ConvertSparesQueryToHttpParams(query.params);
		const requestUrl: string = `${this._apiUrl}/locations`;
		this.fetchingSpareLocations$ = this._httpClient.get<TypedEnvelope<SpareLocationResponse[]>>(requestUrl, { params }).pipe(
			map((envelope: TypedEnvelope<SpareLocationResponse[]>): SpareLocationResponse[] => (envelope.body ? envelope.body : [])),
			finalize(() => (this.fetchingSpareLocations$ = undefined)),
			shareReplay({ refCount: true, bufferSize: 1 }),
		);
		return this.fetchingSpareLocations$;
	}

	private invokeFetchingSpares(query: GetSparesQueryParameters): Observable<GetSparesQueryResponse> {
		if (this.fetchingSpares$) return this.fetchingSpares$;
		const params: HttpParams = ConvertSparesQueryToHttpParams(query);
		this.fetchingSpares$ = this._httpClient.get<TypedEnvelope<GetSparesQueryResponse>>(this._apiUrl, { params }).pipe(
			map((envelope: TypedEnvelope<GetSparesQueryResponse>): GetSparesQueryResponse => {
				let response: GetSparesQueryResponse = DefaultGetSparesQueryResponse();
				if (envelope.body)
					response = {
						...response,
						AveragePrice: envelope.body.AveragePrice,
						MaximalPrice: envelope.body.MaximalPrice,
						MinimalPrice: envelope.body.MinimalPrice,
						TotalCount: envelope.body.TotalCount,
						Spares: envelope.body.Spares,
					};
				return response;
			}),
			shareReplay({ refCount: true, bufferSize: 1 }),
			finalize((): void => (this.fetchingSpares$ = undefined)),
		);
		return this.fetchingSpares$;
	}
}
