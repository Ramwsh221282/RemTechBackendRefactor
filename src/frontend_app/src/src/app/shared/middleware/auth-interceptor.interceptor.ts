import {HttpErrorResponse, HttpInterceptorFn} from '@angular/common/http';
import {AuthenticationStatusService} from '../services/AuthenticationStatusService';
import {inject} from '@angular/core';
import {tap} from 'rxjs';
import {IdentityApiService} from '../api/identity-module/identity-api-service';
import {PermissionsStatusService, UserAccountPermissions} from '../services/PermissionsStatus.service';
import {AccountResponse} from '../api/identity-module/identity-responses';
import {TypedEnvelope} from '../api/envelope';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authStatusService: AuthenticationStatusService = inject(AuthenticationStatusService);
  const identityService: IdentityApiService = inject(IdentityApiService);
  const permissionsService: PermissionsStatusService = inject(PermissionsStatusService);

  return next(req).pipe(
    tap({
      error: (error: HttpErrorResponse): void => {
        if ([401].includes(error.status)) {
          identityService.refreshToken().pipe(
            tap({
              error: (_: HttpErrorResponse): void => {
                authStatusService.setIsNotAuthenticated();
                permissionsService.clean();
              },
              complete: (): void => {
                authStatusService.setIsAuthenticated(true)
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
