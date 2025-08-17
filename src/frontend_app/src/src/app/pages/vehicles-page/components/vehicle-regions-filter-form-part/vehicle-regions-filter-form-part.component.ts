import {
  Component,
  DestroyRef,
  effect,
  EventEmitter,
  inject,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CatalogueModel } from '../../types/CatalogueModel';
import { CatalogueVehiclesService } from '../../services/CatalogueVehiclesService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Select, SelectChangeEvent } from 'primeng/select';
import { CatalogueRegion } from '../../types/CatalogueRegion';

@Component({
  selector: 'app-vehicle-regions-filter-form-part',
  imports: [FormsModule, Select],
  templateUrl: './vehicle-regions-filter-form-part.component.html',
  styleUrl: './vehicle-regions-filter-form-part.component.scss',
})
export class VehicleRegionsFilterFormPartComponent {
  @Output() onRegionSelect: EventEmitter<string | undefined> =
    new EventEmitter();
  private readonly _regions: WritableSignal<CatalogueRegion[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(service: CatalogueVehiclesService) {
    this._regions = signal([]);
    effect(() => {
      service
        .fetchRegions()
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: CatalogueRegion[]): void => {
            this._regions.set(data);
          },
        });
    });
  }

  public selectedRegion: CatalogueRegion | undefined;

  public get regions(): CatalogueRegion[] {
    return this._regions();
  }

  public onChange($event: SelectChangeEvent): void {
    const model: CatalogueRegion | null = $event.value as CatalogueRegion;
    if (model) {
      this.onRegionSelect.emit(model.id);
      return;
    }
    this.onRegionSelect.emit(undefined);
  }
}
