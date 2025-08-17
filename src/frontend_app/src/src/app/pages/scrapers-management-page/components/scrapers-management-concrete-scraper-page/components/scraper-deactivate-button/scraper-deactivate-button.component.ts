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
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { Toast } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ParserStateChangeResult } from '../../../scrapers-management-settings-page/types/ParserStateChangedResult';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { ConfirmDialog } from 'primeng/confirmdialog';

@Component({
  selector: 'app-scraper-deactivate-button',
  imports: [Toast, ConfirmDialog],
  templateUrl: './scraper-deactivate-button.component.html',
  styleUrl: './scraper-deactivate-button.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class ScraperDeactivateButtonComponent {
  @Output() scraperDeactivated: EventEmitter<Scraper> = new EventEmitter();
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }
  private readonly _scraper: WritableSignal<Scraper>;
  private readonly _messageService: MessageService;
  private readonly _service: VehicleScrapersService;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _confirmationService: ConfirmationService;
  constructor(
    messageService: MessageService,
    service: VehicleScrapersService,
    confirmationService: ConfirmationService,
  ) {
    this._confirmationService = confirmationService;
    this._messageService = messageService;
    this._service = service;
    this._scraper = signal(VehicleScrapersService.defaultScraper());
  }

  public click($event: Event): void {
    const current: Scraper = this._scraper();
    if (current.state === 'Работает') {
      this._confirmationService.confirm({
        target: $event.target as EventTarget,
        message: 'Вы действительно хотите остановить работу парсера?',
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
          this.invokeChangeState(current);
        },
        reject: () => {},
      });
    } else {
      this.invokeChangeState(current);
    }
  }

  private invokeChangeState(current: Scraper): void {
    this._service
      .changeState(current, { stateSwitch: false })
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (data: ParserStateChangeResult): void => {
          current.state = data.newState;
          this.scraperDeactivated.emit(current);
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Состояние парсера ${current.name} ${current.type} изменено на: ${data.newState}`,
          );
        },
        error: (err: HttpErrorResponse): void => {
          const message: string = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }

  public get domain(): string {
    return this._scraper().domain;
  }
}
