import {
  Component,
  DestroyRef,
  effect,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { MailingSender } from '../../../models/MailingSender';
import { MailingManagementService } from '../../../services/MailingManagementService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgClass } from '@angular/common';
import {MailerResponse} from '../../../../../shared/api/notifications-module/notifications-responses';
import {ConfirmationService} from 'primeng/api';
import {catchError, EMPTY, map, Observable, tap} from 'rxjs';
import {TypedEnvelope} from '../../../../../shared/api/envelope';
import {ParserResponse} from '../../../../../shared/api/parsers-module/parsers-responses';
import {MessageServiceUtils} from '../../../../../shared/utils/message-service-utils';
import {HttpErrorResponse} from '@angular/common/http';
import {ConfirmDialog} from 'primeng/confirmdialog';

@Component({
  selector: 'app-mailing-management-senders-status-list',
  imports: [NgClass, ConfirmDialog],
  templateUrl: './mailing-management-senders-status-list.component.html',
  styleUrl: './mailing-management-senders-status-list.component.scss',
})
export class MailingManagementSendersStatusListComponent {
  constructor(private readonly _confirmationService: ConfirmationService) {
    this.senders = signal([]);
    this.onSenderPingClicked = new EventEmitter<MailerResponse>();
    this.onSenderRemoveClicked = new EventEmitter<MailerResponse>();
  }

  @Output() onSenderRemoveClicked: EventEmitter<MailerResponse>;
  @Output() onSenderPingClicked: EventEmitter<MailerResponse>;
  @Input({required: true}) set mailers(value: MailerResponse[]) {
    this.senders.set(value);
  }

  readonly senders: WritableSignal<MailerResponse[]>;

  public deleteClick(postix: string): void {
    const sender: MailerResponse | undefined = this.findSenderByPostfix(postix);
    if (!sender) return;

    this._confirmationService.confirm({
      message: `Вы уверены что хотите удалить настройки почтового сервиса ${sender.Email} ${sender.SmtpHost} ?`,
      header: 'Подтверждение',
      closable: true,
      closeOnEscape: true,
      icon: 'pi pi-exclamation-triangle',
      rejectButtonProps: {
        label: 'Отменить',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Подтвердить',
      },
      accept: () => {
        this.onSenderRemoveClicked.emit(sender);
      },
      reject: () => {},
    });
  }

  private getSenderByPostfix(postFix: string): MailerResponse | undefined {
    const senders: MailerResponse[] = this.senders();
    return senders.find((s: MailerResponse): boolean =>
      s.Email.toLowerCase().includes(postFix.toLowerCase()),
    );
  }

  public boolHasSenderWith(postFix: string): boolean {
    const sender: MailerResponse | undefined = this.findSenderByPostfix(postFix);
    return !!sender;
  }

  public pingSenderByPostfix(postfix: string): void {
    const sender: MailerResponse | undefined = this.findSenderByPostfix(postfix);
    if (sender) this.onSenderPingClicked.emit(sender);
  }

  private findSenderByPostfix(postFix: string): MailerResponse | undefined {
    const senders: MailerResponse[] = this.senders();
    return senders.find((s: MailerResponse): boolean =>
      s.Email.toLowerCase().includes(postFix.toLowerCase()),
    );
  }
}
