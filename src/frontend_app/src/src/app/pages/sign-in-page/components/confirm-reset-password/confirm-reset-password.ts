import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { IdentityApiService } from '../../../../shared/api/identity-module/identity-api-service';
import { FormControl, FormGroup } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { catchError, EMPTY, Observable, tap } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-confirm-reset-password',
  imports: [],
  template: `./confirm-reset-password.html`,
  styleUrl: './confirm-reset-password.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmResetPassword {
  // todo handle activated route.
  // todo add route for this component.
  // todo add html and styles for this component.
  // todo navigate to sign-in page on success.

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
  }

  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _accountId: WritableSignal<string | null | undefined>;
  private readonly _ticketId: WritableSignal<string | null | undefined>;

  readonly form: FormGroup = new FormGroup({
    newPassword: new FormControl(''),
  });

  public submitForm(): void {
    const password: string = this.readPassword();
    if (this.isPasswordEmpty(password)) return;
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

  private resetForm(): void {
    this.form.reset();
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
