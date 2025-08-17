import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';

@Component({
  selector: 'app-scraper-elapsed-time',
  imports: [],
  templateUrl: './scraper-elapsed-time.component.html',
  styleUrl: './scraper-elapsed-time.component.scss',
})
export class ScraperElapsedTimeComponent {
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<Scraper>;

  constructor() {
    this._scraper = signal(VehicleScrapersService.defaultScraper());
  }

  public get hours(): number {
    return this._scraper().hours;
  }

  public get seconds(): number {
    return this._scraper().seconds;
  }

  public get minutes(): number {
    return this._scraper().minutes;
  }
}
