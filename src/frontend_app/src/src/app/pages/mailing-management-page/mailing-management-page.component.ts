import { Component } from '@angular/core';
import { Panel } from 'primeng/panel';
import { Breadcrumb } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-mailing-management-page',
  imports: [Panel, Breadcrumb, RouterOutlet],
  templateUrl: './mailing-management-page.component.html',
  styleUrl: './mailing-management-page.component.scss',
})
export class MailingManagementPageComponent {
  public menuItems: MenuItem[] | undefined = [
    { label: 'Приветствие', routerLink: 'greeting' },
    { label: 'Руководство', routerLink: 'doc' },
    { label: 'Настройка', routerLink: 'settings' },
  ];
}
