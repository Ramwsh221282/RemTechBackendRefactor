import {HttpErrorResponse, HttpInterceptorFn} from '@angular/common/http';
import {AuthenticationStatusService} from '../services/AuthenticationStatusService';
import {inject} from '@angular/core';
import {tap} from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authStatusService: AuthenticationStatusService = inject(AuthenticationStatusService);

  return next(req).pipe(
    tap({
      error: (_: HttpErrorResponse): void => {
        authStatusService.setIsNotAuthenticated();
      },
      complete: (): void => {
        authStatusService.setIsAuthenticated(true)
      },
    })
  )
};
