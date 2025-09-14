import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-scraper-state-select',
  imports: [FormsModule, NgClass],
  templateUrl: './scraper-state-select.component.html',
  styleUrl: './scraper-state-select.component.scss',
})
export class ScraperStateSelectComponent {
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<Scraper>;

  constructor() {
    this._scraper = signal(VehicleScrapersService.defaultScraper());
  }

  public get currentState(): string {
    return this._scraper().state;
  }
}
