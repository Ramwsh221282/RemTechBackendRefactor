import {
  Component,
  DestroyRef,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { Select, SelectChangeEvent } from 'primeng/select';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ParserWaitDaysUpdateResult } from '../../../scrapers-management-settings-page/types/ParserWaitDaysUpdateResult';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';

@Component({
  selector: 'app-scraper-wait-days-select',
  imports: [FormsModule, Select, Toast],
  templateUrl: './scraper-wait-days-select.component.html',
  styleUrl: './scraper-wait-days-select.component.scss',
  providers: [MessageService],
})
export class ScraperWaitDaysSelectComponent implements OnInit {
  @Output() parserChanged: EventEmitter<Scraper> = new EventEmitter<Scraper>();
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<Scraper>;
  private readonly _scraperWaitDays: WritableSignal<number[]>;
  private readonly _messageService: MessageService;
  private readonly _service: VehicleScrapersService;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);

  constructor(messageService: MessageService, service: VehicleScrapersService) {
    this._messageService = messageService;
    this._service = service;
    this._scraper = signal(VehicleScrapersService.defaultScraper());
    this._scraperWaitDays = signal([]);
  }

  public onSelect($event: SelectChangeEvent): void {
    const waitDays: number = $event.value as number;
    const current: Scraper = this._scraper();
    this._service
      .changeWaitDays(current, { newWaitDays: waitDays })
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (data: ParserWaitDaysUpdateResult): void => {
          current.waitDays = data.newWaitDays;
          current.nextRun = data.nextRun;
          this.parserChanged.emit(current);
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Изменено время ожидания парсера ${current.name} ${current.type} на ${data.newWaitDays}`,
          );
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }

  public ngOnInit(): void {
    this._scraperWaitDays.set([1, 2, 3, 4, 5, 6, 7]);
  }

  public get waitDaysOptions(): number[] {
    return this._scraperWaitDays();
  }

  public get currentWaitDays(): number {
    return this._scraper().waitDays;
  }
}
