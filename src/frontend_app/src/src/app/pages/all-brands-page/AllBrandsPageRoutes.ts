import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';

export const AllBrandsPageRoutes: Routes = [
  {
    path: 'brands/all',
    loadComponent: () =>
      import('./all-brands-page.component').then(
        (c) => c.AllBrandsPageComponent,
      ),
    canActivate: [RootExistsGuard],
  },
];
