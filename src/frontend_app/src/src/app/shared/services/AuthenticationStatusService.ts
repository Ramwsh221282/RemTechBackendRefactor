import { Injectable, signal, WritableSignal} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationStatusService {
  readonly isAuthenticated: WritableSignal<boolean> = signal(false);

  unlockRefresh(): void {

  }

  lockRefresh(): void {

  }

  get isAuthorized(): boolean {
    return this.isAuthenticated();
  }

  setIsAuthenticated(value: boolean): void {
    this.isAuthenticated.set(value);
  }

  setIsNotAuthenticated(): void {
    this.isAuthenticated.set(false);
  }

  setTokenRefreshed(): void {

  }

  setTokenIsExpired(): void {

  }
}
