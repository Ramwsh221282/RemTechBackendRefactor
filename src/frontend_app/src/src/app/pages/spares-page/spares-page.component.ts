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
import {
	GetSparesQueryResponse,
	SpareLocationResponse,
	SpareResponse,
	SpareTypeResponse,
} from '../../shared/api/spares-module/spares-api.responses';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { GetSparesQueryParameters } from '../../shared/api/spares-module/spares-api.requests';
import { DefaultGetSpareParameters } from '../../shared/api/spares-module/spares-api.factories';
import {
	PriceChangeEvent,
	PriceSortChangeEvent,
	VehiclePriceFilterFormPartComponent,
} from '../vehicles-page/components/vehicle-price-filter-form-part/vehicle-price-filter-form-part.component';
import { catchError, EMPTY, tap } from 'rxjs';
import { SpareTypesSelectComponent } from './components/spare-types-select/spare-types-select.component';
import { SpareLocationsSelectComponent } from './components/spare-locations-select/spare-locations-select.component';

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
		SpareTypesSelectComponent,
		SpareLocationsSelectComponent,
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
	readonly spareTypes: WritableSignal<SpareTypeResponse[]> = signal<SpareTypeResponse[]>([]);
	readonly currentLocation: WritableSignal<SpareLocationResponse | null | undefined> = signal<SpareLocationResponse | null | undefined>(
		null,
	);
	readonly currentSpareType: WritableSignal<SpareTypeResponse | null | undefined> = signal<SpareTypeResponse | null | undefined>(null);
	readonly query: WritableSignal<GetSparesQueryParameters> = signal<GetSparesQueryParameters>(defaultQuery());

	readonly onSparesQueryChange: EffectRef = effect((): void => {
		const query: GetSparesQueryParameters = this.query();
		this.fetchSpares(query);
	});

	readonly onCurrentLocationChange: EffectRef = effect((): void => {
		const location: SpareLocationResponse | null | undefined = this.currentLocation();
		this.query.update((current: GetSparesQueryParameters): GetSparesQueryParameters => {
			return { ...current, RegionId: location?.Id };
		});
	});

	readonly onCurrentSpareTypeChange: EffectRef = effect((): void => {
		const spareType: SpareTypeResponse | null | undefined = this.currentSpareType();
		this.query.update((current: GetSparesQueryParameters): GetSparesQueryParameters => {
			return { ...current, Type: spareType?.Value };
		});
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

	public handlePriceRangeFilterChange($event: PriceChangeEvent): void {
		this.query.update((current: GetSparesQueryParameters): GetSparesQueryParameters => {
			return { ...current, MaximalPrice: $event.maximalPrice, MinimalPrice: $event.minimalPrice };
		});
	}

	public handleSortModeChange($event: PriceSortChangeEvent): void {
		this.query.update((current: GetSparesQueryParameters): GetSparesQueryParameters => {
			return { ...current, OrderMode: $event.mode };
		});
	}

	private fetchSpares(query: GetSparesQueryParameters): void {
		this._sparesApiService
			.fetchSpares(query)
			.pipe(
				tap((response: GetSparesQueryResponse) => {
					this.spares.set(response.Spares);
					const statistics: AggregatedStatistics = mapSparesResponseToStatistics(response);
					this.statistics.set(statistics);
				}),
			)
			.subscribe();
	}

	private fetchInitialData(): void {
		this._sparesApiService
			.fetchSpares(this.query())
			.pipe(
				tap((response: GetSparesQueryResponse): void => {
					const statistics: AggregatedStatistics = mapSparesResponseToStatistics(response);
					this.statistics.set(statistics);
					this.spares.set(response.Spares);
				}),
				catchError(() => EMPTY),
			)
			.subscribe();
	}
}
