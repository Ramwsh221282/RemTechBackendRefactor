import {Component, DestroyRef, effect, inject, OnInit, signal, WritableSignal} from '@angular/core';
import { UserInfo } from '../sign-in-page/types/UserInfo';
import { UserInfoService } from '../../shared/services/UserInfoService';
import { NgIf } from '@angular/common';
import { EmailConfirmationDialogComponent } from './components/email-confirmation-dialog/email-confirmation-dialog.component';
import { EmailChangeDialogComponent } from './components/email-change-dialog/email-change-dialog.component';
import { PasswordChangeDialogComponent } from './components/password-change-dialog/password-change-dialog.component';
import {AccountResponse} from '../../shared/api/identity-module/identity-responses';
import {IdentityApiService} from '../../shared/api/identity-module/identity-api-service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {tap} from 'rxjs';
import {TypedEnvelope} from '../../shared/api/envelope';

@Component({
  selector: 'app-user-info-page',
  imports: [
    NgIf,
    EmailConfirmationDialogComponent,
    EmailChangeDialogComponent,
    PasswordChangeDialogComponent,
  ],
  templateUrl: './user-info-page.component.html',
  styleUrl: './user-info-page.component.scss',
})
export class UserInfoPageComponent implements OnInit {
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  readonly account: WritableSignal<AccountResponse>;

  constructor(private readonly _service: IdentityApiService) {
    this.account = signal({ Id: '', Email: '', Login: '', IsActivated: false, Permissions: [] })
  }

  ngOnInit(): void {
    this._service.fetchAccount()
      .pipe(takeUntilDestroyed(this._destroyRef),
        tap({
          next: (envelope: TypedEnvelope<AccountResponse>): void => {
            if (envelope.body) {
              this.account.set(envelope.body)
            }
          }
        }))
      .subscribe()
    }
}
