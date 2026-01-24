import { Component, EventEmitter, input, InputSignal, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select, SelectChangeEvent } from 'primeng/select';
import { CategoryResponse } from '../../../../shared/api/categories-module/categories-responses';

@Component({
	selector: 'app-vehicle-category-filter-form-part',
	imports: [FormsModule, Select],
	templateUrl: './vehicle-category-filter-form-part.component.html',
	styleUrl: './vehicle-category-filter-form-part.component.scss',
})
export class VehicleCategoryFilterFormPartComponent {
	properties: InputSignal<VehicleCategoryFilterFormPartComponentProps> = input(defaultProps());
	@Output() categorySelected: EventEmitter<CategoryResponse | null | undefined> = new EventEmitter<CategoryResponse | null | undefined>();
	public onChange($event: SelectChangeEvent): void {
		console.log($event);
	}
}

type VehicleCategoryFilterFormPartComponentProps = {
	currentCategory: CategoryResponse | null | undefined;
	categories: CategoryResponse[];
};

const defaultProps: () => VehicleCategoryFilterFormPartComponentProps = (): VehicleCategoryFilterFormPartComponentProps => {
	return {
		currentCategory: null,
		categories: [],
	};
};
