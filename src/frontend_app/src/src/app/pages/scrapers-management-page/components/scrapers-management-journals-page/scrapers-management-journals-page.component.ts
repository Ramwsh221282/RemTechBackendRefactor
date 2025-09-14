import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { NgForOf } from '@angular/common';
import { ParserSettingsNavButtonComponent } from '../scrapers-management-settings-page/components/parser-settings-nav-button/parser-settings-nav-button.component';
import { RouterOutlet } from '@angular/router';
import { Scraper } from '../scrapers-management-settings-page/types/Scraper';
import { VehicleScrapersService } from '../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ScraperJournalNavButtonComponent } from './components/scraper-journal-nav-button/scraper-journal-nav-button.component';

@Component({
  selector: 'app-scrapers-management-journals-page',
  imports: [
    NgForOf,    
    RouterOutlet,
    ScraperJournalNavButtonComponent,
  ],
  templateUrl: './scrapers-management-journals-page.component.html',
  styleUrl: './scrapers-management-journals-page.component.scss',
})
export class ScrapersManagementJournalsPageComponent {
  private readonly _scrapers: WritableSignal<Scraper[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(private readonly _service: VehicleScrapersService) {
    this._scrapers = signal([]);
    effect(() => {
      this._service
        .fetch()
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: Scraper[]): void => {
            this._scrapers.set(data);
          },
        });
    });
  }

  public get scrapers(): Scraper[] {
    return this._scrapers();
  }
}
