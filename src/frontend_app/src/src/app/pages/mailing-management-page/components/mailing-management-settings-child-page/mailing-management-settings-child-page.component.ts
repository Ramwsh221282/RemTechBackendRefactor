import { Component, signal, WritableSignal } from '@angular/core';
import { MailingManagementCreateSenderFormComponent } from './mailing-management-create-sender-form/mailing-management-create-sender-form.component';
import { MailingManagementSendersStatusListComponent } from './mailing-management-senders-status-list/mailing-management-senders-status-list.component';
import { MailingSender } from '../../models/MailingSender';
import { MailingManagementCheckSenderFormComponent } from './mailing-management-check-sender-form/mailing-management-check-sender-form.component';

@Component({
  selector: 'app-mailing-management-settings-child-page',
  imports: [
    MailingManagementCreateSenderFormComponent,
    MailingManagementSendersStatusListComponent,    
    MailingManagementCheckSenderFormComponent,
  ],
  templateUrl: './mailing-management-settings-child-page.component.html',
  styleUrl: './mailing-management-settings-child-page.component.scss',
})
export class MailingManagementSettingsChildPageComponent {
  private readonly _shouldFetch: WritableSignal<boolean>;
  private readonly _mailingSenderToPing: WritableSignal<MailingSender | null>;
  constructor() {
    this._mailingSenderToPing = signal(null);
    this._shouldFetch = signal(false);
  }

  public senderAdded(_: MailingSender): void {
    this._shouldFetch.set(true);
  }

  public fetched(value: boolean): void {
    this._shouldFetch.set(value);
  }

  public get shouldFetch(): boolean {
    return this._shouldFetch();
  }

  public get mailingSenderToPing(): MailingSender | null {
    return this._mailingSenderToPing();
  }

  public acceptSenderToPing(sender: MailingSender): void {
    this._mailingSenderToPing.set(sender);
  }

  public closePingDialog(): void {
    this._mailingSenderToPing.set(null);
  }
}
