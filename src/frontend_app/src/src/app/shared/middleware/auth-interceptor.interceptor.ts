import {HttpErrorResponse, HttpInterceptorFn, HttpRequest} from '@angular/common/http';
import {AuthenticationStatusService} from '../services/AuthenticationStatusService';
import {inject} from '@angular/core';
import {catchError, finalize, map, Observable, of, Subject, switchMap, take, tap, throwError} from 'rxjs';
import {IdentityApiService} from '../api/identity-module/identity-api-service';
import {
  PermissionsStatusService,
  UserAccountPermissions,
  UserAccountPermissionsFromAccountResponse
} from '../services/PermissionsStatus.service';
import {AccountResponse} from '../api/identity-module/identity-responses';
import {TypedEnvelope} from '../api/envelope';

let refreshInProgress: boolean = false;
let refreshSubject: Subject<boolean> | null = null;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authStatus = inject(AuthenticationStatusService);
  const identityService = inject(IdentityApiService);
  const cloned = req.clone({ withCredentials: true });

  const startRefresh = (): Observable<boolean> => {
    if (!refreshInProgress) {
      refreshInProgress = true;
      refreshSubject = new Subject<boolean>();

      identityService.refreshToken().pipe(
        tap(() => {
          authStatus.setIsAuthenticated(true);
        }),
        catchError((err: HttpErrorResponse) => {
          // refresh не удался
          authStatus.setIsNotAuthenticated();
          return throwError(() => err);
        }),
        finalize(() => {
          refreshInProgress = false;
        })
      ).subscribe({
        next: () => {
          // refresh успешен
          refreshSubject?.next(true);
          refreshSubject?.complete();
        },
        error: () => {
          // refresh провалился
          refreshSubject?.next(false);
          refreshSubject?.complete();
        }
      });
    }

    if (!refreshSubject) {
      // на всякий случай, но по идее не должно сюда попадать
      return throwError(() => new Error('No refreshSubject'));
    }

    // ждём один результат текущего refresh
    return refreshSubject.asObservable().pipe(take(1));
  };

  const handle401 = (error: HttpErrorResponse): Observable<any> => {
    // защитимся от рефреша самого себя (если refreshToken тоже вернёт 401)
    const isRefreshRequest = cloned.url.includes('/identity/refresh-token'); // подставь свой URL

    if (error.status !== 401 || isRefreshRequest) {
      return throwError(() => error);
    }

    // 1) запускаем / дожидаемся refresh
    // 2) если успех — повторяем исходный запрос
    // 3) если провал — возвращаем 401 наружу
    return startRefresh().pipe(
      switchMap((success) => {
        if (!success) {
          return throwError(() => error);
        }
        // refresh успешен — повторяем запрос
        return next(cloned);
      })
    );
  };

  return next(cloned).pipe(
    catchError((error: HttpErrorResponse) => handle401(error))
  );

  // return next(req)
  //   .pipe(catchError((error: HttpErrorResponse) => {
  //
  //     if ([401].includes(error.status) && !refreshInProgress) {
  //       refreshInProgress = true;
  //       identityService.refreshToken().pipe(
  //         finalize(() => {
  //           refreshInProgress = false;
  //         }),
  //         catchError((err: HttpErrorResponse) => {
  //           return throwError(() => err);
  //         }),
  //         tap(() => {
  //           authStatus.setIsAuthenticated(true)
  //         })
  //       ).subscribe()
  //       switchMap(() => next(cloned))
  //     }
  //
  //     if ([401].includes(error.status) && refreshInProgress) {
  //       // че тут делать
  //     }
  //
  //     return throwError(() => error);
  //   }))

}
