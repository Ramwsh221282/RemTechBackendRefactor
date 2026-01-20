import { Component, Input, signal, WritableSignal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { VehicleResponse } from '../../../../shared/api/vehicles-module/vehicles-api.responses';
import { DefaultVehicleResponse } from '../../../../shared/api/vehicles-module/vehicles-api.factories';

@Component({
  selector: 'app-recent-vehicle',
  imports: [DecimalPipe],
  templateUrl: './receint-item-card.component.html',
  styleUrl: './receint-item-card.component.scss',
})
export class ReceintItemCardComponent {
  constructor() {
    this.item = signal(DefaultVehicleResponse());
  }

  @Input({ required: true }) set item_setter(value: VehicleResponse) {
    this.item.set(value);
  }

  readonly item: WritableSignal<VehicleResponse>;

  public get characteristicsText(): string {
    return this.item()
      .Characteristics.map((c) => c.Name + ': ' + c.Value)
      .join(', ');
  }
}
