import {HttpErrorResponse, HttpInterceptorFn, HttpRequest} from '@angular/common/http';
import {AuthenticationStatusService} from '../services/AuthenticationStatusService';
import {inject} from '@angular/core';
import {catchError, finalize, map, Observable, of, Subject, switchMap, take, tap, throwError} from 'rxjs';
import {IdentityApiService} from '../api/identity-module/identity-api-service';
import {
  PermissionsStatusService,
  UserAccountPermissionsFromAccountResponse
} from '../services/PermissionsStatus.service';
import {TypedEnvelope} from '../api/envelope';
import {AccountResponse} from '../api/identity-module/identity-responses';

let refreshInProgress: boolean = false;
let refreshSubject: Subject<boolean> | null = null;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authStatus = inject(AuthenticationStatusService);
  const identityService = inject(IdentityApiService);
  const permissionsStatus = inject(PermissionsStatusService);
  const cloned = req.clone({ withCredentials: true });

  const startRefresh = (): Observable<boolean> => {
    if (!refreshInProgress) {
      refreshInProgress = true;
      refreshSubject = new Subject<boolean>();

      identityService.refreshToken().pipe(
        tap(() => {
          authStatus.setIsAuthenticated(true);
          identityService.fetchAccount().subscribe((resp: TypedEnvelope<AccountResponse>): void => {
            if (resp.body) {
              permissionsStatus.initializePermissions(UserAccountPermissionsFromAccountResponse(resp.body));
            }
          })
        }),
        catchError((err: HttpErrorResponse) => {
          authStatus.setIsNotAuthenticated();
          permissionsStatus.clean();
          return throwError(() => err);
        }),
        finalize(() => {
          refreshInProgress = false;
        })
      ).subscribe({
        next: () => {
          refreshSubject?.next(true);
          refreshSubject?.complete();
        },
        error: () => {
          refreshSubject?.next(false);
          refreshSubject?.complete();
        }
      });
    }

    if (!refreshSubject) {
      return throwError(() => new Error('No refreshSubject'));
    }

    return refreshSubject.asObservable().pipe(take(1));
  };

  const handle401 = (error: HttpErrorResponse): Observable<any> => {
    const isRefreshRequest = cloned.url.includes('/identity/refresh-token');

    if (error.status !== 401 || isRefreshRequest) {
      return throwError(() => error);
    }

    return startRefresh().pipe(
      switchMap((success) => {
        if (!success) {
          return throwError(() => error);
        }
        return next(cloned);
      })
    );
  };

  return next(cloned).pipe(
    catchError((error: HttpErrorResponse) => handle401(error))
  );
}
