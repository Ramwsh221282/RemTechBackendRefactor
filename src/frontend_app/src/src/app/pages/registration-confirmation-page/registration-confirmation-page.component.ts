import {
  Component,
  DestroyRef,
  effect,
  inject,
  OnInit,
  signal,
  WritableSignal,
} from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Panel } from 'primeng/panel';
import { Button } from 'primeng/button';
import { ProgressSpinner } from 'primeng/progressspinner';
import { StringUtils } from '../../shared/utils/string-utils';
import { Title } from '@angular/platform-browser';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UsersService } from '../sign-in-page/services/UsersService';
import { HttpErrorResponse } from '@angular/common/http';
import { UserInfoService } from '../../shared/services/UserInfoService';
import { TokensService } from '../../shared/services/TokensService';
import { UserInfo } from '../sign-in-page/types/UserInfo';

@Component({
  selector: 'app-registration-confirmation-page',
  imports: [Panel, Button, RouterLink, ProgressSpinner],
  templateUrl: './registration-confirmation-page.component.html',
  styleUrl: './registration-confirmation-page.component.scss',
})
export class RegistrationConfirmationPageComponent {
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  readonly isLoadingSignal: WritableSignal<boolean>;
  readonly isSuccessSignal: WritableSignal<boolean>;
  readonly errorMessageSignal: WritableSignal<string>;
  readonly statusSignal: WritableSignal<string>;
  readonly confirmationKey: WritableSignal<string | undefined>;

  constructor(
    route: ActivatedRoute,
    title: Title,
    service: UsersService,
    userInfoService: UserInfoService,
    tokensService: TokensService,
  ) {
    this.isLoadingSignal = signal(false);
    this.isSuccessSignal = signal(false);
    this.errorMessageSignal = signal('');
    this.statusSignal = signal('');
    this.confirmationKey = signal(undefined);
    title.setTitle('Подтверждение почты');

    effect(() => {
      route.queryParams.pipe(takeUntilDestroyed(this._destroyRef)).subscribe({
        next: (param: any): void => {
          const key: string | undefined = param.confirmationKey;
          this.confirmationKey.set(key);
        },
      });
    });

    effect(() => {
      const key: string | undefined = this.confirmationKey();
      if (key === undefined) {
        this.isSuccessSignal.set(false);
        this.errorMessageSignal.set(
          'Ошибка в ключе подтверждения. Попробуйте повторить попытку.',
        );
        this.statusSignal.set('Ошибка ключа подтверждения.');
        return;
      }
      this.isLoadingSignal.set(true);
      service
        .confirmEmail(key)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (_: any): void => {
            this.isLoadingSignal.set(false);
            this.statusSignal.set('Успешно');
            this.isSuccessSignal.set(true);
          },
          error: (err: HttpErrorResponse): void => {
            const message = err.error.message as string;
            this.statusSignal.set('Ошибка');
            this.errorMessageSignal.set(message);
            this.isLoadingSignal.set(false);
            this.isSuccessSignal.set(false);
          },
        });
    });

    effect(() => {
      const key: string | undefined = this.confirmationKey();
      if (key === undefined) return;
      const currentUserInfo: UserInfo = userInfoService.userInfo;
      if (!StringUtils.isEmptyOrWhiteSpace(currentUserInfo.id)) {
        if (tokensService.hasToken()) {
          service
            .fetchUserInfo(tokensService.tokenId())
            .pipe(takeUntilDestroyed(this._destroyRef))
            .subscribe({
              next: (info: UserInfo): void => {
                userInfoService.setUserInfo(info);
              },
            });
        }
      }
    });
  }
}
