import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { Title } from '@angular/platform-browser';
import { MessageService } from 'primeng/api';
import { IdentityApiService } from '../../../../shared/api/identity-module/identity-api-service';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, EMPTY, Observable, tap } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';

@Component({
  selector: 'app-confirm-ticket-page',
  imports: [],
  template: `<p>confirm-ticket-page works!</p>`,
  styleUrl: './confirm-ticket-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmTicketPage {
  private readonly _ticketId: WritableSignal<string | null | undefined>;
  private readonly _accountId: WritableSignal<string | null | undefined>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly _title: Title,
    private readonly _messageService: MessageService,
    private readonly _router: Router,
    private readonly _identityService: IdentityApiService,
    private readonly _activatedRoute: ActivatedRoute
  ) {
    this._ticketId = signal(undefined);
    this._accountId = signal(undefined);
    this._title.setTitle('Подтверждение регистрации');
    effect(() => {
      this._activatedRoute.queryParams.subscribe((params) => {
        const ticketId: any | undefined = params['ticketId'];
        const accountId: any | undefined = params['accountId'];
        if (accountId) this._accountId.set(accountId as string);
        if (ticketId) this._ticketId.set(ticketId as string);
        this._identityService
          .confirmTicket(accountId, ticketId)
          .pipe(
            takeUntilDestroyed(this._destroyRef),
            tap(
              (): void => {
                this._router.navigate(['/sign-in']);
              },
              catchError((error: HttpErrorResponse): Observable<never> => {
                const message: string = error.error.message;
                MessageServiceUtils.showError(this._messageService, message);
                return EMPTY;
              })
            )
          )
          .subscribe();
      });
    });
  }
}
