import {
  ChangeDetectionStrategy,
  Component,
  effect,
  signal,
  WritableSignal,
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { StringUtils } from '../../shared/utils/string-utils';
import { MessageService } from 'primeng/api';
import { UsersService } from '../sign-in-page/services/UsersService';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../shared/utils/message-service-utils';
import { finalize } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinner } from 'primeng/progressspinner';
import { PanelModule } from 'primeng/panel';
import { Toast } from 'primeng/toast';

@Component({
  selector: 'app-password-reset-confirmation-page',
  imports: [ButtonModule, ProgressSpinner, PanelModule, RouterLink, Toast],
  providers: [MessageService],
  templateUrl: './PasswordResetConfirmationPage.component.html',
  styleUrl: './PasswordResetConfirmationPage.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PasswordResetConfirmationPageComponent {
  private readonly _keySignal: WritableSignal<string>;
  private readonly _isLoadingSignal: WritableSignal<boolean>;
  private readonly _statusSignal: WritableSignal<string>;
  private readonly _isSuccessSignal: WritableSignal<boolean>;
  private readonly _errorMessageSignal: WritableSignal<string>;
  constructor(
    activatedRoute: ActivatedRoute,
    private readonly _messageService: MessageService,
    private readonly _usersService: UsersService,
  ) {
    this._keySignal = signal('');
    this._isLoadingSignal = signal(false);
    this._statusSignal = signal('');
    this._isSuccessSignal = signal(false);
    this._errorMessageSignal = signal('');
    effect(() => {
      activatedRoute.queryParams.subscribe({
        next: (value): void => {
          const key: string | undefined = value['confirmationKey'];
          if (key && !StringUtils.isEmptyOrWhiteSpace(key))
            this._keySignal.set(key);
        },
      });
    });
    effect(() => {
      const keyValue = this._keySignal();
      if (StringUtils.isEmptyOrWhiteSpace(keyValue)) return;
      this._isLoadingSignal.set(true);
      this._usersService
        .confirmResetPassword(keyValue)
        .pipe(finalize(() => this._isLoadingSignal.set(false)))
        .subscribe({
          next: (_: any): void => {
            MessageServiceUtils.showSuccess(
              this._messageService,
              'Новый пароль был отправлен на почту.',
            );
            this._statusSignal.set('Успешно');
            this._isSuccessSignal.set(true);
          },
          error: (err: HttpErrorResponse): void => {
            const message = err.error.message;
            MessageServiceUtils.showError(this._messageService, message);
            this._isSuccessSignal.set(false);
            this._statusSignal.set('Ошибка');
            this._errorMessageSignal.set(message);
          },
        });
    });
  }

  public get error(): string {
    return this._errorMessageSignal();
  }

  public get isLoading(): boolean {
    return this._isLoadingSignal();
  }

  public get status(): string {
    return this._statusSignal();
  }

  public get success(): boolean {
    return this._isSuccessSignal();
  }
}
