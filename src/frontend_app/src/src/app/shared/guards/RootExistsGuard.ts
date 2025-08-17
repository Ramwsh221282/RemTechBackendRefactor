import { CanActivateFn, Router } from '@angular/router';
import { UsersService } from '../../pages/sign-in-page/services/UsersService';
import { inject } from '@angular/core';
import { map } from 'rxjs';

export const RootExistsGuard: CanActivateFn = (route, state) => {
  const usersService: UsersService = inject(UsersService);
  const router: Router = inject(Router);
  return usersService.checkRoot().pipe(
    map((resp: boolean) => {
      if (!resp) {
        router.navigate(['/create-root']);
        return false;
      }
      return true;
    }),
  );
};
