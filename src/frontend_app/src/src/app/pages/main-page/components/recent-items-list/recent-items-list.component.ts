import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { ReceintItemCardComponent } from '../receint-item-card/receint-item-card.component';
import { VehicleResponse } from '../../../../shared/api/vehicles-module/vehicles-api.responses';
import { SpareResponse } from '../../../../shared/api/spares-module/spares-api.responses';
import { RecentSpareCardComponent } from '../receint-item-card/recent-spare-card/recent-spare-card.component';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';

@Component({
	selector: 'app-recent-items-list',
	imports: [ReceintItemCardComponent, RecentSpareCardComponent, PaginationComponent],
	templateUrl: './recent-items-list.component.html',
	styleUrl: './recent-items-list.component.scss',
})
export class RecentItemsListComponent {
	@Output() sparePageChanged: EventEmitter<number> = new EventEmitter<number>();

	@Output() vehiclePageChanged: EventEmitter<number> = new EventEmitter<number>();

	@Input({ required: true }) set spares_pagination_props(value: { page: number; totalCount: number; pageSize: number }) {
		this.sparesPaginationProps.set(value);
	}

	@Input({ required: true }) set vehicles_pagination_props(value: { page: number; totalCount: number; pageSize: number }) {
		this.vehiclesPaginationProps.set(value);
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
		this.vehicles.set(value);
	}

	readonly watchingVehicles: WritableSignal<boolean> = signal(false);
	readonly watchingSpares: WritableSignal<boolean> = signal(false);
	readonly vehicles: WritableSignal<VehicleResponse[]> = signal([]);
	readonly spares: WritableSignal<SpareResponse[]> = signal([]);

	readonly vehiclesPaginationProps: WritableSignal<{
		page: number;
		totalCount: number;
		pageSize: number;
	}> = signal({
		page: 0,
		totalCount: 0,
		pageSize: 0,
	});
	readonly sparesPaginationProps: WritableSignal<{
		page: number;
		totalCount: number;
		pageSize: number;
	}> = signal({
		page: 0,
		totalCount: 0,
		pageSize: 0,
	});

	public handleSparesPageChange($event: number): void {
		this.updatePaginationForSpares($event);
		this.emitSparesPageChange();
	}

	public handleVehiclesPageChange($event: number): void {
		this.updatePaginationForVehicles($event);
		this.emitVehiclesPageChange();
	}

	private updatePaginationForSpares(page: number): void {
		this.sparesPaginationProps.update((props) => {
			return { ...props, page: page };
		});
	}

	private updatePaginationForVehicles(page: number): void {
		this.vehiclesPaginationProps.update((props) => {
			return { ...props, page: page };
		});
	}

	private emitSparesPageChange(): void {
		const currentPage: number = this.sparesPaginationProps().page;
		this.sparePageChanged.emit(currentPage);
	}

	private emitVehiclesPageChange(): void {
		const currentPage: number = this.vehiclesPaginationProps().page;
		this.vehiclePageChanged.emit(currentPage);
	}
}
