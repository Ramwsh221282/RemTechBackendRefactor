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

@Component({
  selector: 'app-mailing-management-create-sender-form',
  imports: [ReactiveFormsModule, Toast],
  templateUrl: './mailing-management-create-sender-form.component.html',
  styleUrl: './mailing-management-create-sender-form.component.scss',
  providers: [MessageService],
})
export class MailingManagementCreateSenderFormComponent {
  @Output() mailingSenderAdded: EventEmitter<MailingSender> =
    new EventEmitter<MailingSender>();

  createEmailSenderForm = new FormGroup({
    senderEmail: new FormControl(''),
    senderPassword: new FormControl(''),
  });

  private readonly _messageService: MessageService;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _service: MailingManagementService;

  constructor(
    messageService: MessageService,
    service: MailingManagementService,
  ) {
    this._messageService = messageService;
    this._service = service;
  }

  public sumbit(): void {
    const formValue = this.createEmailSenderForm.value;
    const senderEmail: string = formValue.senderEmail as string;
    const senderPassword: string = formValue.senderPassword as string;
    if (StringUtils.isEmptyOrWhiteSpace(senderEmail)) {
      MessageServiceUtils.showError(this._messageService, 'Не указана почта.');
      return;
    }
    if (!StringUtils.isEmailValid(senderEmail)) {
      MessageServiceUtils.showError(
        this._messageService,
        'Некорректный формат почты.',
      );
      return;
    }
    if (StringUtils.isEmptyOrWhiteSpace(senderPassword)) {
      MessageServiceUtils.showError(
        this._messageService,
        'Не предоставлен пароль.',
      );
      return;
    }
    this.create(senderEmail, senderPassword);
  }

  private create(email: string, password: string): void {
    this._service
      .create(email, password)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (result: MailingSender): void => {
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Подключен почтовый сервис: ${result.email}`,
          );
          this.mailingSenderAdded.emit(result);
          this.createEmailSenderForm.reset();
        },
        error: (err: HttpErrorResponse): void => {
          MessageServiceUtils.showError(this._messageService, err.message);
        },
      });
  }
}
