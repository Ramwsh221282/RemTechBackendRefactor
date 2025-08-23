import {
  Component,
  DestroyRef,
  EventEmitter,
  inject,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MessageService } from 'primeng/api';
import { UsersService } from '../../../sign-in-page/services/UsersService';
import { TokensService } from '../../../../shared/services/TokensService';
import { UserInfoService } from '../../../../shared/services/UserInfoService';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Toast } from 'primeng/toast';
import { PasswordRulesComponent } from '../../../../shared/components/password-rules/password-rules.component';

@Component({
  selector: 'app-password-change-dialog',
  imports: [
    Button,
    Dialog,
    FormsModule,
    InputText,
    Toast,
    ReactiveFormsModule,
    PasswordRulesComponent,
  ],
  templateUrl: './password-change-dialog.component.html',
  styleUrl: './password-change-dialog.component.scss',
  providers: [MessageService],
})
export class PasswordChangeDialogComponent {
  @Output() onSubmitOrCancel: EventEmitter<boolean>;
  @Input({ required: true }) set visible_setter(value: boolean) {
    this.visible.set(value);
  }

  public readonly form: FormGroup = new FormGroup({
    password: new FormControl(''),
    newPassword: new FormControl(''),
  });

  public readonly visible: WritableSignal<boolean>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly messageService: MessageService,
    private readonly usersService: UsersService,
    private readonly tokensService: TokensService,
  ) {
    this.visible = signal(false);
    this.onSubmitOrCancel = new EventEmitter<boolean>();
  }

  public submitForm(): void {
    const formValues = this.form.value;
    const password = formValues.password as string;
    const newPassword = formValues.newPassword as string;
    if (StringUtils.isEmptyOrWhiteSpace(password)) {
      MessageServiceUtils.showError(
        this.messageService,
        'Необходимо ввести пароль',
      );
      return;
    }
    if (StringUtils.isEmptyOrWhiteSpace(newPassword)) {
      MessageServiceUtils.showError(
        this.messageService,
        'Необходимо ввести почту',
      );
      return;
    }
    this.processPasswordChange(password, newPassword);
  }

  public cancel(): void {
    this.form.reset();
    this.onSubmitOrCancel.emit(false);
  }

  public processPasswordChange(password: string, newPassword: string): void {
    const token: string = this.tokensService.tokenId();
    this.usersService
      .changeUserPassword(token, password, newPassword)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (_: any): void => {
          const message = 'Пароль был успешно изменен.';
          MessageServiceUtils.showSuccess(this.messageService, message);
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message;
          MessageServiceUtils.showError(this.messageService, message);
        },
      });
  }
}
