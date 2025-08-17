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
import { MailingSender } from '../../../models/MailingSender';
import { MailingManagementService } from '../../../services/MailingManagementService';
import { Dialog } from 'primeng/dialog';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StringUtils } from '../../../../../shared/utils/string-utils';
import { MessageService } from 'primeng/api';
import { MessageServiceUtils } from '../../../../../shared/utils/message-service-utils';
import { InputText } from 'primeng/inputtext';
import { Button } from 'primeng/button';
import { Toast } from 'primeng/toast';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-mailing-management-check-sender-form',
  imports: [Dialog, ReactiveFormsModule, InputText, Button, Toast],
  templateUrl: './mailing-management-check-sender-form.component.html',
  styleUrl: './mailing-management-check-sender-form.component.scss',
  providers: [MessageService],
})
export class MailingManagementCheckSenderFormComponent {
  @Output() onClose: EventEmitter<void> = new EventEmitter<void>();
  private readonly _sender: WritableSignal<MailingSender | null>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _service: MailingManagementService;
  private readonly _messageService: MessageService;
  checkSenderForm: FormGroup = new FormGroup({
    sendTo: new FormControl(''),
  });
  constructor(
    service: MailingManagementService,
    messageService: MessageService,
  ) {
    this._sender = signal({ email: '', name: '' });
    this._service = service;
    this._messageService = messageService;
  }
  @Input({ required: true }) set sender_setter(sender: MailingSender | null) {
    this._sender.set(sender);
    this.visible = !!sender;
  }

  public visible: boolean = false;

  public get senderEmail(): string {
    const sender: MailingSender | null = this._sender();
    return !!sender ? sender.email : '';
  }

  public onSubmit(): void {
    const formValues = this.checkSenderForm.value;
    const sendTo: string = formValues.sendTo as string;
    if (StringUtils.isEmptyOrWhiteSpace(sendTo)) {
      MessageServiceUtils.showError(
        this._messageService,
        'Не указана почта куда отправить тестовое сообщение.',
      );
      return;
    }
    if (!StringUtils.isEmailValid(sendTo)) {
      MessageServiceUtils.showError(
        this._messageService,
        'Формат почты некоррекнтый.',
      );
      return;
    }
    this.invokeTest(sendTo);
  }

  private invokeTest(to: string): void {
    const sender: MailingSender | null = this.sender;
    if (!sender) return;
    this._service
      .ping(sender, to)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (sended: MailingSender) => {
          MessageServiceUtils.showInfo(
            this._messageService,
            `Письмо было отправлено на: ${to} от ${sended.email}.`,
          );
          this.checkSenderForm.reset();
        },
        error: (error: HttpErrorResponse): void => {
          MessageServiceUtils.showError(this._messageService, error.message);
        },
      });
  }

  public get sender(): MailingSender | null {
    return this._sender();
  }

  public closeClick(): void {
    this._sender.set(null);
    this.onClose.emit();
  }
}
