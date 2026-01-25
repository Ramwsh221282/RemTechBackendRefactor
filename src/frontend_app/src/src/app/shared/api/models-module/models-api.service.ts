import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import { ModelResponse } from './models-responses';
import { GetModelsQuery } from './models-get-query';

@Injectable({
	providedIn: 'root',
})
export class ModelsApiService {
	private readonly _httpClient: HttpClient = inject(HttpClient);
	private readonly _apiUrl: string = `${apiUrl}/models`;
	private fetchingModels$: Observable<ModelResponse[]> | undefined;

	public fetchModels(query: GetModelsQuery): Observable<ModelResponse[]> {
		return this.invokeFetchingModels(query);
	}

	private invokeFetchingModels(query: GetModelsQuery): Observable<ModelResponse[]> {
		if (this.fetchingModels$) return this.fetchingModels$;
		const params: HttpParams = query.toHttpParams();
		this.fetchingModels$ = this._httpClient.get<TypedEnvelope<ModelResponse[]>>(this._apiUrl, { params }).pipe(
			map((envelope): ModelResponse[] => {
				let response: ModelResponse[] = [];
				if (envelope.body) response = envelope.body;
				return response;
			}),
			finalize((): void => (this.fetchingModels$ = undefined)),
			shareReplay({ refCount: true, bufferSize: 1 }),
		);
		return this.fetchingModels$;
	}
}
