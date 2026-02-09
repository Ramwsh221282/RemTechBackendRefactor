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
import { MessageService } from 'primeng/api';
import { FormsModule } from '@angular/forms';
import { Select, SelectChangeEvent } from 'primeng/select';
import { Toast } from 'primeng/toast';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-cleaner-threshold',
  imports: [FormsModule, Select, Toast],
  templateUrl: './cleaner-threshold.component.html',
  styleUrl: './cleaner-threshold.component.scss',
  providers: [MessageService],
})
export class CleanerThresholdComponent implements OnInit {
  @Output() cleanerUpdated: EventEmitter<Cleaner>;
  @Input({ required: true }) set cleaner_setter(value: Cleaner) {
    this._cleaner.set(value);
  }

  private readonly _cleaner: WritableSignal<Cleaner>;
  private readonly _tresholds: WritableSignal<number[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly _messageService: MessageService,
    private readonly _service: CleanersService,
  ) {
    this.cleanerUpdated = new EventEmitter<Cleaner>();
    this._cleaner = signal(CleanersService.default());
    this._tresholds = signal([]);
  }

  public get cleaner(): Cleaner {
    return this._cleaner();
  }

  public onSelect($event: SelectChangeEvent): void {
    const treshold: number = $event.value as number;
    this._service
      .changeThreshold(treshold)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (cleaner: Cleaner): void => {
          this.cleanerUpdated.emit(cleaner);
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Чистильщик будет проверять объявления которые содержатся более ${treshold} дней.`,
          );
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }

  public ngOnInit(): void {
    this._tresholds.set([1, 2, 3, 4, 5, 6, 7]);
  }

  public get waitDaysOptions(): number[] {
    return this._tresholds();
  }

  public get currentWaitDays(): number {
    return this._cleaner().waitDays;
  }
}
