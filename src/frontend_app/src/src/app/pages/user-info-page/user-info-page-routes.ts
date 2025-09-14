import { Routes } from '@angular/router';
import { AuthorizedGuard } from '../../shared/guards/AuthorizedGuard';

export const UserInfoPageRoutes: Routes = [
  {
    path: 'user',
    loadComponent: () =>
      import('./user-info-page.component').then((c) => c.UserInfoPageComponent),
    canMatch: [AuthorizedGuard],
  },
];
