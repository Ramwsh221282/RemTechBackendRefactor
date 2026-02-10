import { Routes } from '@angular/router';

export const AllCategoriesRoute: Routes = [
  {
    path: 'categories/all',
    loadComponent: () =>
      import('./all-categories-page.component').then(
        (c) => c.AllCategoriesPageComponent,
      ),
  },
];
