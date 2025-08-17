import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';

export const ConcreteVehicleRoute: Routes = [
  {
    path: 'vehicle',
    loadComponent: () =>
      import('./concrete-vehicle-page.component').then(
        (c) => c.ConcreteVehiclePageComponent,
      ),
    canActivate: [RootExistsGuard],
  },
];
