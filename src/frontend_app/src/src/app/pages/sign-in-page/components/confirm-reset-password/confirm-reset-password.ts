import {
  ChangeDetectionStrategy,
  Component,
  effect,
  signal,
  WritableSignal,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { IdentityApiService } from '../../../../shared/api/identity-module/identity-api-service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { PasswordRulesComponent } from '../../../../shared/components/password-rules/password-rules.component';
import { Toast } from 'primeng/toast';
import { catchError, EMPTY, Observable, tap } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-confirm-reset-password',
  imports: [ReactiveFormsModule, PasswordRulesComponent, Toast],
  templateUrl: './confirm-reset-password.html',
  styleUrl: './confirm-reset-password.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmResetPassword {
  constructor(
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _title: Title,
    private readonly _messageService: MessageService,
    private readonly _router: Router,
    private readonly _identityService: IdentityApiService
  ) {
    this._title.setTitle('Подтверждение сброса пароля');
    this._accountId = signal(undefined);
    this._ticketId = signal(undefined);

    effect(() => {
      this._activatedRoute.queryParams.subscribe((params) => {
        const accountId: any | undefined = params['accountId'];
        const ticketId: any | undefined = params['ticketId'];
        if (accountId) this._accountId.set(accountId as string);
        if (ticketId) this._ticketId.set(ticketId as string);
      });
    });
  }

  private readonly _accountId: WritableSignal<string | null | undefined>;
  private readonly _ticketId: WritableSignal<string | null | undefined>;

  readonly form: FormGroup = new FormGroup({
    newPassword: new FormControl(''),
    newPasswordAgain: new FormControl(''),
  });

  public submit(): void {
    const password: string = this.readPassword();
    if (this.isPasswordEmpty(password)) return;
    if (this.isNewPasswordEmpty(password)) return;
    if (!this.arePasswordEqual(password, this.readNewPasswordAgain())) return;
    this.handlePasswordReset(password);
    this.resetForm();
  }

  private handlePasswordReset(newPassword: string): void {
    const accountId: string | null | undefined = this._accountId();
    const ticketId: string | null | undefined = this._ticketId();
    if (!accountId || !ticketId) {
      MessageServiceUtils.showError(
        this._messageService,
        'Некорректная ссылка для сброса пароля.'
      );
      return;
    }
    this._identityService
      .confirmPasswordReset(accountId, ticketId, newPassword)
      .pipe(
        tap(
          (): void => {
            MessageServiceUtils.showSuccess(
              this._messageService,
              'Пароль успешно сброшен.'
            );
            this._router.navigate(['/sign-in']);
          },
          catchError((error: HttpErrorResponse): Observable<never> => {
            const message: string = error.error.message;
            MessageServiceUtils.showError(this._messageService, message);
            return EMPTY;
          })
        )
      )
      .subscribe();
  }

  private readTokenId(): string | null | undefined {
    return this._activatedRoute.snapshot.params['ticketId'];
  }

  private readAccountId(): string | null | undefined {
    return this._activatedRoute.snapshot.params['accountId'];
  }

  private resetForm(): void {
    this.form.reset();
  }

  private arePasswordEqual(password: string, newPassword: string): boolean {
    const areEqual: boolean = password === newPassword;
    if (!areEqual) {
      MessageServiceUtils.showError(
        this._messageService,
        'Пароли не совпадают.'
      );
    }
    return areEqual;
  }

  private isNewPasswordEmpty(newPassword: string): boolean {
    const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(newPassword);
    if (isEmpty) {
      MessageServiceUtils.showError(
        this._messageService,
        'Новый пароль не может быть пустым.'
      );
    }
    return isEmpty;
  }

  private readNewPasswordAgain(): string {
    const formValues: any = this.form.value;
    return formValues.newPasswordAgain as string;
  }

  private readPassword(): string {
    const formValues: any = this.form.value;
    return formValues.newPassword as string;
  }

  private isPasswordEmpty(password: string): boolean {
    const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(password);
    if (isEmpty) {
      MessageServiceUtils.showError(
        this._messageService,
        'Пароль не может быть пустым.'
      );
    }
    return isEmpty;
  }
}
