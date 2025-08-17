import { Component } from '@angular/core';
import {
  Accordion,
  AccordionContent,
  AccordionHeader,
  AccordionPanel,
} from 'primeng/accordion';
import {
  ScraperDocNavigationButtonData,
  ScraperManagementDocNavigationButtonComponent,
} from './scraper-management-doc-navigation-button/scraper-management-doc-navigation-button.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-scrapers-management-doc-page',
  imports: [ScraperManagementDocNavigationButtonComponent, RouterOutlet],
  templateUrl: './scrapers-management-doc-page.component.html',
  styleUrl: './scrapers-management-doc-page.component.scss',
})
export class ScrapersManagementDocPageComponent {
  public readonly buttonsData: ScraperDocNavigationButtonData[] = [
    {
      name: 'Начало работы',
      routerLink: 'beginning',
    },
    {
      name: 'Изменение состояния парсера',
      routerLink: 'parser-state-edit-doc',
    },
    {
      name: 'Изменение расписания парсера',
      routerLink: 'parser-time-edit-doc',
    },
    {
      name: 'Немедленное включение парсера',
      routerLink: 'parser-instant-enable-doc',
    },
    {
      name: 'Немедленное выключение парсера',
      routerLink: 'parser-instant-disable-doc',
    },
    {
      name: 'Добавление ссылки парсеру',
      routerLink: 'parser-link-appending-doc',
    },
    {
      name: 'Редактирование ссылки у парсера',
      routerLink: 'parser-link-editing-doc',
    },
  ];
}
