import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Router } from '@angular/router';
import { BrandsPopularity } from '../../../../shared/api/main-page/main-page-responses';

@Component({
  selector: 'app-popular-brands-block',
  imports: [],
  templateUrl: './popular-brands-block.component.html',
  styleUrl: './popular-brands-block.component.scss',
})
export class PopularBrandsBlockComponent {
  constructor(private readonly _router: Router) {
    this.brands = signal([]);
  }

  public navigateAllClick(): void {
    this._router.navigate(['brands/all']);
  }

  public navigateByBrand(brand: BrandsPopularity): void {
    this._router.navigate(['vehicles'], {
      queryParams: {
        brandId: brand.Id,
        brandName: brand.Name,
        page: 1,
      },
    });
  }

  readonly brands: WritableSignal<BrandsPopularity[]>;

  @Input({ required: true }) set brands_setter(value: BrandsPopularity[]) {
    this.brands.set(value.slice(0, 4));
  }
}
