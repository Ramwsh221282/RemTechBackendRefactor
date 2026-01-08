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

  // return next(req).pipe(
  //   catchError((error: HttpErrorResponse) => {
  //     if (error.status !== 401) {
  //       return throwError(() => error);
  //     }
  //
  //     if (isVerifyRequest || isRefreshRequest) {
  //       authStatusService.setIsNotAuthenticated();
  //       permissionsService.clean();
  //       return throwError(() => error);
  //     }
  //
  //     return identityService.refreshToken().pipe(
  //       tap({
  //         next: () => {
  //           authStatusService.setIsAuthenticated(true)
  //         },
  //         error: () => {
  //           authStatusService.setIsNotAuthenticated();
  //           permissionsService.clean();
  //         }
  //       }),
  //
  //       switchMap(() => next(req)),
  //       catchError(refreshError => {
  //         authStatusService.setIsNotAuthenticated();
  //         permissionsService.clean();
  //         return throwError(() => refreshError);
  //       })
  //     ).subscribe()
  //   }),
  //   tap({
  //     complete: () => {
  //       authStatusService.setIsNotAuthenticated();
  //     }
  //   })
  // );

  return next(req).pipe(
    tap({
      error: (error: HttpErrorResponse): void => {
        if ([401].includes(error.status) && req.url.includes('verify')) {
          authStatusService.setIsNotAuthenticated();
          permissionsService.clean();
        }
        if ([401].includes(error.status) && req.url.includes('refresh')) {
          authStatusService.setIsNotAuthenticated();
          permissionsService.clean();
        }
        if ([401].includes(error.status)) {
          identityService.refreshToken().pipe(
            tap({
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
