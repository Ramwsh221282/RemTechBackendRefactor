import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ParserSettingsNavButtonComponent } from './components/parser-settings-nav-button/parser-settings-nav-button.component';

import {ParserResponse} from '../../../../shared/api/parsers-module/parsers-responses';
import {ParsersControlApiService} from '../../../../shared/api/parsers-module/parsers-control-api.service';
import {TypedEnvelope} from '../../../../shared/api/envelope';

@Component({
  selector: 'app-scrapers-management-settings-page',
  imports: [RouterOutlet, ParserSettingsNavButtonComponent],
  templateUrl: './scrapers-management-settings-page.component.html',
  styleUrl: './scrapers-management-settings-page.component.scss',
})
export class ScrapersManagementSettingsPageComponent {
  private readonly _scrapers: WritableSignal<ParserResponse[]>;
  private readonly _destoryRef: DestroyRef = inject(DestroyRef);
  constructor(service: ParsersControlApiService) {
    this._scrapers = signal([]);
    effect(() => {
      service
        .fetchParsers()
        .pipe(takeUntilDestroyed(this._destoryRef))
        .subscribe({
          next: (envelope: TypedEnvelope<ParserResponse[]>): void => {
            if (envelope.body) {
              this._scrapers.set(envelope.body);
            }
          },
        });
    });
  }

  public get scrapers(): ParserResponse[] {
    return this._scrapers();
  }
}
