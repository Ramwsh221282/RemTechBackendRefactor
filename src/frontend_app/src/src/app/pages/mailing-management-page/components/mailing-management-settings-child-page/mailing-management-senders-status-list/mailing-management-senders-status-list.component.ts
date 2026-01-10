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

@Component({
  selector: 'app-mailing-management-senders-status-list',
  imports: [NgClass],
  templateUrl: './mailing-management-senders-status-list.component.html',
  styleUrl: './mailing-management-senders-status-list.component.scss',
})
export class MailingManagementSendersStatusListComponent {
  constructor() {
    this.senders = signal([]);
    this.onSenderPingClicked = new EventEmitter<MailerResponse>();
  }

  @Output() onSenderPingClicked: EventEmitter<MailerResponse>;
  @Input({required: true}) set mailers(value: MailerResponse[]) {
    this.senders.set(value);
  }

  readonly senders: WritableSignal<MailerResponse[]>;

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
