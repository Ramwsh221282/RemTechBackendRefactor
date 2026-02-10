import { Routes } from '@angular/router';
import {ConfirmationService, MessageService} from 'primeng/api';

export const UserInfoPageRoutes: Routes = [
  {
    path: 'user',
    loadComponent: () =>
      import('./user-info-page.component').then((c) => c.UserInfoPageComponent),
    providers: [MessageService, ConfirmationService]
  },
];
