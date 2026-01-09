import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-processed-amount',
  imports: [],
  templateUrl: './scraper-processed-amount.component.html',
  styleUrl: './scraper-processed-amount.component.scss',
})
export class ScraperProcessedAmountComponent {
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<ParserResponse>;

  constructor() {
    this._scraper = signal(DefaultParserResponse());
  }

  public get processedAmount(): number {
    return this._scraper().ParsedCount;
  }
}
