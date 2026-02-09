import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { apiUrl } from '../api-endpoint';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { TypedEnvelope } from '../envelope';
import { CategoryResponse } from './categories-responses';
import { GetCategoriesQuery } from './categories-get-query';

@Injectable({
	providedIn: 'root',
})
export class CategoriesApiService {
	private readonly _httpClient: HttpClient = inject(HttpClient);
	private readonly _apiUrl: string = `${apiUrl}/categories`;
	private _fetchingCategories$: Observable<CategoryResponse[]> | undefined;

	public fetchCategories(query: GetCategoriesQuery): Observable<CategoryResponse[]> {
		return this.invokeFetchCategories(query);
	}

	private invokeFetchCategories(query: GetCategoriesQuery): Observable<CategoryResponse[]> {
		if (this._fetchingCategories$) return this._fetchingCategories$;
		const params: HttpParams = query.toHttpParams();
		this._fetchingCategories$ = this._httpClient.get<TypedEnvelope<CategoryResponse[]>>(this._apiUrl, { params }).pipe(
			map((envelope: TypedEnvelope<CategoryResponse[]>): CategoryResponse[] => envelope.body ?? []),
			finalize((): void => (this._fetchingCategories$ = undefined)),
			shareReplay({ bufferSize: 1, refCount: true }),
		);
		return this._fetchingCategories$;
	}
}
