import { Component, EventEmitter, input, InputSignal, OnInit, Output, signal, WritableSignal } from '@angular/core';
import { MultiSelect, MultiSelectChangeEvent } from 'primeng/multiselect';

@Component({
	selector: 'app-action-records-multiselect-filter',
	imports: [MultiSelect],
	template: `
		<div [className]="wrapperClass()">
			<p-multiSelect
				[options]="values()"
				[size]="'small'"
				[fluid]="true"
				[placeholder]="placeHolder()"
				[optionLabel]="labelName()"
				display="chip"
				class="w-full md:w-80"
				[filter]="useFilter()"
				(onChange)="handleMultiSelectChange($event)"
			/>
		</div>
	`,
	styleUrl: './action-records-multiselect-filter.component.css',
})
export class ActionRecordsMultiselectFilterComponent<T> {
	@Output() multiSelectListUpdated: EventEmitter<T[] | null> = new EventEmitter<T[] | null>();

	values: InputSignal<T[]> = input<T[]>([]);
	useFilter: InputSignal<boolean> = input<boolean>(true);
	placeHolder: InputSignal<string> = input<string>('no placeholder.');
	labelName: InputSignal<string> = input<string>('no label.');
	wrapperClass: InputSignal<string> = input<string>('');

	public handleMultiSelectChange($event: MultiSelectChangeEvent): void {
		const selectedValues: T[] = $event.value as T[];

		if (selectedValues.length === 0) {
			this.multiSelectListUpdated.emit(null);
			return;
		}

		this.multiSelectListUpdated.emit(selectedValues);
	}
}
