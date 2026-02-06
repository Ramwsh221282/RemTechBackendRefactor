import {
  Component,
  DestroyRef,
  effect,
  EventEmitter,
  inject,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { ReadRoleResponse } from '../../types/ReadRoleResponse';
import { UsersService } from '../../../sign-in-page/services/UsersService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Select, SelectChangeEvent } from 'primeng/select';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-role-filter-select',
  imports: [Select, FormsModule],
  templateUrl: './role-filter-select.component.html',
  styleUrl: './role-filter-select.component.scss',
})
export class RoleFilterSelectComponent {
  @Output() rolesFetched: EventEmitter<ReadRoleResponse[]> = new EventEmitter();
  @Output() roleSelected: EventEmitter<ReadRoleResponse | null> =
    new EventEmitter();
  @Input({ required: true }) set role_setter(value: ReadRoleResponse | null) {
    this._currentRole.set(value);
  }
  private readonly _roles: WritableSignal<ReadRoleResponse[]>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  public readonly _currentRole: WritableSignal<ReadRoleResponse | null>;
  constructor(service: UsersService) {
    this._roles = signal([]);
    this._currentRole = signal(null);
    effect(() => {
      service
        .readRoles()
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (val: ReadRoleResponse[]): void => {
            this._roles.set(val);
            this.rolesFetched.emit(val);
          },
        });
    });
  }

  public changeRole($event: SelectChangeEvent): void {
    const role: ReadRoleResponse | null = $event.value;
    this.roleSelected.emit(role);
  }

  public get roles(): ReadRoleResponse[] {
    return this._roles();
  }
}
