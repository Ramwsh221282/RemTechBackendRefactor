import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select, SelectChangeEvent } from 'primeng/select';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-wait-days-select',
  imports: [FormsModule, Select, Toast],
  templateUrl: './scraper-wait-days-select.component.html',
  styleUrl: './scraper-wait-days-select.component.scss',
  providers: [MessageService],
})
export class ScraperWaitDaysSelectComponent implements OnInit {
  constructor() {
    this._scraper = signal(DefaultParserResponse());
    this._scraperWaitDays = signal([]);
    this.waitDaysSelected = new EventEmitter<number>();
  }

  @Output() waitDaysSelected: EventEmitter<number>;
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<ParserResponse>;
  private readonly _scraperWaitDays: WritableSignal<number[]>;

  public onSelect($event: SelectChangeEvent): void {
    const waitDays: number = $event.value as number;
    this.waitDaysSelected.emit(waitDays);
  }

  public ngOnInit(): void {
    this._scraperWaitDays.set([1, 2, 3, 4, 5, 6, 7]);
  }

  public get waitDaysOptions(): number[] {
    return this._scraperWaitDays();
  }

  public get currentWaitDays(): number | null | undefined {
    return this._scraper().WaitDays;
  }
}
