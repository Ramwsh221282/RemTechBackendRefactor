import {
  Component,
  DestroyRef,
  effect,
  inject,
  OnInit,
  signal,
  WritableSignal,
} from '@angular/core';
import { CatalogueVehicle } from '../vehicles-page/types/CatalogueVehicle';
import { CatalogueVehiclesService } from '../vehicles-page/services/CatalogueVehiclesService';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DecimalPipe, NgForOf, NgIf } from '@angular/common';
import { UsersService } from '../sign-in-page/services/UsersService';
import { CookieService } from 'ngx-cookie-service';
import { TokensService } from '../../shared/services/TokensService';
import {
  catchError,
  defer,
  filter,
  forkJoin,
  map,
  Observable,
  of,
  switchMap,
  take,
  timeout,
  timer,
} from 'rxjs';

@Component({
  selector: 'app-concrete-vehicle-page',
  imports: [NgForOf, DecimalPipe, NgIf],
  templateUrl: './concrete-vehicle-page.component.html',
  styleUrl: './concrete-vehicle-page.component.scss',
})
export class ConcreteVehiclePageComponent {
  private readonly _vehicle: WritableSignal<CatalogueVehicle>;
  private readonly _similar: WritableSignal<CatalogueVehicle[]>;
  private readonly _currentPhotoIndex: WritableSignal<number> = signal(0);
  private readonly _id: WritableSignal<string | undefined>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _router: Router;
  constructor(
    activatedRoute: ActivatedRoute,
    service: CatalogueVehiclesService,
    router: Router,
  ) {
    this._router = router;
    this._id = signal(undefined);
    this._vehicle = signal(CatalogueVehiclesService.default());
    this._similar = signal([]);
    effect(() => {
      activatedRoute.queryParams
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (params: any) => {
            const id: string | undefined = params['vehicleId'] as string;
            if (id !== undefined) {
              this._id.set(id);
            }
          },
        });
    });
    effect(() => {
      const id: string | undefined = this._id();
      if (id === undefined) return;
      service
        .fetchConcrete(id)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (item: CatalogueVehicle): void => {
            this._vehicle.set(item);
          },
        });
    });
    effect(() => {
      const vehicle: CatalogueVehicle = this._vehicle();
      if (vehicle.description !== '') {
        service
          .fetchSimilar(vehicle.id, vehicle.description)
          .pipe(takeUntilDestroyed(this._destroyRef))
          .subscribe({
            next: (data: CatalogueVehicle[]): void => {
              this._similar.set(data);
            },
          });
      }
    });
  }

  public get currentPhotoIndex(): number {
    return this._currentPhotoIndex();
  }

  public setCurrentPhoto(index: number): void {
    this._currentPhotoIndex.set(index);
  }

  public nextPhoto(): void {
    const currentIndex = this._currentPhotoIndex();
    const maxIndex = this.vehicle.photos.length - 1;
    this._currentPhotoIndex.set(currentIndex < maxIndex ? currentIndex + 1 : 0);
  }

  public prevPhoto(): void {
    const currentIndex = this._currentPhotoIndex();
    this._currentPhotoIndex.set(currentIndex > 0 ? currentIndex - 1 : 0);
  }

  public moveToSimilar(similar: CatalogueVehicle): void {
    const url = this._router
      .createUrlTree(['/vehicle'], {
        queryParams: { vehicleId: similar.id },
      })
      .toString();

    window.open(url, '_blank', 'noopener,noreferrer');
  }

  public get similar(): CatalogueVehicle[] {
    return this._similar();
  }

  public get vehicle(): CatalogueVehicle {
    return this._vehicle();
  }
}
