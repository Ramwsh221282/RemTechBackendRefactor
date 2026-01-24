import { Component, EventEmitter, input, InputSignal, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select, SelectChangeEvent } from 'primeng/select';
import { BrandResponse } from '../../../../shared/api/brands-module/brands-api.responses';

@Component({
	selector: 'app-vehicle-brand-filter-form-part',
	imports: [FormsModule, Select],
	templateUrl: './vehicle-brand-filter-form-part.component.html',
	styleUrl: './vehicle-brand-filter-form-part.component.scss',
})
export class VehicleBrandFilterFormPartComponent {
	disabled: InputSignal<boolean> = input(false);
	brands: InputSignal<BrandResponse[]> = input<BrandResponse[]>([]);
	currentBrand: InputSignal<BrandResponse | null | undefined> = input<BrandResponse | null | undefined>(null);
	@Output() brandChange: EventEmitter<BrandResponse | null | undefined> = new EventEmitter<BrandResponse | null | undefined>();

	public onChange($event: SelectChangeEvent): void {
		const brand: BrandResponse | null | undefined = $event.value;
		this.brandChange.emit(brand);
	}

	public get placeHolderText(): string {
		const currentBrand: BrandResponse | null | undefined = this.currentBrand();
		if (!currentBrand) return 'Выбрать марку';
		return currentBrand.Name;
	}
}
