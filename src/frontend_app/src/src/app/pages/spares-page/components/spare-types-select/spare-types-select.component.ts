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
import { SparesApiService } from '../../../../shared/api/spares-module/spares-api.service';
import { SpareTypeResponse } from '../../../../shared/api/spares-module/spares-api.responses';
import { GetSpareTypesQuery } from '../../../../shared/api/spares-module/spares-api.requests';
import { catchError, EMPTY, finalize, tap } from 'rxjs';
import { Select, SelectChangeEvent } from 'primeng/select';

@Component({
	selector: 'app-spare-types-select',
	imports: [Select],
	templateUrl: './spare-types-select.component.html',
	styleUrl: './spare-types-select.component.css',
})
export class SpareTypesSelectComponent implements OnInit {
	private readonly _service: SparesApiService = inject(SparesApiService);
	textSearch: InputSignal<string | null | undefined> = input(defaultTextSearch());
	amount: InputSignal<number | null | undefined> = input(defaultSpareTypesAmount());
	readonly spareTypes: WritableSignal<SpareTypeResponse[]> = signal([]);
	readonly currentSpareType: WritableSignal<SpareTypeResponse | null | undefined> = signal(undefined);
	readonly loading: WritableSignal<boolean> = signal(false);
	@Output() sparesFetched: EventEmitter<SpareTypeResponse[]> = new EventEmitter<SpareTypeResponse[]>();
	@Output() spareSelected: EventEmitter<SpareTypeResponse | null | undefined> = new EventEmitter<SpareTypeResponse | null | undefined>();

	readonly emitSparesFetched: EffectRef = effect((): void => {
		const spareTypes: SpareTypeResponse[] = this.spareTypes();
		if (spareTypes.length === 0) return;
		this.sparesFetched.emit(this.spareTypes());
	});

	readonly emitSpareSelected: EffectRef = effect((): void => {
		this.spareSelected.emit(this.currentSpareType());
	});

	public ngOnInit(): void {
		this.loading.set(true);
		this._service
			.fetchSpareTypes(this.buildQuery())
			.pipe(
				finalize(() => this.loading.set(false)),
				tap((types: SpareTypeResponse[]) => this.spareTypes.set(types)),
				catchError(() => EMPTY),
			)
			.subscribe();
	}

	public handleSpareTypeSelectChange($event: SelectChangeEvent): void {
		const selectedType: SpareTypeResponse | null | undefined = $event.value as SpareTypeResponse | null | undefined;
		this.currentSpareType.set(selectedType);
	}

	public handleSpareTypeCleared(): void {
		this.currentSpareType.set(undefined);
	}

	private buildQuery(): GetSpareTypesQuery {
		return GetSpareTypesQuery.create().useAmount(this.amount()).useTextSearch(this.textSearch());
	}
}

function defaultTextSearch(): string | null | undefined {
	return undefined;
}

function defaultSpareTypesAmount(): number | null | undefined {
	return undefined;
}
