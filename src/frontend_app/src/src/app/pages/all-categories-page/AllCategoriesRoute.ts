import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';

export const AllCategoriesRoute: Routes = [
  {
    path: 'categories/all',
    loadComponent: () =>
      import('./all-categories-page.component').then(
        (c) => c.AllCategoriesPageComponent,
      ),
    canActivate: [RootExistsGuard],
  },
];
