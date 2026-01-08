import {DestroyRef, inject, Injectable, OnInit, signal, WritableSignal} from '@angular/core';
import {IdentityApiService} from '../api/identity-module/identity-api-service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {tap} from 'rxjs';
import {TypedEnvelope} from '../api/envelope';
import {AccountResponse} from '../api/identity-module/identity-responses';

export type UserAccountPermissions = {
  Name: string;
  Description: string;
  Id: string;
}

@Injectable({
  providedIn: 'root'
})
export class PermissionsStatusService implements OnInit {
  private readonly _permissions: WritableSignal<UserAccountPermissions[]> = signal([]);
  private readonly _identityService: IdentityApiService = inject(IdentityApiService);
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);

  initializePermissions(permissions: UserAccountPermissions[]): void {
    this._permissions.set(permissions);
  }

  clean(): void {
    this._permissions.set([]);
  }

  contains(permission: string): boolean {
    return this._permissions().some(p => p.Name === permission);
  }

  ngOnInit(): void {
    this.fetchUserAccountDataForPermissions();
  }

  private fetchUserAccountDataForPermissions(): void {
    this._identityService.fetchAccount()
      .pipe(takeUntilDestroyed(this._destroyRef),
        tap({
          next: (envelope: TypedEnvelope<AccountResponse>): void => {
            if (envelope.body) {
              this.initializePermissions(envelope.body.Permissions);
            }
          }
        })).subscribe()
  }
}
