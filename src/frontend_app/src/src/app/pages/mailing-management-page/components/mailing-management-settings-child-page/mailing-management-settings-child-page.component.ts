import {Component, DestroyRef, effect, inject, signal, WritableSignal} from '@angular/core';
import { MailingManagementCreateSenderFormComponent } from './mailing-management-create-sender-form/mailing-management-create-sender-form.component';
import { MailingManagementCheckSenderFormComponent } from './mailing-management-check-sender-form/mailing-management-check-sender-form.component';
import {MessageService} from 'primeng/api';
import {NotificationsApiService} from '../../../../shared/api/notifications-module/notifications-api.service';
import {MailerResponse} from '../../../../shared/api/notifications-module/notifications-responses';
import {catchError, EMPTY, map, Observable, tap} from 'rxjs';
import {TypedEnvelope} from '../../../../shared/api/envelope';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {HttpErrorResponse} from '@angular/common/http';
import {MessageServiceUtils} from '../../../../shared/utils/message-service-utils';
import {
  MailingManagementSendersStatusListComponent
} from './mailing-management-senders-status-list/mailing-management-senders-status-list.component';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-mailing-management-settings-child-page',
  imports: [MailingManagementCreateSenderFormComponent, MailingManagementCheckSenderFormComponent, MailingManagementSendersStatusListComponent, NgIf],
  templateUrl: './mailing-management-settings-child-page.component.html',
  styleUrl: './mailing-management-settings-child-page.component.scss',
})
export class MailingManagementSettingsChildPageComponent {
  constructor(
    private readonly _messageService: MessageService,
    private readonly _service: NotificationsApiService,
  ) {
    this.mailerToPing = signal(null);
    this.mailers = signal([]);
    effect((): void => {
      this.fetchMailers();
    });
  }

  readonly mailers: WritableSignal<MailerResponse[]>;
  readonly mailerToPing: WritableSignal<MailerResponse | null>
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);

  public onSenderCreateSubmit(payload: { Email: string, Password: string }): void {
    this.handleSenderCreate(payload.Email, payload.Password);
  }

  public acceptSenderToPing(mailer: MailerResponse): void {
    this.mailerToPing.set(mailer);
  }

  public closePingDialog(): void {
    this.mailerToPing.set(null);
  }

  private handleSenderCreate(email: string, password: string): void {
    this._service.addMailer(email, password)
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((envelope: TypedEnvelope<MailerResponse>): MailerResponse | null => envelope.body ?? null),
        tap((mailer: MailerResponse | null): void => {
          if (mailer) {
            const message = `Создана конфигурация почтового сервиса: ${mailer.Email} ${mailer.SmtpHost}`;
            MessageServiceUtils.showSuccess(this._messageService, message);
            this.fetchMailers();
          }
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe()
  }

  private fetchMailers(): void {
    this._service.fetchMailers()
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((envelope: TypedEnvelope<MailerResponse[]>): MailerResponse[] => envelope.body ?? []),
        tap((mailers: MailerResponse[]): void => this.mailers.set(mailers)),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe()
  }
}
