import {
  Component,
  EventEmitter,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { Toast } from 'primeng/toast';
import { ConfirmDialog } from 'primeng/confirmdialog';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-deactivate-button',
  imports: [Toast, ConfirmDialog],
  templateUrl: './scraper-deactivate-button.component.html',
  styleUrl: './scraper-deactivate-button.component.scss',
  providers: [],
})
export class ScraperDeactivateButtonComponent {
  constructor() {
    this.scraperDeactivated = new EventEmitter();
    this._scraper = signal(DefaultParserResponse());
  }

  private readonly _scraper: WritableSignal<ParserResponse>;
  @Output() scraperDeactivated: EventEmitter<ParserResponse>;
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  public click($event: Event): void {
    $event.stopPropagation();
    this.scraperDeactivated.emit(this._scraper());
  }
}
