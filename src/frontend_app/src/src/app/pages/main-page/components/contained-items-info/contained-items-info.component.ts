import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { ContainedItemsService } from '../../services/contained-items-service';
import { GetContainedItemsByTypeResponse } from '../../types/GetContainedItemsByTypeResponse';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-contained-items-info',
  imports: [],
  templateUrl: './contained-items-info.component.html',
  styleUrl: './contained-items-info.component.scss',
})
export class ContainedItemsInfoComponent {
  private readonly _service: ContainedItemsService;
  private readonly _containedItemsInfo: WritableSignal<
    GetContainedItemsByTypeResponse[]
  >;
  private readonly _destoryRef: DestroyRef = inject(DestroyRef);
  constructor(private service: ContainedItemsService) {
    this._service = service;
    this._containedItemsInfo = signal([]);
    effect(() => {
      service
        .fetchCount()
        .pipe(takeUntilDestroyed(this._destoryRef))
        .subscribe({
          next: (data: GetContainedItemsByTypeResponse[]): void => {
            this._containedItemsInfo.set(data);
          },
        });
    });
  }

  public get vehiclesInfo(): string {
    const info: GetContainedItemsByTypeResponse[] = this._containedItemsInfo();
    const index: number = info.findIndex(
      (item: GetContainedItemsByTypeResponse): boolean =>
        item.type === 'Техника',
    );
    return index < 0 ? '0' : info[index].amount;
  }

  public get sparesInfo(): string {
    const info: GetContainedItemsByTypeResponse[] = this._containedItemsInfo();
    const index: number = info.findIndex(
      (item: GetContainedItemsByTypeResponse): boolean =>
        item.type === 'Запчасти',
    );
    return index < 0 ? '0' : info[index].amount;
  }
}
