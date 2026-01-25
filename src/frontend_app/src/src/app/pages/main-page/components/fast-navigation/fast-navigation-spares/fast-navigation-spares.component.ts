import { Component, computed, effect, EventEmitter, inject, Input, Output, signal, Signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InputText } from 'primeng/inputtext';
import { tap } from 'rxjs';
import { DefaultGetSpareParameters } from '../../../../../shared/api/spares-module/spares-api.factories';
import { GetSparesQueryParameters } from '../../../../../shared/api/spares-module/spares-api.requests';
import { SpareResponse, GetSparesQueryResponse } from '../../../../../shared/api/spares-module/spares-api.responses';
import { SparesApiService } from '../../../../../shared/api/spares-module/spares-api.service';

type FastNavigationSparesProps = {
	page?: number | undefined;
	pageSize?: number | undefined;
	isWatching: boolean;
	textSearch?: string | undefined;
};

const defaultProps: () => FastNavigationSparesProps = (): FastNavigationSparesProps => {
	return { isWatching: false, textSearch: undefined, page: undefined, pageSize: undefined };
};

@Component({
	selector: 'app-fast-navigation-spares',
	imports: [FormsModule, InputText],
	templateUrl: './fast-navigation-spares.component.html',
	styleUrl: './fast-navigation-spares.component.css',
})
export class FastNavigationSparesComponent {
	private readonly _sparesService: SparesApiService = inject(SparesApiService);

	@Input({ required: true }) set spares_fetch_props(value: {
		page?: number | undefined;
		pageSize?: number | undefined;
		isWatching: boolean;
		textSearch?: string | undefined;
	}) {
		this.sparesFetchProps.update((state: FastNavigationSparesProps): FastNavigationSparesProps => {
			return {
				...state,
				isWatching: value.isWatching,
				textSearch: value.textSearch,
				page: value.page,
				pageSize: value.pageSize,
			};
		});
	}
	@Output() spareTextSearchChanged: EventEmitter<string | undefined> = new EventEmitter<string | undefined>();
	@Output() sparesFetched: EventEmitter<{
		spares: SpareResponse[];
		totalCount: number;
	}> = new EventEmitter<{
		spares: SpareResponse[];
		totalCount: number;
	}>();

	readonly sparesFetchProps: WritableSignal<FastNavigationSparesProps> = signal(defaultProps());
	readonly spareTextInput: WritableSignal<string | undefined> = signal(undefined);
	readonly itemTypes: string[] = ['Техника', 'Запчасти'];

	readonly userIsWatchingSpares: Signal<boolean> = computed((): boolean => {
		return this.sparesFetchProps().isWatching;
	});

	readonly sparesQuery: Signal<GetSparesQueryParameters> = computed((): GetSparesQueryParameters => {
		const props: FastNavigationSparesProps = this.sparesFetchProps();
		return {
			...DefaultGetSpareParameters(),
			Page: props.page,
			PageSize: props.pageSize,
			TextSearch: props.textSearch,
		};
	});

	readonly fetchSparesOnQueryChangeEffect = effect((): void => {
		const props: FastNavigationSparesProps = this.sparesFetchProps();
		this.fetchSparesOnQueryChange(props);
	});

	public navigateSpares(): void {}

	public handleUserSpareTextInput($event: Event): void {
		const inputText: string = ($event.target as HTMLInputElement).value;
		this.spareTextSearchChanged.emit(inputText);
		this.spareTextInput.set(inputText);
	}

	private fetchSparesOnQueryChange(props: FastNavigationSparesProps): void {
		if (!props.isWatching) return;
		const query: GetSparesQueryParameters = this.sparesQuery();
		this._sparesService
			.fetchSpares(query)
			.pipe(
				tap((response: GetSparesQueryResponse): void => {
					this.sparesFetched.emit({
						spares: response.Spares,
						totalCount: response.TotalCount,
					});
				}),
			)
			.subscribe();
	}
}
