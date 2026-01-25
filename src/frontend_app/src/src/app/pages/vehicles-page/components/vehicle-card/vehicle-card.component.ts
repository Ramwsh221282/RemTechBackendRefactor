import { Component, inject, input, InputSignal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { VehicleResponse } from '../../../../shared/api/vehicles-module/vehicles-api.responses';
import { DefaultVehicleResponse } from '../../../../shared/api/vehicles-module/vehicles-api.factories';

@Component({
	selector: 'app-vehicle-card',
	imports: [DecimalPipe],
	templateUrl: './vehicle-card.component.html',
	styleUrl: './vehicle-card.component.scss',
})
export class VehicleCardComponent {
	private readonly _router: Router = inject(Router);
	vehicle: InputSignal<VehicleResponse> = input(DefaultVehicleResponse());

	openInNewTab(): void {
		const url: string = this._router
			.createUrlTree(['/vehicle'], {
				queryParams: { vehicleId: this.vehicle().VehicleId },
			})
			.toString();

		window.open(url, '_blank', 'noopener,noreferrer');
	}
}
