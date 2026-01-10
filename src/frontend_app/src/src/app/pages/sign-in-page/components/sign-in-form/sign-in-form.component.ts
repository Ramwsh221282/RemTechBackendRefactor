import { Component, DestroyRef, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import {IdentityApiService} from '../../../../shared/api/identity-module/identity-api-service';
import {
  PermissionsStatusService,
  UserAccountPermissions,
  UserAccountPermissionsFromAccountResponse
} from '../../../../shared/services/PermissionsStatus.service';
import {TypedEnvelope} from '../../../../shared/api/envelope';
import {AccountPermissionsResponse, AccountResponse} from '../../../../shared/api/identity-module/identity-responses';
import {catchError, EMPTY, switchMap, tap} from 'rxjs';
import {AuthenticationStatusService} from '../../../../shared/services/AuthenticationStatusService';

@Component({
  selector: 'app-sign-in-form',
  imports: [ReactiveFormsModule, Toast],
  templateUrl: './sign-in-form.component.html',
  styleUrl: './sign-in-form.component.scss',
  providers: [MessageService],
})
export class SignInFormComponent {
  signIngForm: FormGroup = new FormGroup({
    email: new FormControl(''),
    name: new FormControl(''),
    password: new FormControl(''),
  });

  private readonly _destroyRef: DestroyRef = inject(DestroyRef);

  constructor(
    private readonly _messageService: MessageService,
    private readonly _identityService: IdentityApiService,
    private readonly _authStatusService: AuthenticationStatusService,
    private readonly _router: Router,
    private readonly _permissionsService: PermissionsStatusService,
  ) {
  }

  public submit(): void {
    const formValues = this.signIngForm.value;
    const email: string = formValues.email;
    const name: string = formValues.name;
    const password: string = formValues.password;
    if (StringUtils.isEmptyOrWhiteSpace(password)) {
      MessageServiceUtils.showError(
        this._messageService,
        'Необходимо ввести пароль.',
      );
      return;
    }
    if (StringUtils.allEmpty([email, name])) {
      MessageServiceUtils.showError(
        this._messageService,
        'Необходимо ввести почту или псевдоним',
      );
      return;
    }
    this.authenticate(password, email, name);
  }

  public resetPasswordClick(): void {
    const path = 'reset-password';
    this._router.navigate([path]);
  }

  private authenticate(
    password: string,
    email?: string | null | undefined,
    name?: string | null | undefined,
  ): void {
    this._identityService.authenticate(password, email, name)
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        tap(() => {
          MessageServiceUtils.showSuccess(this._messageService, 'Авторизация успешна');
          this._authStatusService.setIsAuthenticated(true);
          this._authStatusService.unlockRefresh();
          this._authStatusService.setTokenRefreshed();
        }),
        switchMap(() => this._identityService.fetchAccount()),
        tap((envelope: TypedEnvelope<AccountResponse>) => {
          if (envelope.body) {
            this._permissionsService.initializePermissions(UserAccountPermissionsFromAccountResponse(envelope.body));
          }
        }),
        catchError((err: HttpErrorResponse) => {
          const message: string = err.error.message;
          MessageServiceUtils.showError(this._messageService, message)
          return EMPTY;
        }))
      .subscribe()
  }
}
