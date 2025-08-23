import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokensService } from '../services/TokensService';

export const AlreadyAuthorizedGuard: CanActivateFn = () => {
  const router: Router = inject(Router);
  const tokensService: TokensService = inject(TokensService);
  if (tokensService.hasToken()) {
    router.navigate(['']);
    return false;
  }
  return true;
};
