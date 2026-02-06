import { Component, EventEmitter, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { UserInputDebouncer } from '../../../../shared/utils/user-input-debouncer';
import { readInputTextFromInputElementEvent } from '../../../../shared/utils/input-element-string-reader';

@Component({
	selector: 'app-vehicles-text-search',
	imports: [FormsModule, ReactiveFormsModule],
	templateUrl: './vehicles-text-search.component.html',
	styleUrl: './vehicles-text-search.component.scss',
})
export class VehiclesTextSearchComponent {
	@Output() textSearchSubmit: EventEmitter<string | undefined> = new EventEmitter<string | undefined>();
	private readonly _debounce$: UserInputDebouncer = new UserInputDebouncer(1000, this.submitTextSearch.bind(this));
	readonly textInput: WritableSignal<string | null | undefined> = signal('');

	public onTextInputChange($event: Event): void {
		const text: string | null | undefined = readInputTextFromInputElementEvent($event);
		if (!text) this.textInput.set('');
		else this.textInput.set(text);
		this._debounce$.trigger();
	}

	public onReset(): void {
		this._debounce$.trigger();
	}

	private submitTextSearch(): void {
		const text: string | null | undefined = this.textInput();

		if (!text) {
			this.textSearchSubmit.emit(undefined);
			return;
		}

		if (StringUtils.isEmptyOrWhiteSpace(text)) {
			this.textSearchSubmit.emit(undefined);
			return;
		}

		this.textSearchSubmit.emit(text);
	}
}
