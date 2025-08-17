import {
  Component,
  DestroyRef,
  effect,
  EventEmitter,
  inject,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CatalogueVehiclesService } from '../../services/CatalogueVehiclesService';
import { CatalogueCategory } from '../../types/CatalogueCategory';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Select, SelectChangeEvent } from 'primeng/select';

@Component({
  selector: 'app-vehicle-category-filter-form-part',
  imports: [FormsModule, Select],
  templateUrl: './vehicle-category-filter-form-part.component.html',
  styleUrl: './vehicle-category-filter-form-part.component.scss',
})
export class VehicleCategoryFilterFormPartComponent {
  @Input({ required: true }) set current_categoryId(
    categoryId: string | undefined,
  ) {
    this._currentCategoryId.set(categoryId);
  }

  @Output() onCategorySelect: EventEmitter<string | undefined> =
    new EventEmitter();
  private readonly _currentCategoryId: WritableSignal<string | undefined> =
    signal(undefined);
  private readonly _categories: WritableSignal<CatalogueCategory[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(service: CatalogueVehiclesService) {
    this._categories = signal([]);
    effect(() => {
      service
        .fetchCategories()
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: CatalogueCategory[]): void => {
            this._categories.set(data);
            const current: string | undefined = this._currentCategoryId();
            if (current) {
              const index = data.findIndex((c) => c.id === current);
              if (index >= 0) {
                this.selectedCategory = data[index];
              }
            }
          },
        });
    });
  }

  public selectedCategory: CatalogueCategory | undefined;

  public get categories(): CatalogueCategory[] {
    return this._categories();
  }

  public onChange($event: SelectChangeEvent): void {
    const category: CatalogueCategory | null =
      $event.value as CatalogueCategory;
    if (category) {
      this.onCategorySelect.emit(category.id);
      return;
    }
    this.onCategorySelect.emit(undefined);
  }
}
