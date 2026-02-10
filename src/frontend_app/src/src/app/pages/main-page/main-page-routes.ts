import { Routes } from '@angular/router';
import { MessageService } from 'primeng/api';

export const MainPageRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./main-page.component').then((c) => c.MainPageComponent),
    providers: [MessageService],
  },
];
