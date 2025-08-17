import { Component, DestroyRef, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { UsersService } from '../sign-in-page/services/UsersService';
import { StringUtils } from '../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../shared/utils/message-service-utils';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { SignInFormComponent } from '../sign-in-page/components/sign-in-form/sign-in-form.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-root-user-page',
  imports: [ReactiveFormsModule, Toast, SignInFormComponent],
  templateUrl: './create-root-user-page.component.html',
  styleUrl: './create-root-user-page.component.scss',
  providers: [MessageService],
})
export class CreateRootUserPageComponent {
  signUpForm: FormGroup = new FormGroup({
    email: new FormControl(''),
    name: new FormControl(''),
    password: new FormControl(''),
  });

  private readonly _messageService: MessageService;
  private readonly _usersService: UsersService;
  private readonly _destoryRef: DestroyRef = inject(DestroyRef);
  private readonly _router: Router;
  constructor(
    messageService: MessageService,
    usersService: UsersService,
    router: Router,
  ) {
    this._messageService = messageService;
    this._usersService = usersService;
    this._router = router;
  }

  public submit(): void {
    const formValues = this.signUpForm.value;
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
    if (StringUtils.isEmptyOrWhiteSpace(email && name)) {
      MessageServiceUtils.showError(
        this._messageService,
        'Необходимо ввести почту.',
      );
      return;
    }
    if (StringUtils.isEmptyOrWhiteSpace(name)) {
      MessageServiceUtils.showError(
        this._messageService,
        'Необходимо ввести псевдоним.',
      );
      return;
    }
    this.register(email, password, name);
  }

  private register(email: string, password: string, name: string): void {
    this._usersService
      .upRoot(email, password, name)
      .pipe(takeUntilDestroyed(this._destoryRef))
      .subscribe({
        next: (_: any): void => {
          MessageServiceUtils.showSuccess(
            this._messageService,
            'Регистрация успешна.',
          );
          this.signUpForm.reset();
          this._router.navigate(['']);
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }
}
