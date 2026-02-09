import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';
import { AdminAccessGuard } from '../../shared/guards/AdminAccessGuard';

export const ContainedItemsRoutes: Routes = [
  {
    path: 'contained-items',
    loadComponent: () =>
      import('./contained-items-management-page.component').then(
        (c) => c.ContainedItemsManagementPageComponent,
      ),
    canActivate: [RootExistsGuard, AdminAccessGuard],
  },
];
