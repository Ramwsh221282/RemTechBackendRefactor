import {DestroyRef, inject, Injectable, OnInit, signal, WritableSignal} from '@angular/core';
import {AuthenticationStatusService} from './AuthenticationStatusService';
import {IdentityApiService} from '../api/identity-module/identity-api-service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {tap} from 'rxjs';
import {HttpErrorResponse} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class OnApplicationStartupAuthVerificationService implements OnInit {
  private readonly _authenticationChecked: WritableSignal<boolean> = signal(false);
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);

  constructor(
    private readonly _authStatusService: AuthenticationStatusService,
    private readonly _identityService: IdentityApiService) {
  }

  ngOnInit(): void {
    if (this._authStatusService.isAuthorized) return;
    this._identityService.verifyToken()
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        tap({
          error: (error: HttpErrorResponse): void => {
            if ([401].includes(error.status)) {
              this.tryRefreshToken();
            } else {
              this.userNotAuthorized();
            }
          },
          complete: (): void => {
            this.userAuthorized();
          },
          finalize: (): void => {
            this.markAsChecked();
          }
        }))
      .subscribe();
  }

  private tryRefreshToken(): void {
    if (this._authStatusService.isAuthorized) return;
    this._identityService.refreshToken()
      .pipe(takeUntilDestroyed(this._destroyRef),
        tap({
          error: (_: HttpErrorResponse): void => {
            this.userNotAuthorized();
          },
          complete: (): void => {
            this.userAuthorized();
          },
          finalize: (): void => {
            this.markAsChecked();
          }
        })).subscribe();
  }

  private markAsChecked(): void {
    if (this._authenticationChecked()) {
      return;
    }

    this._authenticationChecked.set(true)
  }

  private userAuthorized(): void {
    this._authStatusService.setIsAuthenticated(true);
  }

  private userNotAuthorized(): void {
    this._authStatusService.setIsNotAuthenticated();
  }
}
