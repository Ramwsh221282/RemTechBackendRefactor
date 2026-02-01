import { Component, EventEmitter, input, InputSignal, OnInit, Output, signal, WritableSignal } from '@angular/core';
import { Button } from 'primeng/button';

export interface SortChangeEvent {
	field: string;
	mode: 'ASC' | 'DESC' | 'NONE';
}

@Component({
	selector: 'app-sort-buttons',
	imports: [Button],
	templateUrl: './sort-buttons.component.html',
	styleUrl: './sort-buttons.component.css',
})
export class SortButtonsComponent implements OnInit {
	@Output() sortChange: EventEmitter<SortChangeEvent> = new EventEmitter<SortChangeEvent>();

	readonly sortFieldTracking: WritableSignal<string> = signal('');
	readonly sortModeTracking: WritableSignal<'ASC' | 'DESC' | 'NONE'> = signal('NONE');

	sortField: InputSignal<string> = input<string>('');
	sortMode: InputSignal<'ASC' | 'DESC' | 'NONE'> = input<'ASC' | 'DESC' | 'NONE'>('NONE');

	public select(value: 'ASC' | 'DESC' | 'NONE'): void {
		this.sortModeTracking.set(value);
		this.emitSortChange();
	}

	public getSeverity(mode: 'ASC' | 'DESC' | 'NONE'): 'info' | 'primary' {
		const currentMode: 'ASC' | 'DESC' | 'NONE' = this.sortModeTracking();
		if (mode === 'NONE' && currentMode === 'NONE') return 'info';
		if (mode === 'ASC' && currentMode === 'ASC') return 'info';
		if (mode === 'DESC' && currentMode === 'DESC') return 'info';
		return 'primary';
	}

	ngOnInit(): void {
		this.sortFieldTracking.set(this.sortField());
		this.sortModeTracking.set(this.sortMode());
	}

	private emitSortChange(): void {
		const field: string = this.sortFieldTracking();
		const mode: 'ASC' | 'DESC' | 'NONE' = this.sortModeTracking();
		this.sortChange.emit({ field, mode });
	}
}
