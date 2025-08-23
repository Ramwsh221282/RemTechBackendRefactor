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
import { ConfirmationService, MessageService } from 'primeng/api';
import { UsersService } from '../../../sign-in-page/services/UsersService';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { Toast } from 'primeng/toast';
import { ConfirmDialog } from 'primeng/confirmdialog';

@Component({
  selector: 'app-user-card',
  imports: [Toast, ConfirmDialog],
  templateUrl: './user-card.component.html',
  styleUrl: './user-card.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class UserCardComponent {
  @Output() userRemoved: EventEmitter<ReadUserResponse>;
  @Output() editUserClick: EventEmitter<ReadUserResponse>;
  @Output() deleteUserClick: EventEmitter<ReadUserResponse>;
  @Input({ required: true }) set user_setter(value: ReadUserResponse) {
    this._user.set(value);
  }

  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _user: WritableSignal<ReadUserResponse>;

  constructor(
    private readonly service: UsersService,
    private readonly _messageService: MessageService,
    private readonly _confirmationService: ConfirmationService,
  ) {
    this.userRemoved = new EventEmitter<ReadUserResponse>();
    this.editUserClick = new EventEmitter<ReadUserResponse>();
    this.deleteUserClick = new EventEmitter<ReadUserResponse>();
    this._user = signal({
      emailConfirmed: false,
      name: '',
      id: '',
      email: '',
      role: '',
    });
  }

  public confirmUserDelete($event: Event): void {
    this._confirmationService.confirm({
      target: $event.target as EventTarget,
      message: 'Удалить пользователя?',
      header: 'Подтверждение',
      closable: true,
      closeOnEscape: true,
      icon: 'pi pi-exclamation-triangle',
      rejectButtonProps: {
        label: 'Отмена',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Удалить',
      },
      accept: () => {
        this.processUserDeletion(this._user());
      },
      reject: () => {},
    });
  }

  private processUserDeletion(user: ReadUserResponse): void {
    this.service
      .removeUserProfile(user.id)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (_: any): void => {
          const message = 'Пользователь был удален.';
          MessageServiceUtils.showSuccess(this._messageService, message);
          this.userRemoved.emit(user);
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }

  public onEditClick(): void {
    this.editUserClick.emit(this._user());
  }

  public get name(): string {
    return this._user().name;
  }

  public get email(): string {
    return this._user().email;
  }

  public get role(): string {
    return this._user().role;
  }

  public get emailConfirmed(): boolean {
    return this._user().emailConfirmed;
  }
}
