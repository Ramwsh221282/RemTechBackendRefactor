import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';

@Component({
  selector: 'app-scraper-processed-amount',
  imports: [],
  templateUrl: './scraper-processed-amount.component.html',
  styleUrl: './scraper-processed-amount.component.scss',
})
export class ScraperProcessedAmountComponent {
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<Scraper>;

  constructor() {
    this._scraper = signal(VehicleScrapersService.defaultScraper());
  }

  public get processedAmount(): number {
    return this._scraper().processed;
  }
}
