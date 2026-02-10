import { Routes } from '@angular/router';

export const ForbiddenPageRoutes: Routes = [
  {
    path: 'forbidden',
    loadComponent: () =>
      import('./forbidden-page.component').then(
        (c) => c.ForbiddenPageComponent,
      ),
  },
];
