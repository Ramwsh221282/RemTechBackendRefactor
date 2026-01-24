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
	properties: InputSignal<VehicleModelFilterFormPartComponentProps> = input(defaultProps());
	@Output() modelSelected: EventEmitter<string | null | undefined> = new EventEmitter<string | null | undefined>();
	public onChange($event: SelectChangeEvent): void {}

	get placeHolderText(): string {
		const currentModel: ModelResponse | null | undefined = this.properties().currentModel;
		if (!currentModel) return 'Выбрать модель';
		return currentModel.Name;
	}
}

type VehicleModelFilterFormPartComponentProps = {
	currentModel: ModelResponse | null | undefined;
	models: ModelResponse[];
};

const defaultProps: () => VehicleModelFilterFormPartComponentProps = (): VehicleModelFilterFormPartComponentProps => {
	return {
		currentModel: null,
		models: [],
	};
};
