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
import {
  DefaultMailerResponse,
  MailerResponse
} from '../../../../../shared/api/notifications-module/notifications-responses';

@Component({
  selector: 'app-mailing-management-check-sender-form',
  imports: [Dialog, ReactiveFormsModule, InputText, Button, Toast],
  templateUrl: './mailing-management-check-sender-form.component.html',
  styleUrl: './mailing-management-check-sender-form.component.scss',
  providers: [MessageService],
})
export class MailingManagementCheckSenderFormComponent {
  constructor(private readonly _messageService: MessageService) {
    this.sender = signal(DefaultMailerResponse());
    this.onTestMessageSendSubmitted = new EventEmitter<{ mailerId: string, recipientEmail: string }>();
  }

  @Input({ required: true }) set sender_setter(sender: MailerResponse) {
    this.sender.set(sender);
    this.visible = !!sender;
  }

  @Output() onTestMessageSendSubmitted: EventEmitter<{ mailerId: string, recipientEmail: string }>;
  @Output() onClose: EventEmitter<void> = new EventEmitter<void>();

  readonly sender: WritableSignal<MailerResponse>;
  checkSenderForm: FormGroup = new FormGroup({
    sendTo: new FormControl(''),
  });
  visible: boolean = false;

  public onSubmit(): void {
    const mailer: MailerResponse = this.sender();
    const recipient: string = this.readRecipientEmail();
    if (this.isRecipientEmailEmpty(recipient)) return;
    this.onTestMessageSendSubmitted.emit({ mailerId: mailer.Id, recipientEmail: recipient });
  }

  public closeClick(): void {
    this.onClose.emit();
  }

  private isRecipientEmailEmpty(recipient: string): boolean {
    if (StringUtils.isEmptyOrWhiteSpace(recipient)) {
      MessageServiceUtils.showError(this._messageService, 'Почта не указана');
      return true;
    }
    return false;
  }

  private readRecipientEmail(): string {
    const formValues = this.checkSenderForm.value;
    return formValues.sendTo as string;
  }
}
