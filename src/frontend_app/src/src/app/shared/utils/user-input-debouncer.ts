import { debounceTime, Subject, Subscription } from 'rxjs';

export class UserInputDebouncer {
	private readonly _debounceSubscription$: Subscription;
	private readonly _debounce$: Subject<void>;
	constructor(
		private readonly delay: number,
		private readonly callback: () => void,
	) {
		this._debounce$ = new Subject<void>();
		this._debounceSubscription$ = this._debounce$.pipe(debounceTime(this.delay)).subscribe(() => this.callback());
	}

	public trigger(): void {
		this._debounce$.next();
	}

	public dispose(): void {
		this._debounceSubscription$.unsubscribe();
	}
}
