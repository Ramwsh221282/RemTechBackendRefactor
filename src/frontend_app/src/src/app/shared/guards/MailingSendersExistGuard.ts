import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { map } from 'rxjs';
import { MailingManagementService } from '../../pages/mailing-management-page/services/MailingManagementService';
import { MailingSender } from '../../pages/mailing-management-page/models/MailingSender';

export const MailingSendersExistGuard: CanActivateFn = (route, state) => {
  const service: MailingManagementService = inject(MailingManagementService);
  const router: Router = inject(Router);
  return service.read().pipe(
    map((resp: MailingSender[]) => {
      if (resp.length > 0) {
        return true;
      } else {
        router.navigate(['/oops']);
        return false;
      }
    }),
  );
};
