import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';

@Component({
  selector: 'app-scraper-domain-info',
  imports: [],
  templateUrl: './scraper-domain-info.component.html',
  styleUrl: './scraper-domain-info.component.scss',
})
export class ScraperDomainInfoComponent {
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }
  private readonly _scraper: WritableSignal<Scraper>;
  constructor() {
    this._scraper = signal(VehicleScrapersService.defaultScraper());
  }

  public get domain(): string {
    return this._scraper().domain;
  }
}
