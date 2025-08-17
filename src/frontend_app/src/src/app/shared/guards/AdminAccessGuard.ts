import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { UsersService } from '../../pages/sign-in-page/services/UsersService';
import { CookieService } from 'ngx-cookie-service';
import { HttpErrorResponse } from '@angular/common/http';
import { catchError, map, of } from 'rxjs';
import { TokensService } from '../services/TokensService';

export const AdminAccessGuard: CanActivateFn = (route, state) => {
  const usersService: UsersService = inject(UsersService);
  const cookiesService: CookieService = inject(CookieService);
  const tokensService: TokensService = inject(TokensService);
  const router: Router = inject(Router);
  const token: string | undefined = cookiesService.get('RemTechAccessToken');
  const tokenId: string | undefined = cookiesService.get(
    'RemTechAccessTokenId',
  );
  if (!token || !tokenId) {
    tokensService.setNotAdmin();
    router.navigate(['/sign-in']);
    return false;
  }

  return usersService.verifyAdminAccess(tokenId).pipe(
    map(() => true),
    catchError((err: HttpErrorResponse) => {
      if (err.status === 403) {
        tokensService.setNotAdmin();
        router.navigate(['/forbidden']);
      } else {
        tokensService.setNotAdmin();
        router.navigate(['/sign-in']);
      }
      return of(false);
    }),
  );
};
