import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';

export const SparesPageRoutes: Routes = [
  {
    path: 'spares',
    loadComponent: () =>
      import('./spares-page.component').then((c) => c.SparesPageComponent),
    canActivate: [RootExistsGuard],
  },
];
