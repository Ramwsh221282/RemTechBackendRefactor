import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { VehicleScrapersService } from './services/vehicle-scrapers.service';
import { Scraper } from './types/Scraper';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ParserSettingsNavButtonComponent } from './components/parser-settings-nav-button/parser-settings-nav-button.component';
import { NgForOf } from '@angular/common';

@Component({
  selector: 'app-scrapers-management-settings-page',
  imports: [RouterOutlet, ParserSettingsNavButtonComponent, NgForOf],
  templateUrl: './scrapers-management-settings-page.component.html',
  styleUrl: './scrapers-management-settings-page.component.scss',
})
export class ScrapersManagementSettingsPageComponent {
  private readonly _scrapers: WritableSignal<Scraper[]>;
  private readonly _destoryRef: DestroyRef = inject(DestroyRef);
  constructor(service: VehicleScrapersService) {
    this._scrapers = signal([]);
    effect(() => {
      service
        .fetch()
        .pipe(takeUntilDestroyed(this._destoryRef))
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
