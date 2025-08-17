import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';

export const ForbiddenPageRoutes: Routes = [
  {
    path: 'forbidden',
    loadComponent: () =>
      import('./forbidden-page.component').then(
        (c) => c.ForbiddenPageComponent,
      ),
  },
];
