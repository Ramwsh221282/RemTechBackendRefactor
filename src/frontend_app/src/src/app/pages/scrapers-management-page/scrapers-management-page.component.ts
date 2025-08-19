import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Panel } from 'primeng/panel';
import { Breadcrumb } from 'primeng/breadcrumb';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-scrapers-management-page',
  imports: [Panel, Breadcrumb, RouterOutlet],
  templateUrl: './scrapers-management-page.component.html',
  styleUrl: './scrapers-management-page.component.scss',
})
export class ScrapersManagementPageComponent {
  public menuItems: MenuItem[] | undefined = [
    { label: 'Описание', routerLink: 'greeting' },
    { label: 'Руководство', routerLink: 'doc' },
    { label: 'Настройка', routerLink: 'settings' },
    { label: 'Чистильщик', routerLink: 'cleaner' },
    { label: 'Журналы', routerLink: 'journals' },
  ];
}
