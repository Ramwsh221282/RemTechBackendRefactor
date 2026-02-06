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
	categories: InputSignal<CategoryResponse[]> = input(defaultCategories());
	currentCategory: InputSignal<CategoryResponse | null | undefined> = input(defaultCategory());
	@Output() categorySelected: EventEmitter<CategoryResponse | null | undefined> = new EventEmitter<CategoryResponse | null | undefined>();

	public onChange($event: SelectChangeEvent): void {
		const category: CategoryResponse | null | undefined = $event.value;
		this.categorySelected.emit(category);
	}

	public get placeHolderText(): string {
		const currentCategory: CategoryResponse | null | undefined = this.currentCategory();
		if (!currentCategory) return 'Выбрать категорию';
		return currentCategory.Name;
	}
}

function defaultCategory(): CategoryResponse | null | undefined {
	return undefined;
}

function defaultCategories(): CategoryResponse[] {
	return [];
}
