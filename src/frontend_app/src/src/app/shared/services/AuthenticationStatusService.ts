import { Injectable, signal, WritableSignal } from '@angular/core';

@Injectable({
	providedIn: 'root',
})
export class AuthenticationStatusService {
	readonly isAuthenticated: WritableSignal<boolean> = signal(false);
	readonly userId: WritableSignal<string | null> = signal(null);

	get isAuthorized(): boolean {
		return this.isAuthenticated();
	}

	setUserId(userId: string): void {
		this.userId.set(userId);
	}

	setIsAuthenticated(value: boolean): void {
		this.isAuthenticated.set(value);
	}

	setIsNotAuthenticated(): void {
		this.userId.set(null);
		this.isAuthenticated.set(false);
	}
}
