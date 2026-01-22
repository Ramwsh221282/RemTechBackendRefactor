import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select, SelectChangeEvent } from 'primeng/select';
import { CatalogueModel } from '../../types/CatalogueModel';
import { ModelResponse } from '../../../../shared/api/models-module/models-responses';

@Component({
	selector: 'app-vehicle-model-filter-form-part',
	imports: [FormsModule, Select],
	templateUrl: './vehicle-model-filter-form-part.component.html',
	styleUrl: './vehicle-model-filter-form-part.component.scss',
})
export class VehicleModelFilterFormPartComponent {
	@Output() modelSelected: EventEmitter<string | null | undefined> = new EventEmitter<string | null | undefined>();
	readonly currentModel: WritableSignal<ModelResponse | null | undefined> = signal(undefined);
	readonly models: WritableSignal<CatalogueModel[]> = signal([]);
	@Input({ required: true }) set current_model(value: ModelResponse | null | undefined) {
		this.currentModel.set(value);
	}

	public onChange($event: SelectChangeEvent): void {
		const model: CatalogueModel | null = $event.value as CatalogueModel;
		if (model) {
			this.modelSelected.emit(model.id);
			return;
		}
		this.modelSelected.emit(undefined);
	}
}
