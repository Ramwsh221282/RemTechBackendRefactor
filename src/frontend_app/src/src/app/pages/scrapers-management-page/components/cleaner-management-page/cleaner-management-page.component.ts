import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { Toast } from 'primeng/toast';
import { Cleaner } from './types/Cleaner';
import { CleanersService } from './services/CleanersService';
import { CleanerStateSelectComponent } from './components/cleaner-state-select/cleaner-state-select.component';
import { CleanerActivateButtonComponent } from './components/cleaner-activate-button/cleaner-activate-button.component';
import { CleanerDeactivateButtonComponent } from './components/cleaner-deactivate-button/cleaner-deactivate-button.component';
import { CleanerLastRunInfoComponent } from './components/cleaner-last-run-info/cleaner-last-run-info.component';
import { CleanerNextRunInfoComponent } from './components/cleaner-next-run-info/cleaner-next-run-info.component';
import { CleanerWaitDaysSelectComponent } from './components/cleaner-wait-days-select/cleaner-wait-days-select.component';
import { CleanerProcessedAmountComponent } from './components/cleaner-processed-amount/cleaner-processed-amount.component';
import { MessageService } from 'primeng/api';
import { CleanerElapsedTimeComponent } from './components/cleaner-elapsed-time/cleaner-elapsed-time.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { CleanerThresholdComponent } from './components/cleaner-threshold/cleaner-threshold.component';

@Component({
  selector: 'app-cleaner-management-page',
  imports: [
    Toast,
    CleanerStateSelectComponent,
    CleanerActivateButtonComponent,
    CleanerDeactivateButtonComponent,
    CleanerLastRunInfoComponent,
    CleanerNextRunInfoComponent,
    CleanerWaitDaysSelectComponent,
    CleanerProcessedAmountComponent,
    CleanerElapsedTimeComponent,
    CleanerThresholdComponent,
  ],
  templateUrl: './cleaner-management-page.component.html',
  styleUrl: './cleaner-management-page.component.scss',
  providers: [MessageService],
})
export class CleanerManagementPageComponent {
  private readonly _cleaner: WritableSignal<Cleaner>;
  private readonly _service: CleanersService;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    service: CleanersService,
    private readonly _messageService: MessageService,
  ) {
    this._cleaner = signal(CleanersService.default());
    this._service = service;
    effect(() => {
      service
        .fetch()
        .pipe()
        .subscribe({
          next: (cleaner: Cleaner): void => {
            this._cleaner.set(cleaner);
          },
        });
    });
  }

  public acceptCleanerUpdate($event: Cleaner): void {
    this._cleaner.set($event);
  }

  public instantlyEnableClick(): void {
    this._service
      .instantlyEnable()
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (cleaner: Cleaner): void => {
          this._cleaner.set(cleaner);
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Чистильщик был немедленно включен.`,
          );
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }

  public get cleaner(): Cleaner {
    return this._cleaner();
  }
}
