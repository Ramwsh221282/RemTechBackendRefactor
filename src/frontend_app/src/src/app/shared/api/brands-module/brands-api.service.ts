import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import { BrandResponse } from './brands-api.responses';
import { apiUrl } from '../api-endpoint';
import { GetBrandsQuery } from './brands-get-query';

@Injectable({
	providedIn: 'root',
})
export class BrandsApiService {
	private readonly _httpClient: HttpClient = inject(HttpClient);
	private readonly _apiUrl: string = `${apiUrl}/brands`;
	private _fetchingBrands$: Observable<BrandResponse[]> | undefined;

	public fetchBrands(query: GetBrandsQuery): Observable<BrandResponse[]> {
		return this.invokeBrandsFetch(query);
	}

	private invokeBrandsFetch(query: GetBrandsQuery): Observable<BrandResponse[]> {
		if (this._fetchingBrands$) return this._fetchingBrands$;
		const params: HttpParams = query.toHttpParams();
		this._fetchingBrands$ = this._httpClient.get<TypedEnvelope<BrandResponse[]>>(this._apiUrl, { params }).pipe(
			map((envelope: TypedEnvelope<BrandResponse[]>): BrandResponse[] => {
				let response: BrandResponse[] = [];
				if (envelope.body) response = envelope.body;
				return response;
			}),
			finalize((): void => (this._fetchingBrands$ = undefined)),
			shareReplay({ refCount: true, bufferSize: 1 }),
		);
		return this._fetchingBrands$;
	}
}
