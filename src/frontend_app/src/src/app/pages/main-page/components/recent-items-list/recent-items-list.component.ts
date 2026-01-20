import { Component, Input, signal, WritableSignal } from '@angular/core';
import { ReceintItemCardComponent } from '../receint-item-card/receint-item-card.component';
import { ReceintItemsPaginationComponent } from '../receint-items-pagination/receint-items-pagination.component';
import { VehicleResponse } from '../../../../shared/api/vehicles-module/vehicles-api.responses';
import { SpareResponse } from '../../../../shared/api/spares-module/spares-api.responses';
import { RecentSpareCardComponent } from '../receint-item-card/recent-spare-card/recent-spare-card.component';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-recent-items-list',
  imports: [
    ReceintItemCardComponent,
    ReceintItemsPaginationComponent,
    RecentSpareCardComponent,
    JsonPipe,
  ],
  templateUrl: './recent-items-list.component.html',
  styleUrl: './recent-items-list.component.scss',
})
export class RecentItemsListComponent {
  constructor() {
    this._page = signal(1);
    this.vehicles = signal([]);
    this.spares = signal([]);
    this.watchingVehicles = signal(false);
    this.watchingSpares = signal(false);
  }

  public readSpares(): SpareResponse[] {
    const spares = this.spares();
    console.log(spares);
    return spares;
  }

  @Input({ required: true }) set watching_vehicles(value: boolean) {
    this.watchingVehicles.set(value);
  }

  @Input({ required: true }) set watching_spares(value: boolean) {
    this.watchingSpares.set(value);
  }

  @Input({ required: true }) set spares_setter(value: SpareResponse[]) {
    this.spares.set(value);
  }

  @Input({ required: true }) set vehicles_setter(value: VehicleResponse[]) {
    console.log(value);
    this.vehicles.set(value);
  }

  readonly watchingVehicles: WritableSignal<boolean>;
  readonly watchingSpares: WritableSignal<boolean>;
  readonly vehicles: WritableSignal<VehicleResponse[]>;
  readonly spares: WritableSignal<SpareResponse[]>;

  private readonly _page: WritableSignal<number>;

  public get page(): number {
    return this._page();
  }

  public acceptPageChange($event: number): void {
    this._page.set($event);
  }
}
