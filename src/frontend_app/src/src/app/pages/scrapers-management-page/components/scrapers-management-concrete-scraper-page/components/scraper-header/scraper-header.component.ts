import {Component, Input, signal, WritableSignal} from '@angular/core';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-header',
  imports: [],
  templateUrl: './scraper-header.component.html',
  styleUrl: './scraper-header.component.scss'
})
export class ScraperHeaderComponent {
  @Input({required: true}) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<ParserResponse> = signal(DefaultParserResponse());

  get scraperHeader(): string {
    const scraper: ParserResponse = this._scraper();
    return `${scraper.Domain} ${scraper.ServiceType}`;
  }
}
