import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Panel } from 'primeng/panel';
import { Button } from 'primeng/button';
import { ProgressSpinner } from 'primeng/progressspinner';
import { StringUtils } from '../../shared/utils/string-utils';
import { catchError, finalize, Observable } from 'rxjs';
import { CustomHttpErrorFactory } from '../../shared/types/CustomHttpError';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-registration-confirmation-page',
  imports: [Panel, Button, RouterLink, ProgressSpinner],
  templateUrl: './registration-confirmation-page.component.html',
  styleUrl: './registration-confirmation-page.component.scss',
})
export class RegistrationConfirmationPageComponent implements OnInit {
  private readonly _route: ActivatedRoute;
  readonly isLoadingSignal: WritableSignal<boolean>;
  readonly isSuccessSignal: WritableSignal<boolean>;
  readonly errorMessageSignal: WritableSignal<string>;
  readonly statusSignal: WritableSignal<string>;

  constructor(route: ActivatedRoute, title: Title) {
    this._route = route;
    this.isLoadingSignal = signal(false);
    this.isSuccessSignal = signal(false);
    this.errorMessageSignal = signal('');
    this.statusSignal = signal('');
    title.setTitle('Подтверждение почты');
  }

  public ngOnInit() {
    this._route.queryParams.subscribe((params) => {
      const userId = params['userid'];
      const token = params['token'];
      this.processConfirmation(userId, token);
    });
  }

  private processConfirmation(userId: string, token: string): void {}

  private isTokenValid(token: string): boolean {
    if (StringUtils.isEmptyOrWhiteSpace(token)) {
      this.errorMessageSignal.set('Некорректный токен подтверждения.');
      return false;
    }
    return true;
  }

  private isUserIdValid(userId: string): boolean {
    if (StringUtils.isEmptyOrWhiteSpace(userId)) {
      this.errorMessageSignal.set('Некорректный идентификатор пользователя.');
      return false;
    }
    return true;
  }
}
