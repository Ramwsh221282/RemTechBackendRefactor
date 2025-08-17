import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';
import { MailingSendersExistGuard } from '../../shared/guards/MailingSendersExistGuard';

export const SignInPageRoutes: Routes = [
  {
    path: 'sign-in',
    loadComponent: () =>
      import('./sign-in-page.component').then((c) => c.SignInPageComponent),
    canActivate: [RootExistsGuard],
  },
];
