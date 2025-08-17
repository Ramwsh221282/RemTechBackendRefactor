import {
  Component,
  DestroyRef,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { Cleaner } from '../../types/Cleaner';
import { CleanersService } from '../../services/CleanersService';
import { Select, SelectChangeEvent } from 'primeng/select';
import { Toast } from 'primeng/toast';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';

@Component({
  selector: 'app-cleaner-wait-days-select',
  imports: [Select, Toast, FormsModule],
  templateUrl: './cleaner-wait-days-select.component.html',
  styleUrl: './cleaner-wait-days-select.component.scss',
  providers: [MessageService],
})
export class CleanerWaitDaysSelectComponent implements OnInit {
  @Output() cleanerUpdated: EventEmitter<Cleaner> = new EventEmitter();
  @Input({ required: true }) set cleaner_setter(value: Cleaner) {
    this._cleaner.set(value);
  }

  private readonly _cleaner: WritableSignal<Cleaner>;
  private readonly _scraperWaitDays: WritableSignal<number[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly _messageService: MessageService,
    private readonly _service: CleanersService,
  ) {
    this._cleaner = signal(CleanersService.default());
    this._scraperWaitDays = signal([]);
  }

  public get cleaner(): Cleaner {
    return this._cleaner();
  }

  public onSelect($event: SelectChangeEvent): void {
    const waitDays: number = $event.value as number;
    this._service
      .changeWaitDays(waitDays)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (cleaner: Cleaner): void => {
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Изменено время ожидания чистильщика: на ${cleaner.waitDays}`,
          );
          this.cleanerUpdated.emit(cleaner);
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }

  public ngOnInit(): void {
    this._scraperWaitDays.set([1, 2, 3, 4, 5, 6, 7]);
  }

  public get waitDaysOptions(): number[] {
    return this._scraperWaitDays();
  }

  public get currentWaitDays(): number {
    return this._cleaner().waitDays;
  }
}
