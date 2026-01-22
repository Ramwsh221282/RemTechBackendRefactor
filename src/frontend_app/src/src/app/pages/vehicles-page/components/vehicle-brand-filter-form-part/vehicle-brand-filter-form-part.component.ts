import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select, SelectChangeEvent } from 'primeng/select';
import { CatalogueBrand } from '../../types/CatalogueBrand';
import { BrandResponse } from '../../../../shared/api/brands-module/brands-api.responses';

@Component({
	selector: 'app-vehicle-brand-filter-form-part',
	imports: [FormsModule, Select],
	templateUrl: './vehicle-brand-filter-form-part.component.html',
	styleUrl: './vehicle-brand-filter-form-part.component.scss',
})
export class VehicleBrandFilterFormPartComponent {
	readonly brands: WritableSignal<CatalogueBrand[]> = signal([]);
	readonly currentBrand: WritableSignal<BrandResponse | null | undefined> = signal(undefined);
	@Input({ required: true }) set current_brand(value: BrandResponse | null | undefined) {
		this.currentBrand.set(value);
	}
	@Output() onBrandChange: EventEmitter<string | null | undefined> = new EventEmitter();

	public onChange($event: SelectChangeEvent): void {
		const brand: CatalogueBrand | null = $event.value as CatalogueBrand;
		if (brand) {
			this.onBrandChange.emit(brand.id);
			return;
		}
		this.onBrandChange.emit(undefined);
	}
}
