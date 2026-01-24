import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { GetVehiclesQueryResponse } from './vehicles-api.responses';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import { GetVehiclesQuery } from './vehicles-get-query';

@Injectable({
	providedIn: 'root',
})
export class VehiclesApiService {
	private readonly _httpClient: HttpClient = inject(HttpClient);
	private readonly _apiUrl: string = `${apiUrl}/vehicles`;
	private _fetchingVehicles$: Observable<GetVehiclesQueryResponse> | undefined;

	public fetchVehicles(query: GetVehiclesQuery): Observable<GetVehiclesQueryResponse> {
		return this.invokeFetchVehicles(query);
	}

	private invokeFetchVehicles(query: GetVehiclesQuery): Observable<GetVehiclesQueryResponse> {
		if (this._fetchingVehicles$) return this._fetchingVehicles$;
		const params: HttpParams = query.toHttpParams();
		this._fetchingVehicles$ = this._httpClient.get<TypedEnvelope<GetVehiclesQueryResponse>>(this._apiUrl, { params }).pipe(
			map((envelope: TypedEnvelope<GetVehiclesQueryResponse>): GetVehiclesQueryResponse => {
				let response: GetVehiclesQueryResponse = {
					Vehicles: [],
					TotalCount: 0,
					AveragePrice: 0,
					MaximalPrice: 0,
					MinimalPrice: 0,
				};
				if (envelope.body) response = envelope.body;
				return response;
			}),
			shareReplay({ refCount: true, bufferSize: 1 }),
			finalize((): void => (this._fetchingVehicles$ = undefined)),
		);
		return this._fetchingVehicles$;
	}
}
