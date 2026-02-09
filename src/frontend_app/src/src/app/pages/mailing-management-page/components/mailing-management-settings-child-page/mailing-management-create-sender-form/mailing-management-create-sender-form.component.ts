import {
  Component,
  DestroyRef,
  EventEmitter,
  inject,
  Output,
} from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StringUtils } from '../../../../../shared/utils/string-utils';
import { MessageService } from 'primeng/api';
import { Toast } from 'primeng/toast';
import { MessageServiceUtils } from '../../../../../shared/utils/message-service-utils';
import { MailingManagementService } from '../../../services/MailingManagementService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MailingSender } from '../../../models/MailingSender';
import { HttpErrorResponse } from '@angular/common/http';
import {
  DefaultMailerResponse,
  MailerResponse
} from '../../../../../shared/api/notifications-module/notifications-responses';

@Component({
  selector: 'app-mailing-management-create-sender-form',
  imports: [ReactiveFormsModule, Toast],
  templateUrl: './mailing-management-create-sender-form.component.html',
  styleUrl: './mailing-management-create-sender-form.component.scss',
})
export class MailingManagementCreateSenderFormComponent {
  constructor(private readonly _messageService: MessageService) {
    this.createMailerSubmit = new EventEmitter();
  }

  @Output() createMailerSubmit: EventEmitter<{ Email: string, Password: string }>;

  createEmailSenderForm = new FormGroup({
    senderEmail: new FormControl(''),
    senderPassword: new FormControl(''),
  });

  public sumbit(): void {
    const email: string = this.readSenderEmail();
    const password: string = this.readSenderPassword();
    this.create(email, password);
  }

  private create(email: string, password: string): void {
    if (this.isEmailEmpty(email)) return;
    if (this.isPasswordEmpty(password)) return;
    this.createMailerSubmit.emit({ Email: email, Password: password });
    this.createEmailSenderForm.reset();
  }

  private isEmailEmpty(email: string): boolean {
    const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(email);
    if (isEmpty) MessageServiceUtils.showError(this._messageService, 'Почта не указана')
    return isEmpty;
  }

  private isPasswordEmpty(password: string): boolean {
    const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(password);
    if (isEmpty) MessageServiceUtils.showError(this._messageService, 'Пароль не указан');
    return isEmpty;
  }

  private readSenderEmail(): string {
    const formValues = this.createEmailSenderForm.value;
    return formValues.senderEmail as string;
  }

  private readSenderPassword(): string {
    const formValues = this.createEmailSenderForm.value;
    return formValues.senderPassword as string;
  }

  private resetForm(): void {
    this.createEmailSenderForm.reset();
  }
}
