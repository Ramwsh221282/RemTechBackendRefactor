import { Component, DestroyRef, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { UsersService } from '../../services/UsersService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { TokensService } from '../../../../shared/services/TokensService';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-sign-in-form',
  imports: [ReactiveFormsModule, Toast],
  templateUrl: './sign-in-form.component.html',
  styleUrl: './sign-in-form.component.scss',
  providers: [MessageService],
})
export class SignInFormComponent {
  signIngForm: FormGroup = new FormGroup({
    email: new FormControl(''),
    name: new FormControl(''),
    password: new FormControl(''),
  });

  private readonly _messageService: MessageService;
  private readonly _service: UsersService;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);

  constructor(
    messageService: MessageService,
    service: UsersService,
    private readonly _router: Router,
    private readonly _tokensService: TokensService,
    private readonly _cookiesService: CookieService,
  ) {
    this._messageService = messageService;
    this._service = service;
  }

  public submit(): void {
    const formValues = this.signIngForm.value;
    const email: string = formValues.email;
    const name: string = formValues.name;
    const password: string = formValues.password;
    if (StringUtils.isEmptyOrWhiteSpace(password)) {
      MessageServiceUtils.showError(
        this._messageService,
        'Необходимо ввести пароль.',
      );
      return;
    }
    if (StringUtils.allEmpty([email, name])) {
      MessageServiceUtils.showError(
        this._messageService,
        'Необходимо ввести почту или псевдоним',
      );
      return;
    }
    this.authenticate(password, email, name);
  }

  public resetPasswordClick(): void {
    const path = 'reset-password';
    this._router.navigate([path]);
  }

  private authenticate(
    password: string,
    email?: string | null,
    name?: string | null,
  ): void {
    this._service
      .authenticate(password, email, name)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (_: any): void => {
          MessageServiceUtils.showSuccess(
            this._messageService,
            'Авторизация успешна.',
          );
          const tokenId = this._cookiesService.get('RemTechAccessTokenId');
          this._service.verifyAdminAccess(tokenId).subscribe({
            next: (_): void => {
              this._tokensService.setAdmin();
              this._router.navigate(['']);
            },
            error: (_): void => {
              this._tokensService.setNotAdmin();
              this._router.navigate(['']);
            },
          });
        },
        error: (err: HttpErrorResponse): void => {
          const message: string = err.error.message;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }
}
