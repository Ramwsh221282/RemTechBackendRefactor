import {
  Component,
  DestroyRef,
  EventEmitter,
  inject,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { Cleaner } from '../../types/Cleaner';
import { CleanersService } from '../../services/CleanersService';
import { MessageService } from 'primeng/api';
import { Toast } from 'primeng/toast';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';

@Component({
  selector: 'app-cleaner-activate-button',
  imports: [Toast],
  templateUrl: './cleaner-activate-button.component.html',
  styleUrl: './cleaner-activate-button.component.scss',
  providers: [MessageService],
})
export class CleanerActivateButtonComponent {
  @Output() cleanerUpdated: EventEmitter<Cleaner> = new EventEmitter<Cleaner>();
  @Input({ required: true }) set cleaner_setter(value: Cleaner) {
    this._cleaner.set(value);
  }
  private readonly _cleaner: WritableSignal<Cleaner>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly _messageService: MessageService,
    private readonly _service: CleanersService,
  ) {
    this._cleaner = signal(CleanersService.default());
  }

  public get cleaner(): Cleaner {
    return this._cleaner();
  }

  public click(): void {
    this._service
      .enable()
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (cleaner: Cleaner): void => {
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Чистильщик переведен в состояние ожидания запуска.`,
          );
          this.cleanerUpdated.emit(cleaner);
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }
}
