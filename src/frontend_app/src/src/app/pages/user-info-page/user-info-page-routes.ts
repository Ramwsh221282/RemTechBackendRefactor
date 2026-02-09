import { Routes } from '@angular/router';
import { AuthorizedGuard } from '../../shared/guards/AuthorizedGuard';
import {ConfirmationService, MessageService} from 'primeng/api';

export const UserInfoPageRoutes: Routes = [
  {
    path: 'user',
    loadComponent: () =>
      import('./user-info-page.component').then((c) => c.UserInfoPageComponent),
    canMatch: [AuthorizedGuard],
    providers: [MessageService, ConfirmationService]
  },
];
