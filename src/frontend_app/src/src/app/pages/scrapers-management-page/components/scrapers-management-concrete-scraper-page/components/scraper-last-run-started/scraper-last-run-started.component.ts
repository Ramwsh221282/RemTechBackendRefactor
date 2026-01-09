import {Component, Input, signal, WritableSignal} from '@angular/core';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-scraper-last-run-started',
  imports: [
    DatePipe
  ],
  templateUrl: './scraper-last-run-started.component.html',
  styleUrl: './scraper-last-run-started.component.scss'
})
export class ScraperLastRunStartedComponent {
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<ParserResponse>;

  constructor() {
    this._scraper = signal(DefaultParserResponse());
  }

  public get lastRunStarted(): Date | null | undefined {
    return this._scraper().StartedAt;
  }
}
