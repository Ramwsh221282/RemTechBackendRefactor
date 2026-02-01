import { Component, EventEmitter, input, InputSignal, OnDestroy, OnInit, Output, signal, WritableSignal } from '@angular/core';
import { InputText } from 'primeng/inputtext';
import { readInputTextFromInputElementEventNullable } from '../../../../../../shared/utils/input-element-string-reader';
import { UserInputDebouncer } from '../../../../../../shared/utils/user-input-debouncer';

@Component({
	selector: 'app-action-record-filter-input',
	imports: [InputText],
	templateUrl: './action-record-invoker-login-search-input.component.html',
	styleUrl: './action-record-invoker-login-search-input.component.css',
})
export class ActionRecordInvokerLoginSearchInputComponent implements OnInit, OnDestroy {
	@Output() searchChange: EventEmitter<string | null> = new EventEmitter<string | null>();
	defaultValue: InputSignal<string | null> = input<string | null>(null);
	placeholder: InputSignal<string> = input<string>('no placeholder.');
	private readonly _debounce: UserInputDebouncer = new UserInputDebouncer(1000, this.emitUserInput.bind(this));
	readonly searchValue: WritableSignal<string | null> = signal<string | null>(null);

	public ngOnInit(): void {
		const defaultValue: string | null = this.defaultValue();
		if (defaultValue) this.searchValue.set(defaultValue);
	}

	public ngOnDestroy(): void {
		this._debounce.dispose();
	}

	public handleUserInput($event: Event): void {
		const input: string | null | undefined = readInputTextFromInputElementEventNullable($event);
		this.searchValue.set(input);
		this._debounce.trigger();
	}

	private emitUserInput(): void {
		const input: string | null = this.searchValue();
		this.searchChange.emit(input);
	}
}
