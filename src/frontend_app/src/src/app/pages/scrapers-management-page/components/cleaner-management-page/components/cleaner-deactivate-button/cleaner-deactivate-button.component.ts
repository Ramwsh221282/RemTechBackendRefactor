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
import { ConfirmDialog } from 'primeng/confirmdialog';
import { Toast } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';

@Component({
  selector: 'app-cleaner-deactivate-button',
  imports: [ConfirmDialog, Toast],
  templateUrl: './cleaner-deactivate-button.component.html',
  styleUrl: './cleaner-deactivate-button.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class CleanerDeactivateButtonComponent {
  @Output() cleanerUpdated: EventEmitter<Cleaner>;
  @Input({ required: true }) set cleaner_setter(value: Cleaner) {
    this._cleaner.set(value);
  }
  private readonly _cleaner: WritableSignal<Cleaner>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly _messageService: MessageService,
    private readonly _confirmationService: ConfirmationService,
    private readonly _service: CleanersService,
  ) {
    this.cleanerUpdated = new EventEmitter<Cleaner>();
    this._cleaner = signal(CleanersService.default());
  }

  public get cleaner(): Cleaner {
    return this._cleaner();
  }

  public click($event: Event): void {
    const currentState = this._cleaner().state;
    if (currentState === 'Работает') {
      this._confirmationService.confirm({
        target: $event.target as EventTarget,
        message: 'Вы действительно хотите остановить работу чистильщика?',
        header: 'Подтверждение',
        closable: true,
        closeOnEscape: true,
        icon: 'pi pi-exclamation-triangle',
        rejectButtonProps: {
          label: 'Нет',
          severity: 'secondary',
          outlined: true,
        },
        acceptButtonProps: {
          label: 'Да',
        },
        accept: () => {
          this.initDisabling();
        },
        reject: () => {},
      });
    } else {
      this.initDisabling();
    }
  }

  private initDisabling(): void {
    this._service
      .disable()
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (cleaner: Cleaner): void => {
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Чистильщик отключен`,
          );
          this.cleanerUpdated.emit(cleaner);
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }
}
