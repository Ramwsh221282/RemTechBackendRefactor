import { Routes } from '@angular/router';

export const RegistrationConfirmationRoute: Routes = [
  {
    path: 'email-confirmation',
    loadComponent: () =>
      import('./registration-confirmation-page.component').then(
        (c) => c.RegistrationConfirmationPageComponent,
      ),
  },
];
