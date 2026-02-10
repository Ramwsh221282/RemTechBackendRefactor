import { Component, Input, signal, WritableSignal } from '@angular/core';
import { DatePipe } from '@angular/common';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-next-run-info',
  imports: [DatePipe],
  templateUrl: './scraper-next-run-info.component.html',
  styleUrl: './scraper-next-run-info.component.scss',
})
export class ScraperNextRunInfoComponent {
  constructor() {
    this._scraper = signal(DefaultParserResponse());
  }

  private readonly _scraper: WritableSignal<ParserResponse>;

  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  public get nextRunDate(): Date | null | undefined {
    return this._scraper().NextRun;
  }
}
