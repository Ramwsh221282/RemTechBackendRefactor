import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokensService } from '../services/TokensService';
import { StringUtils } from '../utils/string-utils';
import { map, tap } from 'rxjs';

export const AuthorizedGuard: CanActivateFn = (route, state) => {
  const service: TokensService = inject(TokensService);
  const router: Router = inject(Router);
  return service.tokenAsObservable().pipe(
    tap((token: string): boolean => {
      if (StringUtils.isEmptyOrWhiteSpace(token)) {
        router.navigate(['/sign-in']);
        return false;
      }
      return true;
    }),
    map(() => true),
  );
};
