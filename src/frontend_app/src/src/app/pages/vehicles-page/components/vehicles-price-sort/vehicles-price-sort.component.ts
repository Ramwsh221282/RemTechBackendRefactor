import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-vehicles-price-sort',
  imports: [FormsModule],
  templateUrl: './vehicles-price-sort.component.html',
  styleUrl: './vehicles-price-sort.component.scss',
})
export class VehiclesPriceSortComponent {
  @Output() sortChange: EventEmitter<string | undefined> = new EventEmitter();
  public mode: string = '';

  public changeSort(sortValue: string) {
    if (sortValue === 'ASC') {
      this.sortChange.emit(sortValue);
      return;
    }
    if (sortValue === 'DESC') {
      this.sortChange.emit(sortValue);
      return;
    }
    this.sortChange.emit(undefined);
  }
}
