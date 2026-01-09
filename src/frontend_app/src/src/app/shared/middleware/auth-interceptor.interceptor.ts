import {HttpErrorResponse, HttpInterceptorFn} from '@angular/common/http';
import {AuthenticationStatusService} from '../services/AuthenticationStatusService';
import {inject} from '@angular/core';
import {catchError, switchMap, tap, throwError} from 'rxjs';
import {IdentityApiService} from '../api/identity-module/identity-api-service';
import {PermissionsStatusService, UserAccountPermissions} from '../services/PermissionsStatus.service';
import {AccountResponse} from '../api/identity-module/identity-responses';
import {TypedEnvelope} from '../api/envelope';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authStatusService: AuthenticationStatusService = inject(AuthenticationStatusService);
  const identityService: IdentityApiService = inject(IdentityApiService);
  const permissionsService: PermissionsStatusService = inject(PermissionsStatusService);

  const isVerifyRequest: boolean = req.url.includes('verify');
  const isRefreshRequest: boolean = req.url.includes('refresh');

  return next(req).pipe(
    tap({
      error: (error: HttpErrorResponse): void => {
        if ([401].includes(error.status) && isVerifyRequest) {
          authStatusService.setIsNotAuthenticated();
          permissionsService.clean();
        }

        if ([401].includes(error.status) && isRefreshRequest) {
          authStatusService.setIsNotAuthenticated();
          permissionsService.clean();
        }

        if ([401].includes(error.status)) {
          identityService.refreshToken().pipe(
            tap({
              complete: (): void => {
                authStatusService.setIsAuthenticated(true)
                identityService.fetchAccount()
                  .pipe(tap({
                    next: (account: TypedEnvelope<AccountResponse>): void => {
                      if (account.body) {
                        permissionsService.initializePermissions(mapPermissions(account.body));
                      }
                    }
                  })).subscribe()
              }
            })
          ).subscribe()
        }

      },
      complete: (): void => {
        authStatusService.setIsAuthenticated(true)
      },
    })
  )
};

const mapPermissions = (response: AccountResponse): UserAccountPermissions[] => {
  return response.Permissions.map(p => {
    return { Id: p.Id, Name: p.Name, Description: p.Description };
  })
}
