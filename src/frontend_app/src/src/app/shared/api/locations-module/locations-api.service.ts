import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { apiUrl } from '../api-endpoint';
import { finalize, map, Observable, shareReplay } from 'rxjs';
import { LocationResponse } from './locations.responses';
import { GetLocationsQueryParameters } from './locations.requests';
import { LocationsQueryIncludes } from './locations.behavior';
import { TypedEnvelope } from '../envelope';

@Injectable({
    providedIn: 'root',
})
export class LocationsApiService {
    private readonly _httpClient: HttpClient = inject(HttpClient);
    private readonly _apiUrl: string = `${apiUrl}`;
    private fetchingLocations$: Observable<LocationResponse[]> | undefined;

    public fetchLocations(query: GetLocationsQueryParameters): Observable<LocationResponse[]> {
        return this.invokeLocationsFetch(query);
    }

    private invokeLocationsFetch(query: GetLocationsQueryParameters): Observable<LocationResponse[]> {
        if (this.fetchingLocations$) return this.fetchingLocations$;
        const requestUrl: string = `${this._apiUrl}/locations`;
        const params: HttpParams = this.craeteQueryParameters(query);
        this.fetchingLocations$ = this._httpClient.get<TypedEnvelope<LocationResponse[]>>(requestUrl, { params }).pipe(
            map((envelope: TypedEnvelope<LocationResponse[]>): LocationResponse[] => {
                return envelope.body ?? [];
            }),
            finalize((): void => (this.fetchingLocations$ = undefined)),
            shareReplay({ refCount: true, bufferSize: 1 }),
        );

        return this.fetchingLocations$;
    }

    private craeteQueryParameters(query: GetLocationsQueryParameters): HttpParams {
        let params: HttpParams = new HttpParams();
        const includes: string[] = [];

        if (query.Id) {
            params = params.append('id', query.Id);
        }

        if (query.CategoryId) {
            params = params.append('category-id', query.CategoryId);
        }

        if (query.BrandId) {
            params = params.append('brand-id', query.BrandId);
        }

        if (query.ModelId) {
            params = params.append('model-id', query.ModelId);
        }

        if (query.CategoryName) {
            params = params.append('category-name', query.CategoryName);
        }

        if (query.BrandName) {
            params = params.append('brand-name', query.BrandName);
        }

        if (query.ModelName) {
            params = params.append('model-name', query.ModelName);
        }

        if (query.Page) {
            params = params.append('page', query.Page.toString());
        }

        if (query.OrderByName) {
            params = params.append('order-by-name', query.OrderByName.toString());
        }

        if (query.TextSearch) {
            params = params.append('text-search', query.TextSearch);
            includes.push('text-search-score');
        }

        if (LocationsQueryIncludes('vehicles-count', query)) {
            includes.push('vehicles-count');
        }

        if (includes.length > 0) {
            for (const include of includes) {
                params = params.append('include', include);
            }
        }

        return params;
    }
}
