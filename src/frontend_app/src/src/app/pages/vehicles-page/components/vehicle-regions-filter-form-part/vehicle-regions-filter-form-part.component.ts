import { Component, EventEmitter, input, InputSignal, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select, SelectChangeEvent } from 'primeng/select';
import { LocationResponse } from '../../../../shared/api/locations-module/locations.responses';

@Component({
	selector: 'app-vehicle-regions-filter-form-part',
	imports: [FormsModule, Select],
	templateUrl: './vehicle-regions-filter-form-part.component.html',
	styleUrl: './vehicle-regions-filter-form-part.component.scss',
})
export class VehicleRegionsFilterFormPartComponent {
	disabled: InputSignal<boolean> = input(false);
	selectedLocation: InputSignal<LocationResponse | null | undefined> = input(defaultCurrentLocation());
	locations: InputSignal<LocationResponse[]> = input(defaultLocations());

	@Output() locationSelected: EventEmitter<LocationResponse | null | undefined> = new EventEmitter<LocationResponse | null | undefined>();

	public userChangesLocation($event: SelectChangeEvent): void {
		const location: LocationResponse | null | undefined = $event.value;
		this.locationSelected.emit(location);
	}

	public get placeHolderText(): string {
		if (!this.selectedLocation) return 'Выбрать регион';
		const location: LocationResponse | null | undefined = this.selectedLocation();
		if (!location) return 'Выбрать регион';
		return location.Name;
	}
}

function defaultCurrentLocation(): LocationResponse | null | undefined {
	return undefined;
}

function defaultLocations(): LocationResponse[] {
	return [];
}
