import { Component, DestroyRef, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { PasswordRulesComponent } from '../../../../shared/components/password-rules/password-rules.component';
import { IdentityApiService } from '../../../../shared/api/identity-module/identity-api-service';
import { catchError, EMPTY, Observable } from 'rxjs';

@Component({
	selector: 'app-sign-up-form',
	imports: [ReactiveFormsModule, Toast, PasswordRulesComponent],
	templateUrl: './sign-up-form.component.html',
	styleUrl: './sign-up-form.component.scss',
})
export class SignUpFormComponent {
	constructor(
		private readonly _messageService: MessageService,
		private readonly _identityService: IdentityApiService,
	) {}

	private readonly _destoryRef: DestroyRef = inject(DestroyRef);

	signUpForm: FormGroup = new FormGroup({
		email: new FormControl(''),
		name: new FormControl(''),
		password: new FormControl(''),
	});

	public submit(): void {
		const email: string = this.readEmail();
		const login: string = this.readLogin();
		const password: string = this.readPassword();
		if (this.isEmailEmpty(email)) return;
		if (this.isLoginEmpty(login)) return;
		if (this.isPasswordEmpty(password)) return;
		this.register(email, login, password);
	}

	private isEmailEmpty(email: string): boolean {
		const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(email);
		if (isEmpty) {
			MessageServiceUtils.showError(this._messageService, 'Необходимо ввести почту.');
		}
		return isEmpty;
	}

	private isLoginEmpty(login: string): boolean {
		const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(login);
		if (isEmpty) {
			MessageServiceUtils.showError(this._messageService, 'Необходимо ввести псевдоним.');
		}
		return isEmpty;
	}

	private isPasswordEmpty(password: string): boolean {
		const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(password);
		if (isEmpty) {
			MessageServiceUtils.showError(this._messageService, 'Необходимо ввести пароль.');
		}
		return isEmpty;
	}

	private readEmail(): string {
		const formValues = this.signUpForm.value;
		return formValues.email;
	}

	private readLogin(): string {
		const formValues = this.signUpForm.value;
		return formValues.name;
	}

	private readPassword(): string {
		const formValues = this.signUpForm.value;
		return formValues.password;
	}

	private register(email: string, login: string, password: string): void {
		this._identityService
			.signUp(email, login, password)
			.pipe(
				takeUntilDestroyed(this._destoryRef),
				catchError((error: HttpErrorResponse): Observable<never> => {
					const message: string = error.error.message;
					MessageServiceUtils.showError(this._messageService, message);
					return EMPTY;
				}),
			)
			.subscribe(() => {
				const message: string = 'Подтверждение регистрации отправлено на Вашу почту.';
				MessageServiceUtils.showSuccess(this._messageService, message);
			});
	}
}
