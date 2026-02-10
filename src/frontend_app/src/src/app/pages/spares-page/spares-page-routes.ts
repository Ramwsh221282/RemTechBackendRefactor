import { Routes } from '@angular/router';

export const SparesPageRoutes: Routes = [
  {
    path: 'spares',
    loadComponent: () =>
      import('./spares-page.component').then((c) => c.SparesPageComponent),
  },
];
