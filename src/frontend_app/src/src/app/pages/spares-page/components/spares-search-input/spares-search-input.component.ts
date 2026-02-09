import { Component, effect, EffectRef, EventEmitter, input, InputSignal, Output, signal, WritableSignal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Button } from 'primeng/button';
import { UserInputDebouncer } from '../../../../shared/utils/user-input-debouncer';
import { StringUtils } from '../../../../shared/utils/string-utils';

@Component({
	selector: 'app-spares-search-input',
	imports: [Button, ReactiveFormsModule],
	templateUrl: './spares-search-input.component.html',
	styleUrl: './spares-search-input.component.scss',
})
export class SparesSearchInputComponent {
	showSubmitButton: InputSignal<boolean> = input(false);
	showResetButton: InputSignal<boolean> = input(false);
	@Output() onTextSearchSubmit: EventEmitter<string | null> = new EventEmitter<string | null>();
	private readonly _debouncer: UserInputDebouncer = new UserInputDebouncer(1000, this.search.bind(this));
	readonly inputValue: WritableSignal<string> = signal('');

	readonly onInputChangeEffect: EffectRef = effect((): void => {
		this._debouncer.trigger();
	});

	public handleInputChange($event: Event): void {
		const input: HTMLInputElement = $event.target as HTMLInputElement;
		this.inputValue.set(input.value);
		this._debouncer.trigger();
	}

	public onReset(): void {
		this.inputValue.set('');
		this._debouncer.trigger();
	}

	public search(): void {
		const input: string | null = this.inputValue();
		if (!input) this.onTextSearchSubmit.emit(null);
		if (StringUtils.isEmptyOrWhiteSpace(input)) this.onTextSearchSubmit.emit(null);
		this.onTextSearchSubmit.emit(input);
	}
}
