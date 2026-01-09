import {HttpErrorResponse, HttpInterceptorFn, HttpRequest} from '@angular/common/http';
import {AuthenticationStatusService} from '../services/AuthenticationStatusService';
import {inject} from '@angular/core';
import {catchError, switchMap, tap, throwError} from 'rxjs';
import {IdentityApiService} from '../api/identity-module/identity-api-service';
import {
  PermissionsStatusService,
  UserAccountPermissions,
  UserAccountPermissionsFromAccountResponse
} from '../services/PermissionsStatus.service';
import {AccountResponse} from '../api/identity-module/identity-responses';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authStatusService: AuthenticationStatusService = inject(AuthenticationStatusService);
  const identityService: IdentityApiService = inject(IdentityApiService);
  const permissionsService: PermissionsStatusService = inject(PermissionsStatusService);

  const isVerifyRequest: boolean = req.url.includes('verify');
  const isRefreshRequest: boolean = req.url.includes('refresh');
  const clonedRequest: HttpRequest<unknown> = req.clone({  withCredentials: true })

  return next(clonedRequest).pipe(
    tap({
      complete: () => {
        authStatusService.setIsAuthenticated(true);
      }
    }),
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && (isVerifyRequest || isRefreshRequest)) {
        authStatusService.setIsNotAuthenticated();
        permissionsService.clean();
        return throwError(() => error);
      }

      if (error.status === 401) {
        return identityService.refreshToken().pipe(
          switchMap(() => {
            authStatusService.setIsAuthenticated(true);

            return identityService.fetchAccount().pipe(
              tap(account => {
                if (account.body) {
                  permissionsService.initializePermissions(UserAccountPermissionsFromAccountResponse(account.body));
                }
              }),
              switchMap(() => next(clonedRequest))
            );
          }),
          catchError(refreshError => {
            authStatusService.setIsNotAuthenticated();
            permissionsService.clean();
            return throwError(() => refreshError);
          })
        );
      }
      return throwError(() => error);
    })
  );

  // return next(req).pipe(
  //   tap({
  //     error: (error: HttpErrorResponse): void => {
  //       if ([401].includes(error.status) && isVerifyRequest) {
  //         authStatusService.setIsNotAuthenticated();
  //         permissionsService.clean();
  //       }
  //
  //       if ([401].includes(error.status) && isRefreshRequest) {
  //         authStatusService.setIsNotAuthenticated();
  //         permissionsService.clean();
  //       }
  //
  //       if ([401].includes(error.status)) {
  //         identityService.refreshToken().pipe(
  //           switchMap(() => {
  //             authStatusService.setIsAuthenticated(true);
  //             return identityService.fetchAccount().pipe(
  //               tap(account => {
  //                 if (account.body) {
  //                   permissionsService.initializePermissions(mapPermissions(account.body));
  //                 }
  //               }),
  //               switchMap(() => next(clonedRequest))
  //             );
  //           }),
  //           catchError(() => {
  //             authStatusService.setIsNotAuthenticated();
  //             permissionsService.clean();
  //             return throwError(() => error);
  //           })
  //         ).subscribe()
  //       }
  //
  //     },
  //     complete: (): void => {
  //       authStatusService.setIsAuthenticated(true)
  //     },
  //   })
  // )
};
