import {
  Component,
  DestroyRef,
  EventEmitter,
  inject,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { ReadUserResponse } from '../../types/ReadUserResponse';
import { ReadRoleResponse } from '../../types/ReadRoleResponse';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Select, SelectChangeEvent } from 'primeng/select';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { UsersService } from '../../../sign-in-page/services/UsersService';
import { UpdateUserDetails } from '../../../sign-in-page/types/UpdateUserDetails';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { PreviousUserDetails } from '../../../sign-in-page/types/PreviousUserDetails';
import { UpdateUserRequest } from '../../../sign-in-page/types/UpdateUserRequest';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UpdateUserProfileResult } from '../../../sign-in-page/types/UpdateUserProfileResult';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-edit-user-dialog',
  imports: [
    Button,
    Dialog,
    FormsModule,
    InputText,
    Select,
    Toast,
    ReactiveFormsModule,
  ],
  templateUrl: './edit-user-dialog.component.html',
  styleUrl: './edit-user-dialog.component.scss',
  providers: [MessageService],
})
export class EditUserDialogComponent {
  @Output() userEditApplied: EventEmitter<ReadUserResponse | null>;
  @Output() userUpdated: EventEmitter<UpdateUserProfileResult>;
  @Input({ required: true }) set visible_setter(value: boolean) {
    this._visible.set(value);
  }
  @Input({ required: true }) set edit_user_setter(value: ReadUserResponse) {
    this._user.set(value);
  }
  @Input({ required: true }) set roles_setter(value: ReadRoleResponse[]) {
    this._availableRoles.set(value);
  }
  public readonly form: FormGroup = new FormGroup({
    email: new FormControl(''),
    passwordCheck: new FormControl(false),
    name: new FormControl(''),
    role: new FormControl(''),
  });
  private readonly _selectedRole: WritableSignal<ReadRoleResponse | null>;
  private readonly _availableRoles: WritableSignal<ReadRoleResponse[]>;
  private readonly _user: WritableSignal<ReadUserResponse>;
  private readonly _newPasswordGenerated: WritableSignal<boolean>;
  private readonly _visible: WritableSignal<boolean>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    private readonly _service: UsersService,
    private readonly _messageService: MessageService,
  ) {
    this.userUpdated = new EventEmitter<UpdateUserProfileResult>();
    this._visible = signal(false);
    this.userEditApplied = new EventEmitter();
    this._availableRoles = signal([]);
    this._selectedRole = signal(null);
    this._newPasswordGenerated = signal(false);
    this._user = signal({
      emailConfirmed: false,
      email: '',
      id: '',
      role: '',
      name: '',
    });
  }
  public cancel(): void {
    this.userEditApplied.emit(null);
  }
  public onRoleChange($event: SelectChangeEvent): void {
    const role = $event.value as ReadRoleResponse;
    this._selectedRole.set(role);
  }
  public get visible(): boolean {
    return this._visible();
  }
  public get email(): string {
    return this._user().email;
  }
  public get name(): string {
    return this._user().name;
  }
  public get role(): string {
    return this._user().role;
  }
  public get roles(): ReadRoleResponse[] {
    return this._availableRoles();
  }
  public onPasswordCheckChange(): void {
    this._newPasswordGenerated.set(
      this.form.get('passwordCheck')?.value || false,
    );
  }

  public submitForm(): void {
    const formValues = this.form.value;
    const email: string = formValues.email as string;
    const name: string = formValues.name as string;
    const password: boolean = formValues.passwordCheck;
    const role: ReadRoleResponse | null = formValues.role;
    const update: UpdateUserDetails = this.formUpdateDetails(
      email,
      name,
      password,
      role,
    );
    const previous: PreviousUserDetails = this.formCurrentUserDetails();
    const request: UpdateUserRequest = {
      updateUserDetails: update,
      previousDetails: previous,
    };
    this.processUserUpdate(request);
  }

  public processUserUpdate(request: UpdateUserRequest): void {
    this._service
      .updateUserProfile(request)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (result: UpdateUserProfileResult): void => {
          const message = `Данные пользователя были обновлены.`;
          MessageServiceUtils.showSuccess(this._messageService, message);
          this.userUpdated.emit(result);
          this.form.reset();
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }

  private formCurrentUserDetails(): PreviousUserDetails {
    const userData: ReadUserResponse = this._user();
    return {
      userEmail: userData.email,
      userId: userData.id,
      userName: userData.name,
      userRole: userData.role,
    };
  }

  private formUpdateDetails(
    email: string,
    name: string,
    password: boolean,
    role: ReadRoleResponse | null,
  ): UpdateUserDetails {
    return {
      isPasswordUpdateRequired: password,
      newUserEmail: StringUtils.isEmptyOrWhiteSpace(email) ? null : email,
      newUserName: StringUtils.isEmptyOrWhiteSpace(name) ? null : name,
      newUserRole: role === null ? null : role.name,
    };
  }
}
