import { Component, Input, signal, WritableSignal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';

@Component({
  selector: 'app-scraper-next-run-info',
  imports: [DatePipe],
  templateUrl: './scraper-next-run-info.component.html',
  styleUrl: './scraper-next-run-info.component.scss',
})
export class ScraperNextRunInfoComponent {
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<Scraper>;

  constructor() {
    this._scraper = signal(VehicleScrapersService.defaultScraper());
  }

  public get nextRunDate(): Date {
    return this._scraper().nextRun;
  }
}
