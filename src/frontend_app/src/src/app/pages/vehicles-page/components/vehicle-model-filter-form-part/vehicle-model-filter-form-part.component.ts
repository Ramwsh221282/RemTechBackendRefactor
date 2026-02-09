import { Component, EventEmitter, input, InputSignal, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select, SelectChangeEvent } from 'primeng/select';
import { ModelResponse } from '../../../../shared/api/models-module/models-responses';

@Component({
	selector: 'app-vehicle-model-filter-form-part',
	imports: [FormsModule, Select],
	templateUrl: './vehicle-model-filter-form-part.component.html',
	styleUrl: './vehicle-model-filter-form-part.component.scss',
})
export class VehicleModelFilterFormPartComponent {
	disabled: InputSignal<boolean> = input(false);
	models: InputSignal<ModelResponse[]> = input<ModelResponse[]>([]);
	currentModel: InputSignal<ModelResponse | null | undefined> = input<ModelResponse | null | undefined>(null);
	@Output() modelSelected: EventEmitter<ModelResponse | null | undefined> = new EventEmitter<ModelResponse | null | undefined>();

	public onChange($event: SelectChangeEvent): void {
		const model: ModelResponse | null | undefined = $event.value;
		this.modelSelected.emit(model);
	}

	get placeHolderText(): string {
		const currentModel: ModelResponse | null | undefined = this.currentModel();
		if (!currentModel) return 'Выбрать модель';
		return currentModel.Name;
	}
}
