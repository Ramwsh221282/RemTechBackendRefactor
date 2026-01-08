import {Injectable, OnInit, signal, WritableSignal} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationStatusService {
  readonly isAuthenticated: WritableSignal<boolean> = signal(false);

  setIsAuthenticated(value: boolean): void {
    this.isAuthenticated.set(true);
    console.log(`Is authenticated: ${this.isAuthenticated()}`)
  }

  setIsNotAuthenticated(): void {
    this.isAuthenticated.set(false);
    console.log(`Is authenticated: ${this.isAuthenticated()}`)
  }
}
