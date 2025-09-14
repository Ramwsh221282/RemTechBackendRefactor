import { Component, Input, signal, WritableSignal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { Toast } from 'primeng/toast';

@Component({
  selector: 'app-scraper-last-run-info',
  imports: [DatePipe],
  templateUrl: './scraper-last-run-info.component.html',
  styleUrl: './scraper-last-run-info.component.scss',
})
export class ScraperLastRunInfoComponent {
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<Scraper>;

  constructor() {
    this._scraper = signal(VehicleScrapersService.defaultScraper());
  }

  public get lastRunDate(): Date {
    return this._scraper().lastRun;
  }

  public click(): void {}
}
