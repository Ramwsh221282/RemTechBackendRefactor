import { Component, effect, EffectRef, inject, OnInit, signal, WritableSignal } from '@angular/core';

import { SparesSearchInputComponent } from './components/spares-search-input/spares-search-input.component';
import { SparePhotoComponent } from './components/spare-photo/spare-photo.component';
import { SpareContentComponent } from './components/spare-content/spare-content.component';
import { SpareTitleComponent } from './components/spare-title/spare-title.component';
import { SpareDetailsComponent } from './components/spare-details/spare-details.component';
import { SpareSourceComponent } from './components/spare-source/spare-source.component';
import { Paginator } from 'primeng/paginator';
import { ActivatedRoute } from '@angular/router';
import { SparesApiService } from '../../shared/api/spares-module/spares-api.service';
import { GetSparesQueryResponse, SpareLocationResponse, SpareResponse } from '../../shared/api/spares-module/spares-api.responses';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { GetSpareLocationsQuery, GetSparesQueryParameters } from '../../shared/api/spares-module/spares-api.requests';
import { DefaultGetSpareParameters } from '../../shared/api/spares-module/spares-api.factories';
import { VehiclePriceFilterFormPartComponent } from '../vehicles-page/components/vehicle-price-filter-form-part/vehicle-price-filter-form-part.component';
import { VehicleRegionsFilterFormPartComponent } from '../vehicles-page/components/vehicle-regions-filter-form-part/vehicle-regions-filter-form-part.component';
import { catchError, EMPTY, forkJoin, Observable, tap } from 'rxjs';

type AggregatedStatistics = {
	totalCount: number;
	averagePrice: number;
	minimalPrice: number;
	maximalPrice: number;
};

function defaultStatistics(): AggregatedStatistics {
	return { totalCount: 0, averagePrice: 0, minimalPrice: 0, maximalPrice: 0 };
}

function defaultQuery(): GetSparesQueryParameters {
	return { ...DefaultGetSpareParameters(), Page: 1, PageSize: 30 };
}

function mapSparesResponseToStatistics(response: GetSparesQueryResponse): AggregatedStatistics {
	return {
		averagePrice: response.AveragePrice,
		maximalPrice: response.MaximalPrice,
		minimalPrice: response.MinimalPrice,
		totalCount: response.TotalCount,
	};
}

@Component({
	selector: 'app-spares-page',
	imports: [
		SparesSearchInputComponent,
		SparePhotoComponent,
		SpareContentComponent,
		SpareTitleComponent,
		SpareDetailsComponent,
		SpareSourceComponent,
		Paginator,
		PaginationComponent,
		VehiclePriceFilterFormPartComponent,
		VehicleRegionsFilterFormPartComponent,
	],
	templateUrl: './spares-page.component.html',
	styleUrl: './spares-page.component.scss',
})
export class SparesPageComponent implements OnInit {
	private readonly _sparesApiService: SparesApiService = inject(SparesApiService);
	private readonly _activatedRoute: ActivatedRoute = inject(ActivatedRoute);

	readonly pageSize: number = 30;
	readonly spares: WritableSignal<SpareResponse[]> = signal<SpareResponse[]>([]);
	readonly locations: WritableSignal<SpareLocationResponse[]> = signal<SpareLocationResponse[]>([]);
	readonly statistics: WritableSignal<AggregatedStatistics> = signal<AggregatedStatistics>(defaultStatistics());
	readonly query: WritableSignal<GetSparesQueryParameters> = signal<GetSparesQueryParameters>(defaultQuery());

	readonly onSparesQueryChange: EffectRef = effect((): void => {
		const query: GetSparesQueryParameters = this.query();
		this.fetchSpares(query);
	});

	public ngOnInit(): void {
		this.fetchInitialData();
	}

	public acceptPageChange(page: number): void {
		this.query.update((current: GetSparesQueryParameters): GetSparesQueryParameters => {
			return { ...current, Page: page };
		});
	}

	public acceptTextSearch(text: string | null): void {
		this.query.update((current: GetSparesQueryParameters): GetSparesQueryParameters => {
			return { ...current, TextSearch: text };
		});
	}

	private fetchSpares(query: GetSparesQueryParameters): void {
		this._sparesApiService.fetchSpares(query).pipe(
			tap((response: GetSparesQueryResponse) => {
				this.spares.set(response.Spares);
				const statistics: AggregatedStatistics = mapSparesResponseToStatistics(response);
				this.statistics.set(statistics);
			}),
		);
	}

	private fetchInitialData(): void {
		const locationsFetch$: Observable<SpareLocationResponse[]> = this._sparesApiService.fetchSpareLocations(
			GetSpareLocationsQuery.create(),
		);

		const sparesFetch$: Observable<GetSparesQueryResponse> = this._sparesApiService.fetchSpares(this.query());

		forkJoin([sparesFetch$, locationsFetch$])
			.pipe(
				tap((response: [GetSparesQueryResponse, SpareLocationResponse[]]) => {
					const sparesResponse: GetSparesQueryResponse = response[0];
					const locations: SpareLocationResponse[] = response[1];
					const statistics: AggregatedStatistics = mapSparesResponseToStatistics(sparesResponse);
					this.locations.set(locations);
					this.spares.set(sparesResponse.Spares);
					this.statistics.set(statistics);
				}),
				catchError(() => EMPTY),
			)
			.subscribe();
	}
}
