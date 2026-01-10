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
import {catchError, EMPTY, map, Observable, tap} from 'rxjs';
import {TypedEnvelope} from '../../shared/api/envelope';
import {ConfirmationService, MessageService} from 'primeng/api';
import {Toast} from 'primeng/toast';
import {ConfirmDialog} from 'primeng/confirmdialog';
import {PermissionsStatusService} from '../../shared/services/PermissionsStatus.service';
import {AuthenticationStatusService} from '../../shared/services/AuthenticationStatusService';
import {Router} from '@angular/router';
import {HttpErrorResponse} from '@angular/common/http';
import {MessageServiceUtils} from '../../shared/utils/message-service-utils';
import {ParserResponse} from '../../shared/api/parsers-module/parsers-responses';

@Component({
  selector: 'app-user-info-page',
  imports: [
    NgIf,
    EmailConfirmationDialogComponent,
    EmailChangeDialogComponent,
    PasswordChangeDialogComponent,
    Toast,
    ConfirmDialog,
  ],
  templateUrl: './user-info-page.component.html',
  styleUrl: './user-info-page.component.scss',
})
export class UserInfoPageComponent implements OnInit {
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  readonly account: WritableSignal<AccountResponse>;

  constructor(
    private readonly _service: IdentityApiService,
    private readonly _messageService: MessageService,
    private readonly _confirmationService: ConfirmationService,
    private readonly _permissionsService: PermissionsStatusService,
    private readonly _authService: AuthenticationStatusService,
    private readonly _router: Router) {
    this.account = signal({ Id: '', Email: '', Login: '', IsActivated: false, Permissions: [] })
  }

  public logoutClicked(): void {
    this._confirmationService.confirm({
      message: 'Вы действительно хотите выйти?',
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
        this.handleLogout();
      },
      reject: () => {},
    });
  }

  private handleLogout(): void {
    this._service.logout()
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: ():void => {
          this._authService.setIsNotAuthenticated();
          this._permissionsService.clean();
          this._router.navigate([''])
        },
        error: (err: HttpErrorResponse): void => {
          const message: string = err.error.message;
          MessageServiceUtils.showError(this._messageService, message)
        },
      });
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
