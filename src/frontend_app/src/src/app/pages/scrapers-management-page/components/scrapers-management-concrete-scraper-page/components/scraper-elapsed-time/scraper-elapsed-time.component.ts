import { Component, Input, signal, WritableSignal } from '@angular/core';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-elapsed-time',
  imports: [],
  templateUrl: './scraper-elapsed-time.component.html',
  styleUrl: './scraper-elapsed-time.component.scss',
})
export class ScraperElapsedTimeComponent {
  constructor() {
    this._scraper = signal(DefaultParserResponse());
  }

  private readonly _scraper: WritableSignal<ParserResponse>;

  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  public get hours(): number {
    return this._scraper().ElapsedHours;
  }

  public get seconds(): number {
    return this._scraper().ElapsedSeconds;
  }

  public get minutes(): number {
    return this._scraper().ElapsedMinutes;
  }
}
