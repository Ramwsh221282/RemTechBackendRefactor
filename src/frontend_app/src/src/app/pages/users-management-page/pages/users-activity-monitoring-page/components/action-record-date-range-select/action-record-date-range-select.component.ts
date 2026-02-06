import { Component, EventEmitter, Input, input, InputSignal, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePicker } from 'primeng/datepicker';

@Component({
	selector: 'app-action-record-date-range-select',
	imports: [DatePicker, FormsModule],
	template: `
		<p-datePicker
			[disabled]="disabled()"
			[showClear]="true"
			[selectionMode]="'range'"
			[size]="'small'"
			[fluid]="true"
			[placeholder]="'Выбрать диапазон дат'"
			[dateFormat]="'dd.mm.yy'"
			[(ngModel)]="selectedRange"
			(ngModelChange)="handleDateRangeSelected($event)"
		/>
	`,
	styleUrl: './action-record-date-range-select.component.css',
})
export class ActionRecordDateRangeSelectComponent {
	@Output() dateRangeSelected: EventEmitter<Date[] | null> = new EventEmitter<Date[] | null>();
	placeHolder: InputSignal<string> = input('no placeholder');
	dateFormat: InputSignal<string> = input('dd.mm.yy');
	disabled: InputSignal<boolean> = input(false);
	selectedRange: Date[] = [];

	public handleDateRangeSelected($event: Date[] | null): void {
		if ($event === null) {
			this.dateRangeSelected.emit(null);
			return;
		}

		const normalized: Date[] = $event.filter((date) => {
			return date !== null;
		});

		if (normalized.length == 2) {
			this.dateRangeSelected.emit(normalized);
		}
	}
}
