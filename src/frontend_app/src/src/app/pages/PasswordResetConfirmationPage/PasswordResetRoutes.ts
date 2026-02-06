import { Routes } from '@angular/router';

export const PasswordResetRoutes: Routes = [
  {
    path: 'password-reset-confirm',
    loadComponent: () =>
      import('./PasswordResetConfirmationPage.component').then(
        (c) => c.PasswordResetConfirmationPageComponent,
      ),
  },
];
