import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CatalogueCategory } from '../../types/CatalogueCategory';
import { Select, SelectChangeEvent } from 'primeng/select';
import { CategoryResponse } from '../../../../shared/api/categories-module/categories-responses';

@Component({
	selector: 'app-vehicle-category-filter-form-part',
	imports: [FormsModule, Select],
	templateUrl: './vehicle-category-filter-form-part.component.html',
	styleUrl: './vehicle-category-filter-form-part.component.scss',
})
export class VehicleCategoryFilterFormPartComponent {
	readonly currentCategory: WritableSignal<CategoryResponse | null | undefined> = signal(undefined);
	readonly categories: WritableSignal<CatalogueCategory[]> = signal([]);

	@Input({ required: true }) set current_category(value: CategoryResponse | null | undefined) {
		this.currentCategory.set(value);
	}

	@Output() onCategorySelect: EventEmitter<string | null | undefined> = new EventEmitter();

	public onChange($event: SelectChangeEvent): void {
		const category: CatalogueCategory | null = $event.value as CatalogueCategory;
		if (category) {
			this.onCategorySelect.emit(category.id);
			return;
		}
		this.onCategorySelect.emit(undefined);
	}
}
