import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';
import { MessageService } from 'primeng/api';

export const MainPageRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./main-page.component').then((c) => c.MainPageComponent),
    canActivate: [RootExistsGuard],
    providers: [MessageService],
  },
];
