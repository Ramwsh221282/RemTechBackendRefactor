import { Component, Input, signal, WritableSignal } from '@angular/core';
import { CatalogueVehicle } from '../../types/CatalogueVehicle';
import { CatalogueVehiclesService } from '../../services/CatalogueVehiclesService';
import { DecimalPipe, NgForOf, NgIf } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-vehicle-card',
  imports: [DecimalPipe, NgForOf, NgIf],
  templateUrl: './vehicle-card.component.html',
  styleUrl: './vehicle-card.component.scss',
})
export class VehicleCardComponent {
  @Input({ required: true }) set vehicle_setter(vehicle: CatalogueVehicle) {
    this._vehicle.set(vehicle);
  }
  private readonly _vehicle: WritableSignal<CatalogueVehicle>;
  private readonly _router: Router;
  constructor(router: Router) {
    this._router = router;
    this._vehicle = signal(CatalogueVehiclesService.default());
  }

  public get vehicle(): CatalogueVehicle {
    return this._vehicle();
  }

  openInNewTab() {
    const url = this._router
      .createUrlTree(['/vehicle'], {
        queryParams: { vehicleId: this.vehicle.id },
      })
      .toString();

    window.open(url, '_blank', 'noopener,noreferrer');
  }
}
