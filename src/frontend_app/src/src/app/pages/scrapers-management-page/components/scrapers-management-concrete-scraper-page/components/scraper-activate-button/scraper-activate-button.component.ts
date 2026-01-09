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
import { MessageService } from 'primeng/api';
import { takeUntil } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ParserStateChangeResult } from '../../../scrapers-management-settings-page/types/ParserStateChangedResult';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { Toast } from 'primeng/toast';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {ParsersControlApiService} from '../../../../../../shared/api/parsers-module/parsers-control-api.service';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-activate-button',
  imports: [Toast],
  templateUrl: './scraper-activate-button.component.html',
  styleUrl: './scraper-activate-button.component.scss',
  providers: [MessageService],
})
export class ScraperActivateButtonComponent {
  constructor() {
    this._scraper = signal(DefaultParserResponse());
    this.parserStartClicked = new EventEmitter<ParserResponse>();
  }

  private readonly _scraper: WritableSignal<ParserResponse>;
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }
  @Output() parserStartClicked: EventEmitter<ParserResponse>;
  public click(): void {
    this.parserStartClicked.emit(this._scraper());
  }
}
