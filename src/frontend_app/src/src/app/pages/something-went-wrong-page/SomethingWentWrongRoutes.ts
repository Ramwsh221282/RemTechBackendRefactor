import { Routes } from '@angular/router';

export const SomethingWentWrongRoutes: Routes = [
  {
    path: 'oops',
    loadComponent: () =>
      import('./something-went-wrong-page.component').then(
        (c) => c.SomethingWentWrongPageComponent,
      ),
  },
];
