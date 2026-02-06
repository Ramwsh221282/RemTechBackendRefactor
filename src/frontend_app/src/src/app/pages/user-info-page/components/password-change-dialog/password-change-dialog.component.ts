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
})
export class PasswordChangeDialogComponent {
  constructor(
    private readonly _messageService: MessageService,
  ) {
    this.visible = signal(false);
    this.passwordChangeSubmitted = new EventEmitter<{ newPassword: string, currentPassword: string }>();
    this.onCancel = new EventEmitter<boolean>();
  }

  @Output() passwordChangeSubmitted: EventEmitter<{ newPassword: string, currentPassword: string }>;
  @Output() onCancel: EventEmitter<boolean>;
  @Input({ required: true }) set visible_setter(value: boolean) {
    this.visible.set(value);
  }
  readonly form: FormGroup = new FormGroup({
    currentPassword: new FormControl(''),
    newPassword: new FormControl(''),
  });
  readonly visible: WritableSignal<boolean>;

  public submitForm(): void {
    const newPassword: string = this.readNewPassword();
    const currentPassword: string = this.readCurrentPassword();
    if (this.isPasswordEmpty(newPassword)) return;
    if (this.isPasswordEmpty(currentPassword)) return;
    this.passwordChangeSubmitted.emit({ newPassword, currentPassword });
    this.resetForm();
  }

  public cancel(): void {
    this.form.reset();
    this.onCancel.emit(true);
  }

  private readNewPassword(): string {
    const formValues = this.form.value;
    return formValues.newPassword as string;
  }

  private readCurrentPassword(): string {
    const formValues = this.form.value;
    return formValues.currentPassword as string;
  }

  private resetForm(): void {
    this.form.reset();
  }

  private isPasswordEmpty(password: string): boolean {
    const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(password);
    if (isEmpty) MessageServiceUtils.showError(this._messageService, 'Пароль не указан');
    return isEmpty;
  }
}
