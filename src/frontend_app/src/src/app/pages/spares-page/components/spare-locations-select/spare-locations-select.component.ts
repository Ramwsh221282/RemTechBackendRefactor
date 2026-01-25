import {
	Component,
	effect,
	EffectRef,
	EventEmitter,
	inject,
	input,
	InputSignal,
	OnInit,
	Output,
	signal,
	WritableSignal,
} from '@angular/core';
import { SpareLocationResponse } from '../../../../shared/api/spares-module/spares-api.responses';
import { SparesApiService } from '../../../../shared/api/spares-module/spares-api.service';
import { SelectChangeEvent, Select } from 'primeng/select';
import { GetSpareLocationsQuery } from '../../../../shared/api/spares-module/spares-api.requests';
import { finalize, tap } from 'rxjs';

@Component({
	selector: 'app-spare-locations-select',
	imports: [Select],
	templateUrl: './spare-locations-select.component.html',
	styleUrl: './spare-locations-select.component.css',
})
export class SpareLocationsSelectComponent implements OnInit {
	private readonly _service: SparesApiService = inject(SparesApiService);
	readonly locations: WritableSignal<SpareLocationResponse[]> = signal([]);
	readonly currentLocation: WritableSignal<SpareLocationResponse | null | undefined> = signal(undefined);
	readonly isLoading: WritableSignal<boolean> = signal(false);
	textSearch: InputSignal<string | null | undefined> = input(defaultTextSearch());
	amount: InputSignal<number | null | undefined> = input(defaultAmount());
	@Output() locationsFetched: EventEmitter<SpareLocationResponse[]> = new EventEmitter<SpareLocationResponse[]>();
	@Output() locationSelected: EventEmitter<SpareLocationResponse | null | undefined> = new EventEmitter<
		SpareLocationResponse | null | undefined
	>();

	readonly emitLocationsFetched: EffectRef = effect((): void => {
		const locations: SpareLocationResponse[] = this.locations();
		if (locations.length === 0) return;
		this.locationsFetched.emit(this.locations());
	});

	readonly emitLocationSelected: EffectRef = effect((): void => {
		this.locationSelected.emit(this.currentLocation());
	});

	public ngOnInit(): void {
		const query: GetSpareLocationsQuery = this.buildQuery();
		this.isLoading.set(true);
		this._service
			.fetchSpareLocations(query)
			.pipe(
				finalize(() => this.isLoading.set(false)),
				tap((locations: SpareLocationResponse[]) => this.locations.set(locations)),
			)
			.subscribe();
	}

	public handleLocationSelected($event: SelectChangeEvent): void {
		const selectedLocation: SpareLocationResponse | null | undefined = $event.value as SpareLocationResponse | null | undefined;
		this.currentLocation.set(selectedLocation);
	}

	public handleLocationCleared(): void {
		this.currentLocation.set(undefined);
	}

	private buildQuery(): GetSpareLocationsQuery {
		return GetSpareLocationsQuery.create().useAmount(this.amount()).useTextSearch(this.textSearch());
	}
}

function defaultTextSearch(): string | null | undefined {
	return null;
}

function defaultAmount(): number | null | undefined {
	return null;
}
