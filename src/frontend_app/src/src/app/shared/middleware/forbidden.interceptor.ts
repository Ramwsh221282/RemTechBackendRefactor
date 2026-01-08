import {HttpErrorResponse, HttpInterceptorFn} from '@angular/common/http';
import {inject} from '@angular/core';
import {Router} from '@angular/router';
import {tap} from 'rxjs';

export const ForbiddenInterceptor: HttpInterceptorFn = (req, next) => {
  const router: Router = inject(Router);

  return next(req)
    .pipe(tap({
      error: (error: HttpErrorResponse): void => {
        if ([403].includes(error.status)) {
          router.navigate(['forbidden'])
        }
      }
    }))
}
