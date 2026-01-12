import { Routes } from '@angular/router';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';
import { MessageService } from 'primeng/api';

export const SignInPageRoutes: Routes = [
  {
    path: 'sign-in',
    loadComponent: () =>
      import('./sign-in-page.component').then((c) => c.SignInPageComponent),
    canActivate: [RootExistsGuard],
    providers: [MessageService],
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import(
        './components/reset-password-page/reset-password-page.component'
      ).then((c) => c.ResetPasswordPageComponent),
    canActivate: [RootExistsGuard],
    providers: [MessageService],
  },
];
