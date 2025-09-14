import { Routes } from '@angular/router';

export const CreateRootUserRoutes: Routes = [
  {
    path: 'create-root',
    loadComponent: () =>
      import('./create-root-user-page.component').then(
        (c) => c.CreateRootUserPageComponent,
      ),
  },
];
