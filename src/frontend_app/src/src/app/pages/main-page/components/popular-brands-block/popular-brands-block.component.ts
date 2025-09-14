import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { PopularBrand } from '../../types/PopularBrand';
import { PopularBrandsService } from '../../services/popular-brands-service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';

@Component({
  selector: 'app-popular-brands-block',
  imports: [],
  templateUrl: './popular-brands-block.component.html',
  styleUrl: './popular-brands-block.component.scss',
})
export class PopularBrandsBlockComponent {
  private readonly _brands: WritableSignal<PopularBrand[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _router: Router;
  constructor(service: PopularBrandsService, router: Router) {
    this._router = router;
    this._brands = signal([]);
    effect(() => {
      service
        .fetch()
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe((data: PopularBrand[]): void => {
          this._brands.set(data);
        });
    });
  }

  public get brands(): PopularBrand[] {
    return this._brands();
  }

  public navigateAllClick(): void {
    this._router.navigate(['brands/all']);
  }

  public navigateByBrand(brand: PopularBrand): void {
    this._router.navigate(['vehicles'], {
      queryParams: {
        brandId: brand.id,
        brandName: brand.name,
        page: 1,
      },
    });
  }
}
