import {
  Component,
  EventEmitter,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { MessageService } from 'primeng/api';
import { ParserResponse } from '../../../../../../shared/api/parsers-module/parsers-responses';
import { DefaultParserResponse } from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-activate-button',
  imports: [],
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
