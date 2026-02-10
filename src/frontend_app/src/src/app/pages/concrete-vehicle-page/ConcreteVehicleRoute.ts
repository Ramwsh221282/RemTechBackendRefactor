import { Routes } from '@angular/router';

export const ConcreteVehicleRoute: Routes = [
  {
    path: 'vehicle',
    loadComponent: () =>
      import('./concrete-vehicle-page.component').then(
        (c) => c.ConcreteVehiclePageComponent,
      ),
  },
];
