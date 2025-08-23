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
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Button } from 'primeng/button';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { UsersService } from '../../../sign-in-page/services/UsersService';
import { TokensService } from '../../../../shared/services/TokensService';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { Toast } from 'primeng/toast';

@Component({
  selector: 'app-email-confirmation-dialog',
  imports: [Dialog, InputText, Button, ReactiveFormsModule, Toast],
  templateUrl: './email-confirmation-dialog.component.html',
  styleUrl: './email-confirmation-dialog.component.scss',
  providers: [MessageService],
})
export class EmailConfirmationDialogComponent {
  @Output() onSubmitOrCancel: EventEmitter<boolean>;
  @Input({ required: true }) set visible_setter(value: boolean) {
    this.visible.set(value);
  }

  public readonly form: FormGroup = new FormGroup({
    password: new FormControl(''),
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
    if (StringUtils.isEmptyOrWhiteSpace(password)) {
      MessageServiceUtils.showError(
        this.messageService,
        'Необходимо ввести пароль',
      );
      return;
    }
    this.createConfirmEmailTicket(password);
  }

  public cancel(): void {
    this.form.reset();
    this.onSubmitOrCancel.emit(false);
  }

  public createConfirmEmailTicket(password: string): void {
    const token = this.tokensService.tokenId();
    this.usersService
      .createEmailConfirmationTicket(password, token)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (_: string): void => {
          const message = 'Письмо подтверждения отправлено на почту.';
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
