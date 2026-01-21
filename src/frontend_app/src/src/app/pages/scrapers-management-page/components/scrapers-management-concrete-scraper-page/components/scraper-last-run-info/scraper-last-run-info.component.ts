import { Component, Input, signal, WritableSignal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ParserResponse } from '../../../../../../shared/api/parsers-module/parsers-responses';
import { DefaultParserResponse } from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-last-run-info',
  imports: [DatePipe],
  templateUrl: './scraper-last-run-info.component.html',
  styleUrl: './scraper-last-run-info.component.scss',
})
export class ScraperLastRunInfoComponent {
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<ParserResponse>;

  constructor() {
    this._scraper = signal(DefaultParserResponse());
  }

  public get lastRunDate(): Date | null | undefined {
    return this._scraper().FinishedAt;
  }

  public click(): void {}
}
