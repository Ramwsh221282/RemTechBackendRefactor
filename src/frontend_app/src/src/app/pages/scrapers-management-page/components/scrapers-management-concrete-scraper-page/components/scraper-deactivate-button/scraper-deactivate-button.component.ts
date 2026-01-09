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
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {ParsersControlApiService} from '../../../../../../shared/api/parsers-module/parsers-control-api.service';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-deactivate-button',
  imports: [Toast, ConfirmDialog],
  templateUrl: './scraper-deactivate-button.component.html',
  styleUrl: './scraper-deactivate-button.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class ScraperDeactivateButtonComponent {
  constructor(
    private readonly _messageService: MessageService,
    private readonly _service: ParsersControlApiService,
    private readonly _confirmationService: ConfirmationService,
  ) {
    this.scraperDeactivated = new EventEmitter();
    this._scraper = signal(DefaultParserResponse());
  }

  private readonly _scraper: WritableSignal<ParserResponse>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);

  @Output() scraperDeactivated: EventEmitter<ParserResponse>;
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  public click($event: Event): void {

  }

  private invokeChangeState(current: Scraper): void {

  }

  public get domain(): string {
    return this._scraper().Domain;
  }
}
