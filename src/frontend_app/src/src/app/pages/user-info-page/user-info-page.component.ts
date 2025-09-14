import { Component, signal, WritableSignal } from '@angular/core';
import { UserInfo } from '../sign-in-page/types/UserInfo';
import { UserInfoService } from '../../shared/services/UserInfoService';
import { NgIf } from '@angular/common';
import { EmailConfirmationDialogComponent } from './components/email-confirmation-dialog/email-confirmation-dialog.component';
import { EmailChangeDialogComponent } from './components/email-change-dialog/email-change-dialog.component';
import { PasswordChangeDialogComponent } from './components/password-change-dialog/password-change-dialog.component';

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
export class UserInfoPageComponent {
  private readonly _userInfo: WritableSignal<UserInfo>;
  public readonly _isEmailConfirmationDialogVisible: WritableSignal<boolean>;
  private readonly _isChangingEmail: WritableSignal<boolean>;
  private readonly _isChangingPassword: WritableSignal<boolean>;

  constructor(userInfoService: UserInfoService) {
    this._userInfo = signal(userInfoService.userInfo);
    this._isEmailConfirmationDialogVisible = signal(false);
    this._isChangingEmail = signal(false);
    this._isChangingPassword = signal(false);
  }

  public get userInfo(): UserInfo {
    return this._userInfo();
  }

  public changePasswordState($event: boolean): void {
    this._isChangingPassword.set($event);
  }

  public get isChangingPassword(): boolean {
    return this._isChangingPassword();
  }

  public get isChangingEmail(): boolean {
    return this._isChangingEmail();
  }

  public get isConfirmingEmail(): boolean {
    return this._isEmailConfirmationDialogVisible();
  }

  public changeEmailChangeState($event: boolean): void {
    this._isChangingEmail.set($event);
  }

  public changeEmailConfirmationState($event: boolean) {
    this._isEmailConfirmationDialogVisible.set($event);
  }
}
