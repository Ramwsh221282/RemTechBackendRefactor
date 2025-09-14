import { Component, Input, signal, WritableSignal } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface ScraperDocNavigationButtonData {
  name: string;
  routerLink: string;
}

@Component({
  selector: 'app-scraper-management-doc-navigation-button',
  imports: [RouterLink],
  templateUrl: './scraper-management-doc-navigation-button.component.html',
  styleUrl: './scraper-management-doc-navigation-button.component.scss',
})
export class ScraperManagementDocNavigationButtonComponent {
  @Input({ required: true }) set data_setter(
    value: ScraperDocNavigationButtonData,
  ) {
    this._data.set(value);
  }

  private readonly _data: WritableSignal<ScraperDocNavigationButtonData>;

  constructor() {
    this._data = signal({ name: '', routerLink: '' });
  }

  public get data(): ScraperDocNavigationButtonData {
    return this._data();
  }
}
