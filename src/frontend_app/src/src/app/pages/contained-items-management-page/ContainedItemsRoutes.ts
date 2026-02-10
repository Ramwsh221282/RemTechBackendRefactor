import { Routes } from '@angular/router';

export const ContainedItemsRoutes: Routes = [
  {
    path: 'contained-items',
    loadComponent: () =>
      import('./contained-items-management-page.component').then(
        (c) => c.ContainedItemsManagementPageComponent,
      ),
  },
];
