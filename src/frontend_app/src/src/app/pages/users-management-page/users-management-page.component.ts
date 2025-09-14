import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { ReadUserResponse } from './types/ReadUserResponse';
import { UsersService } from '../sign-in-page/services/UsersService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EmailFilterInputComponent } from './components/email-filter-input/email-filter-input.component';
import { NameFilterInputComponent } from './components/name-filter-input/name-filter-input.component';
import { RoleFilterSelectComponent } from './components/role-filter-select/role-filter-select.component';
import { ReadRoleResponse } from './types/ReadRoleResponse';
import { AddUserDialogComponent } from './components/add-user-dialog/add-user-dialog.component';
import { UserCardComponent } from './components/user-card/user-card.component';
import { AddUserButtonComponent } from './components/add-user-button/add-user-button.component';
import { EditUserDialogComponent } from './components/edit-user-dialog/edit-user-dialog.component';
import { NgIf } from '@angular/common';
import { UpdateUserProfileResult } from '../sign-in-page/types/UpdateUserProfileResult';
import { ConfirmationService, MessageService } from 'primeng/api';
import { MessageServiceUtils } from '../../shared/utils/message-service-utils';
import { HttpErrorResponse } from '@angular/common/http';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-users-management-page',
  imports: [
    EmailFilterInputComponent,
    NameFilterInputComponent,
    RoleFilterSelectComponent,
    AddUserDialogComponent,
    UserCardComponent,
    AddUserButtonComponent,
    EditUserDialogComponent,
    NgIf,
    PaginationComponent,
  ],
  templateUrl: './users-management-page.component.html',
  styleUrl: './users-management-page.component.scss',
  providers: [ConfirmationService, MessageService],
})
export class UsersManagementPageComponent {
  public readonly _nameFilter: WritableSignal<string | null>;
  public readonly _roleFilter: WritableSignal<ReadRoleResponse | null>;
  public readonly _emailFilter: WritableSignal<string | null>;
  private readonly _page: WritableSignal<number>;
  private readonly _isAddingUser: WritableSignal<boolean>;
  private readonly _availableRoles: WritableSignal<ReadRoleResponse[]>;
  private readonly _userToEdit: WritableSignal<ReadUserResponse | null>;
  public readonly _users: WritableSignal<ReadUserResponse[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _totalCount: WritableSignal<number>;
  constructor(private readonly service: UsersService) {
    this._totalCount = signal(0);
    this._userToEdit = signal(null);
    this._availableRoles = signal([]);
    this._isAddingUser = signal(false);
    this._nameFilter = signal(null);
    this._roleFilter = signal(null);
    this._emailFilter = signal(null);
    this._page = signal(1);
    this._users = signal([]);
    effect(() => {
      const page: number = this._page();
      const nameFilter: string | null = this._nameFilter();
      const roleFilter: ReadRoleResponse | null = this._roleFilter();
      const emailFilter: string | null = this._emailFilter();
      service
        .readUsers(
          page,
          nameFilter,
          emailFilter,
          roleFilter ? roleFilter.name : null,
        )
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (users: ReadUserResponse[]): void => {
            this._users.set(users);
          },
        });
    });
    effect(() => {
      const nameFilter: string | null = this._nameFilter();
      const roleFilter: ReadRoleResponse | null = this._roleFilter();
      const emailFilter: string | null = this._emailFilter();
      service
        .readUsersCount(
          nameFilter,
          emailFilter,
          roleFilter ? roleFilter.name : null,
        )
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (count: number): void => {
            this._totalCount.set(count);
          },
        });
    });
  }

  public get totalCount(): number {
    return this._totalCount();
  }

  public changeUserToEdit(user: ReadUserResponse | null): void {
    this._userToEdit.set(user);
  }

  public acceptRoles(roles: ReadRoleResponse[]): void {
    this._availableRoles.set(roles);
  }

  public get availableRoles(): ReadRoleResponse[] {
    return this._availableRoles();
  }

  public handleAddUserClick(): void {
    this._isAddingUser.set(true);
  }

  public handleAddUserFormClose(): void {
    this._isAddingUser.set(false);
  }

  public get isAddingUser(): boolean {
    return this._isAddingUser();
  }

  public get currentEmailFilter(): string | null {
    return this._emailFilter();
  }

  public get currentNameFilter(): string | null {
    return this._nameFilter();
  }

  public get userToEdit(): ReadUserResponse | null {
    return this._userToEdit();
  }

  public get currentRoleFilter(): ReadRoleResponse | null {
    return this._roleFilter();
  }

  public changeNameFilter(name: string | null): void {
    this._nameFilter.set(name);
  }

  public handleUserRemoved(user: ReadUserResponse): void {
    const users: ReadUserResponse[] = this._users();
    const filtered = users.filter((u) => u.id !== user.id);
    this._users.set(filtered);
  }

  public handleUserUpdated(user: UpdateUserProfileResult): void {
    const users: ReadUserResponse[] = this._users();
    const userIndex = users.findIndex((u) => u.id === user.userId);
    if (userIndex === -1) return;
    users[userIndex] = {
      ...users[userIndex],
      email: user.userEmail,
      name: user.userName,
      role: user.userRole,
    };
    this._users.set(users);
  }

  public changeEmailFilter(name: string | null): void {
    this._emailFilter.set(name);
  }

  public changeRoleFilter(role: ReadRoleResponse | null): void {
    this._roleFilter.set(role);
  }

  public get currentPage(): number {
    return this._page();
  }

  public changePage(page: number): void {
    this._page.set(page);
  }
}
