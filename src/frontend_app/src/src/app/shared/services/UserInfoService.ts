import { Injectable, signal, WritableSignal } from '@angular/core';
import { UserInfo } from '../../pages/sign-in-page/types/UserInfo';

@Injectable({
  providedIn: 'root',
})
export class UserInfoService {
  private readonly userInfoSignal: WritableSignal<UserInfo>;

  constructor() {
    this.userInfoSignal = signal({
      emailConfirmed: false,
      email: '',
      name: '',
      id: '',
    });
  }

  public setUserInfo(userInfo: UserInfo) {
    this.userInfoSignal.set(userInfo);
  }
  public get userInfo(): UserInfo {
    return this.userInfoSignal();
  }

  public get hasUserInfo(): boolean {
    const info = this.userInfoSignal();
    if (info.email === '' || info.name === '' || info.id === '') return false;
    return true;
  }
}
