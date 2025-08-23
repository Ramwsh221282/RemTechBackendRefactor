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
import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { InputText } from 'primeng/inputtext';
import { Toast } from 'primeng/toast';
import { ReadRoleResponse } from '../../types/ReadRoleResponse';
import { MessageService } from 'primeng/api';
import { Select, SelectChangeEvent } from 'primeng/select';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { UsersService } from '../../../sign-in-page/services/UsersService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReadUserResponse } from '../../types/ReadUserResponse';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-add-user-dialog',
  imports: [
    Button,
    Dialog,
    FormsModule,
    InputText,
    Toast,
    ReactiveFormsModule,
    Select,
  ],
  templateUrl: './add-user-dialog.component.html',
  styleUrl: './add-user-dialog.component.scss',
  providers: [MessageService],
})
export class AddUserDialogComponent {
  @Output() onClose: EventEmitter<void> = new EventEmitter();
  @Output() userAdded: EventEmitter<ReadUserResponse> = new EventEmitter();
  @Input({ required: true }) set visible_setter(value: boolean) {
    this._isVisible.set(value);
  }

  @Input({ required: true }) set roles(value: ReadRoleResponse[]) {
    this._roles.set(value);
  }

  public readonly form: FormGroup = new FormGroup({
    email: new FormControl(''),
    name: new FormControl(''),
    role: new FormControl(''),
  });

  private readonly _isVisible: WritableSignal<boolean>;
  private readonly _roles: WritableSignal<ReadRoleResponse[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  public readonly currentRole: WritableSignal<ReadRoleResponse | null>;

  constructor(
    private readonly messageService: MessageService,
    private readonly usersService: UsersService,
  ) {
    this._isVisible = signal(false);
    this._roles = signal([]);
    this.currentRole = signal(null);
  }

  public onRoleChange(role: SelectChangeEvent): void {
    const selectedRole: ReadRoleResponse | null = role.value;
    this.currentRole.set(selectedRole);
  }

  public get roles(): ReadRoleResponse[] {
    return this._roles();
  }

  public submitForm(): void {
    const formValues = this.form.value;
    const email = formValues.email;
    const name = formValues.name;
    const role: ReadRoleResponse | null =
      formValues.role as ReadRoleResponse | null;
    if (StringUtils.isEmptyOrWhiteSpace(email)) {
      MessageServiceUtils.showError(
        this.messageService,
        `Необходимо указать почту.`,
      );
      return;
    }
    if (StringUtils.isEmptyOrWhiteSpace(name)) {
      MessageServiceUtils.showError(
        this.messageService,
        `Необходимо указать псевдоним.`,
      );
      return;
    }
    if (!role) {
      MessageServiceUtils.showError(
        this.messageService,
        `Необходимо указать роль.`,
      );
      return;
    }
    this.processCreateUser(email, name, role.name);
  }

  public processCreateUser(email: string, name: string, role: string): void {
    this.usersService
      .createUserByAdmin(email, name, role)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (user: ReadUserResponse): void => {
          const message = `Добавлен пользователь ${user.email} ${user.name} ${user.role}`;
          MessageServiceUtils.showSuccess(this.messageService, message);
          this.userAdded.emit(user);
          this.currentRole.set(null);
          this.form.reset();
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this.messageService, message);
          this.currentRole.set(null);
          this.form.reset();
        },
      });
  }

  public cancel(): void {
    this.form.reset();
    this.onClose.emit();
  }

  public get visible(): boolean {
    return this._isVisible();
  }
}
