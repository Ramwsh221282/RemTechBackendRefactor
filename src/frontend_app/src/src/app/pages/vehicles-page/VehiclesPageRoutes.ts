import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';

export const VehiclesPageRoutes: Routes = [
  {
    path: 'vehicles',
    loadComponent: () =>
      import('./vehicles-page.component').then((c) => c.VehiclesPageComponent),
    canActivate: [RootExistsGuard],
  },
];
