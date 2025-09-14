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
import { UsersService } from '../../services/UsersService';
import { HttpErrorResponse } from '@angular/common/http';
import { Toast, ToastModule } from 'primeng/toast';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-reset-password-page',
  imports: [FormsModule, ReactiveFormsModule, Toast, ToastModule],
  templateUrl: './reset-password-page.component.html',
  styleUrl: './reset-password-page.component.scss',
  providers: [MessageService],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResetPasswordPageComponent {
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly _messageService: MessageService,
    private readonly _usersService: UsersService,
    private readonly _title: Title,
  ) {
    this._title.setTitle('Сброс пароля');
  }

  public readonly form: FormGroup = new FormGroup({
    login: new FormControl(''),
    email: new FormControl(''),
  });

  public submit($event: MouseEvent): void {
    $event.stopPropagation();
    const formValues = this.form.value;
    const login: string = formValues.login;
    const email: string = formValues.email;
    if (!this.areInputsValid(login, email)) {
      const error: string = 'Необходимо указать или email, или логин.';
      MessageServiceUtils.showError(this._messageService, error);
      return;
    }
    if (!this.isEmailValid(email)) {
      const error: string = 'Формат почты некорректный.';
      MessageServiceUtils.showError(this._messageService, error);
      return;
    }
    this.requestResetPassword(login, email);
  }

  private requestResetPassword(login: string, email: string): void {
    this._usersService.requireResetPassword(login, email).subscribe({
      next: (_: any): void => {
        const message =
          'Данные для восстановления пароля были отправлены на почту, указанную при регистрации.';
        MessageServiceUtils.showInfo(this._messageService, message);
      },
      error: (err: HttpErrorResponse): void => {
        const error = err.error.message;
        MessageServiceUtils.showError(this._messageService, error);
      },
    });
  }

  private areInputsValid(login: string, email: string): boolean {
    return (
      !StringUtils.isEmptyOrWhiteSpace(login) ||
      !StringUtils.isEmptyOrWhiteSpace(email)
    );
  }

  private isEmailValid(email: string): boolean {
    return (
      !StringUtils.isEmptyOrWhiteSpace(email) && StringUtils.isEmailValid(email)
    );
  }
}
