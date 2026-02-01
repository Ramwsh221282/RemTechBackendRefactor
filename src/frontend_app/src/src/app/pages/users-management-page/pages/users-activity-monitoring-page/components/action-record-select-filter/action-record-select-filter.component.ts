import { Component, EventEmitter, Input, input, InputSignal, Output } from '@angular/core';
import { Select, SelectChangeEvent } from 'primeng/select';

@Component({
	selector: 'app-action-record-select-filter',
	imports: [Select],
	templateUrl: './action-record-select-filter.component.html',
	styleUrl: './action-record-select-filter.component.css',
})
export class ActionRecordSelectFilterComponent<T> {
	@Output() valueSelected: EventEmitter<T | null> = new EventEmitter<T | null>();
	values: InputSignal<T[]> = input<T[]>([]);
	placeHolder: InputSignal<string> = input<string>('no placeholder.');
	showClearButton: InputSignal<boolean> = input<boolean>(false);
	labelName: InputSignal<string> = input<string>('no label.');

	public handleInputChange($event: SelectChangeEvent): void {
		const value: T = $event.value as T;
		this.valueSelected.emit(value);
	}
}
