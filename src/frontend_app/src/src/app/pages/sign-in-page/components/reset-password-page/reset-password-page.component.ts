import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  inject,
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MessageService } from 'primeng/api';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { HttpErrorResponse } from '@angular/common/http';
import { Toast, ToastModule } from 'primeng/toast';
import { Title } from '@angular/platform-browser';
import { IdentityApiService } from '../../../../shared/api/identity-module/identity-api-service';
import { tap } from 'rxjs/internal/operators/tap';
import { Envelope } from '../../../../shared/api/envelope';
import { catchError, EMPTY, Observable } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-reset-password-page',
  imports: [FormsModule, ReactiveFormsModule, Toast, ToastModule],
  templateUrl: './reset-password-page.component.html',
  styleUrl: './reset-password-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResetPasswordPageComponent {
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly _messageService: MessageService,
    private readonly _identityService: IdentityApiService,
    private readonly _title: Title
  ) {
    this._title.setTitle('Сброс пароля');
  }

  readonly form: FormGroup = new FormGroup({
    login: new FormControl(''),
    email: new FormControl(''),
  });

  public submit($event: MouseEvent): void {
    $event.stopPropagation();
    const login: string = this.readLogin();
    const email: string = this.readEmail();
    if (this.areAllInputsEmpty(login, email)) return;
    this.requestResetPassword(login, email);
    this.resetForm();
  }

  private requestResetPassword(login: string, email: string): void {
    this._identityService
      .resetPassword(login, email)
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        tap((_: Envelope): void => {
          const message: string = `Инструкция по сбросу пароля отправлена на адрес электронной почты учетной записи.`;
          MessageServiceUtils.showSuccess(this._messageService, message);
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      )
      .subscribe();
  }

  private areAllInputsEmpty(login: string, email: string): boolean {
    const loginEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(login);
    const emailEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(email);
    if (loginEmpty && emailEmpty) {
      MessageServiceUtils.showError(
        this._messageService,
        'Необходимо указать или email, или логин.'
      );
      return true;
    }
    return false;
  }

  private resetForm(): void {
    this.form.reset();
  }

  private readLogin(): string {
    const formValues = this.form.value;
    return formValues.login as string;
  }

  private readEmail(): string {
    const formValues = this.form.value;
    return formValues.email as string;
  }
}
