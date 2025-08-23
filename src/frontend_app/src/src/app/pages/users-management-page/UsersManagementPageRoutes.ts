import { Routes } from '@angular/router';
import { AdminAccessGuard } from '../../shared/guards/AdminAccessGuard';

export const UsersManagementPageRoutes: Routes = [
  {
    path: 'users-management',
    loadComponent: () =>
      import('./users-management-page.component').then(
        (c) => c.UsersManagementPageComponent,
      ),
    canActivate: [AdminAccessGuard],
  },
];
