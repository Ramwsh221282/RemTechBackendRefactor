import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { PopularCategory } from '../../types/PopularCategories';
import { PopularCategoriesService } from '../../services/popular-categories-service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';

@Component({
  selector: 'app-popular-categories-block',
  imports: [],
  templateUrl: './popular-categories-block.component.html',
  styleUrl: './popular-categories-block.component.scss',
})
export class PopularCategoriesBlockComponent {
  private readonly _categories: WritableSignal<PopularCategory[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _router: Router;
  constructor(service: PopularCategoriesService, router: Router) {
    this._router = router;
    this._categories = signal([]);
    effect(() => {
      service
        .fetch()
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: PopularCategory[]): void => {
            this._categories.set(data);
          },
        });
    });
  }

  public navigateAll(): void {
    this._router.navigate(['categories/all']);
  }

  public get categories(): PopularCategory[] {
    return this._categories();
  }

  public navigateByCategory(category: PopularCategory): void {
    this._router.navigate(['vehicles'], {
      queryParams: {
        categoryId: category.id,
        categoryName: category.name,
        page: 1,
      },
    });
  }
}
