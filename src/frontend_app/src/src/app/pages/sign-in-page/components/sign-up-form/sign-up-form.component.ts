import { Component, DestroyRef, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { UsersService } from '../../services/UsersService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-sign-up-form',
  imports: [ReactiveFormsModule, Toast],
  templateUrl: './sign-up-form.component.html',
  styleUrl: './sign-up-form.component.scss',
  providers: [MessageService],
})
export class SignUpFormComponent {
  signUpForm: FormGroup = new FormGroup({
    email: new FormControl(''),
    name: new FormControl(''),
    password: new FormControl(''),
  });

  private readonly _messageService: MessageService;
  private readonly _usersService: UsersService;
  private readonly _destoryRef: DestroyRef = inject(DestroyRef);
  constructor(messageService: MessageService, usersService: UsersService) {
    this._messageService = messageService;
    this._usersService = usersService;
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
      .register(email, password, name)
      .pipe(takeUntilDestroyed(this._destoryRef))
      .subscribe({
        next: (_: any): void => {
          MessageServiceUtils.showSuccess(
            this._messageService,
            'Регистрация успешна.',
          );
          this.signUpForm.reset();
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }
}
