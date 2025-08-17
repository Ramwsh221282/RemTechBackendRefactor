import { Routes } from '@angular/router';
import { AdminAccessGuard } from '../../../shared/guards/AdminAccessGuard';
import { RootExistsGuard } from '../../../shared/guards/RootExistsGuard';

export const MailingManagementPageRoutes: Routes = [
  {
    path: 'mailing-management',
    loadComponent: () =>
      import('../mailing-management-page.component').then(
        (c) => c.MailingManagementPageComponent,
      ),
    canActivate: [RootExistsGuard, AdminAccessGuard],
    children: [
      {
        path: 'greeting',
        loadComponent: () =>
          import(
            '../components/mailing-management-greeting-child-page/mailing-management-greeting-child-page.component'
          ).then((c) => c.MailingManagementGreetingChildPageComponent),
      },
      {
        path: 'doc',
        loadComponent: () =>
          import(
            '../components/mailing-management-doc-child-page/mailing-management-doc-child-page.component'
          ).then((c) => c.MailingManagementDocChildPageComponent),
      },
      {
        path: 'settings',
        loadComponent: () =>
          import(
            '../components/mailing-management-settings-child-page/mailing-management-settings-child-page.component'
          ).then((c) => c.MailingManagementSettingsChildPageComponent),
      },
    ],
  },
];
