import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select, SelectChangeEvent, SelectFilterEvent } from 'primeng/select';
import { LocationResponse } from '../../../../shared/api/locations-module/locations.responses';
import { StringUtils } from '../../../../shared/utils/string-utils';

export type VehicleRegionsFilterFormPartProps = {
    locations: LocationResponse[];
    selectedLocation: LocationResponse | null | undefined;
};

export const DefaultVehicleRegionsFilterFormPartProps: VehicleRegionsFilterFormPartProps = {
    locations: [],
    selectedLocation: undefined,
};

@Component({
    selector: 'app-vehicle-regions-filter-form-part',
    imports: [FormsModule, Select],
    templateUrl: './vehicle-regions-filter-form-part.component.html',
    styleUrl: './vehicle-regions-filter-form-part.component.scss',
})
export class VehicleRegionsFilterFormPartComponent {
    readonly selectedLocation: WritableSignal<LocationResponse | null | undefined> = signal<
        LocationResponse | null | undefined
    >(undefined);

    readonly locations: WritableSignal<LocationResponse[]> = signal([]);

    @Input({ required: true }) set location_props(value: VehicleRegionsFilterFormPartProps) {
        this.locations.set(value.locations);
        this.selectedLocation.set(value.selectedLocation);
    }

    @Output() locationSelected: EventEmitter<LocationResponse | null | undefined> = new EventEmitter<
        LocationResponse | null | undefined
    >();
    @Output() locationFilterTyped: EventEmitter<string | null | undefined> = new EventEmitter<
        string | null | undefined
    >();

    public userChangesLocation($event: SelectChangeEvent): void {}

    public handleUserFiltersLocation($event: SelectFilterEvent): void {
        const input: string = $event.filter;
        if (StringUtils.isEmptyOrWhiteSpace(input)) {
            this.locationFilterTyped.emit(undefined);
            return;
        }
        this.locationFilterTyped.emit(input);
    }
}
