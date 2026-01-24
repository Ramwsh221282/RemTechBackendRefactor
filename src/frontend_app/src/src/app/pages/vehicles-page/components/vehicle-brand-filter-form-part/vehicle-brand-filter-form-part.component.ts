import { Component, EventEmitter, input, Input, Output, signal, WritableSignal } from '@angular/core';
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
	properties = input<VehicleBrandFilterFormPartComponentProps>(defaultProps());
	@Output() brandChange: EventEmitter<BrandResponse | null | undefined> = new EventEmitter<BrandResponse | null | undefined>();
	public onChange($event: SelectChangeEvent): void {}
}

type VehicleBrandFilterFormPartComponentProps = {
	currentBrand: BrandResponse | null | undefined;
	brands: BrandResponse[];
};

const defaultProps: () => VehicleBrandFilterFormPartComponentProps = (): VehicleBrandFilterFormPartComponentProps => {
	return {
		currentBrand: null,
		brands: [],
	};
};
