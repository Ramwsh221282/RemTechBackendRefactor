import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-state-select',
  imports: [FormsModule, NgClass],
  templateUrl: './scraper-state-select.component.html',
  styleUrl: './scraper-state-select.component.scss',
})
export class ScraperStateSelectComponent {
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<ParserResponse>;

  constructor() {
    this._scraper = signal(DefaultParserResponse());
  }

  public get currentState(): string {
    return this._scraper().State;
  }
}
