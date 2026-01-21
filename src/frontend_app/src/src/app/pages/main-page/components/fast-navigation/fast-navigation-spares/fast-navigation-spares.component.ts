import {
  Component,
  computed,
  effect,
  EventEmitter,
  Input,
  Output,
  signal,
  Signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InputText } from 'primeng/inputtext';
import { tap } from 'rxjs';
import { DefaultGetSpareParameters } from '../../../../../shared/api/spares-module/spares-api.factories';
import { GetSparesQueryParameters } from '../../../../../shared/api/spares-module/spares-api.requests';
import {
  SpareResponse,
  GetSparesQueryResponse,
} from '../../../../../shared/api/spares-module/spares-api.responses';
import { SparesApiService } from '../../../../../shared/api/spares-module/spares-api.service';

@Component({
  selector: 'app-fast-navigation-spares',
  imports: [FormsModule, InputText],
  templateUrl: './fast-navigation-spares.component.html',
  styleUrl: './fast-navigation-spares.component.css',
})
export class FastNavigationSparesComponent {
  constructor(private readonly _sparesService: SparesApiService) {
    this.sparesFetched = new EventEmitter<{
      spares: SpareResponse[];
      totalCount: number;
    }>();
    this.spareTextSearchChanged = new EventEmitter<string | undefined>();

    this.sparesFetchProps = signal({
      isWatching: false,
      page: undefined,
      pageSize: undefined,
    });

    this.itemTypes = ['Техника', 'Запчасти'];
    this.fetchSparesOnQueryChange();
  }

  readonly sparesFetchProps: WritableSignal<{
    page?: number | undefined;
    pageSize?: number | undefined;
    isWatching: boolean;
    textSearch?: string | undefined;
  }>;

  @Input({ required: true }) set spares_fetch_props(value: {
    page?: number | undefined;
    pageSize?: number | undefined;
    isWatching: boolean;
    textSearch?: string | undefined;
  }) {
    this.sparesFetchProps.set(value);
  }

  @Output() spareTextSearchChanged: EventEmitter<string | undefined>;

  @Output() sparesFetched: EventEmitter<{
    spares: SpareResponse[];
    totalCount: number;
  }>;

  readonly itemTypes: string[];

  readonly userIsWatchingSpares: Signal<boolean> = computed((): boolean => {
    return this.sparesFetchProps().isWatching;
  });

  public spareTextInput: WritableSignal<string | undefined> = signal(undefined);

  public navigateSpares(): void {}

  public handleUserSpareTextInput($event: Event): void {
    const inputText: string = ($event.target as HTMLInputElement).value;
    this.spareTextSearchChanged.emit(inputText);
    this.spareTextInput.set(inputText);
  }

  private fetchSparesOnQueryChange(): void {
    effect(() => {
      const props: {
        page?: number | undefined;
        pageSize?: number | undefined;
        isWatching: boolean;
      } = this.sparesFetchProps();
      if (!props.isWatching) return;
      const query: GetSparesQueryParameters = {
        ...DefaultGetSpareParameters(),
        Page: props.page,
        PageSize: props.pageSize,
        TextSearch: this.spareTextInput(),
      };
      this.invokeFetchSpares(query);
    });
  }

  private invokeFetchSpares(query: GetSparesQueryParameters): void {
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
