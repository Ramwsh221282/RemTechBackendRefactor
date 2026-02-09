import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Scraper } from '../../types/Scraper';
import { Router } from '@angular/router';
import {ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';

@Component({
  selector: 'app-parser-settings-nav-button',
  imports: [],
  templateUrl: './parser-settings-nav-button.component.html',
  styleUrl: './parser-settings-nav-button.component.scss',
})
export class ParserSettingsNavButtonComponent {
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this._scraper.set(value);
  }

  private readonly _scraper: WritableSignal<ParserResponse>;
  private readonly _router: Router;

  constructor(router: Router) {
    this._scraper = signal({
      Id: '',
      Domain: '',
      ServiceType: '',
      FinishedAt: null,
      StartedAt: null,
      NextRun: null,
      WaitDays: null,
      State: '',
      ParsedCount: 0,
      ElapsedHours: 0,
      ElapsedSeconds: 0,
      ElapsedMinutes: 0,
      Links: [],
    });
    this._router = router;
  }

  public get scraper(): string {
    return this._scraper().Domain;
  }

  public get type(): string {
    return this._scraper().ServiceType;
  }

  public onClick(): void {
    const scraper: ParserResponse = this._scraper();
    const id: string = scraper.Id
    this._router.navigate(['/scrapers', 'settings', id]);
  }
}
