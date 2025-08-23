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
import { MessageService } from 'primeng/api';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { UsersService } from '../../../sign-in-page/services/UsersService';
import { TokensService } from '../../../../shared/services/TokensService';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Toast } from 'primeng/toast';
import { UserInfoService } from '../../../../shared/services/UserInfoService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-email-change-dialog',
  imports: [Button, Dialog, FormsModule, InputText, Toast, ReactiveFormsModule],
  templateUrl: './email-change-dialog.component.html',
  styleUrl: './email-change-dialog.component.scss',
  providers: [MessageService],
})
export class EmailChangeDialogComponent {
  @Output() onSubmitOrCancel: EventEmitter<boolean>;
  @Input({ required: true }) set visible_setter(value: boolean) {
    this.visible.set(value);
  }

  public readonly form: FormGroup = new FormGroup({
    password: new FormControl(''),
    email: new FormControl(''),
  });

  public readonly visible: WritableSignal<boolean>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly messageService: MessageService,
    private readonly usersService: UsersService,
    private readonly tokensService: TokensService,
    private readonly userInfoService: UserInfoService,
  ) {
    this.visible = signal(false);
    this.onSubmitOrCancel = new EventEmitter<boolean>();
  }

  public submitForm(): void {
    const formValues = this.form.value;
    const password = formValues.password as string;
    const email = formValues.email as string;
    if (StringUtils.isEmptyOrWhiteSpace(password)) {
      MessageServiceUtils.showError(
        this.messageService,
        'Необходимо ввести пароль',
      );
      return;
    }
    if (StringUtils.isEmptyOrWhiteSpace(email)) {
      MessageServiceUtils.showError(
        this.messageService,
        'Необходимо ввести почту',
      );
      return;
    }
    this.createChangeEmailTicket(email, password);
  }

  public cancel(): void {
    this.form.reset();
    this.onSubmitOrCancel.emit(false);
  }

  public createChangeEmailTicket(newEmail: string, password: string): void {
    const token: string = this.tokensService.tokenId();
    const userId: string = this.userInfoService.userInfo.id;
    this.usersService
      .createChangeEmailTicket(token, userId, password, newEmail)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (_: any): void => {
          const message = `Письмо подтверждения почты отправлено на новую почту.`;
          MessageServiceUtils.showSuccess(this.messageService, message);
          this.form.reset();
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this.messageService, message);
          this.form.reset();
        },
      });
  }
}
