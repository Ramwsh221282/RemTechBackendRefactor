import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';

export const MainPageRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./main-page.component').then((c) => c.MainPageComponent),
    canActivate: [RootExistsGuard],
  },
];
