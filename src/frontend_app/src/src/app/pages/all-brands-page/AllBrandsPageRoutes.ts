import { Routes } from '@angular/router';

export const AllBrandsPageRoutes: Routes = [
  {
    path: 'brands/all',
    loadComponent: () =>
      import('./all-brands-page.component').then(
        (c) => c.AllBrandsPageComponent,
      ),
  },
];
