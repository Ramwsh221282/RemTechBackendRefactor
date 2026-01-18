import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Router } from '@angular/router';
import { CategoriesPopularity } from '../../../../shared/api/main-page/main-page-responses';

@Component({
  selector: 'app-popular-categories-block',
  imports: [],
  templateUrl: './popular-categories-block.component.html',
  styleUrl: './popular-categories-block.component.scss',
})
export class PopularCategoriesBlockComponent {
  constructor(private readonly _router: Router) {
    this.categories = signal([]);
  }

  readonly categories: WritableSignal<CategoriesPopularity[]>;

  public navigateAll(): void {
    this._router.navigate(['categories/all']);
  }

  public navigateByCategory(category: CategoriesPopularity): void {
    // this._router.navigate(['vehicles'], {
    //   queryParams: {
    //     categoryId: category.id,
    //     categoryName: category.name,
    //     page: 1,
    //   },
    // });
  }

  @Input({ required: true }) set categories_setter(
    value: CategoriesPopularity[],
  ) {
    this.categories.set(value.slice(0, 4));
  }
}
