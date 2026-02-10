import {Injectable, signal, WritableSignal} from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AppMenuService {
  private readonly sideBarVisibility: WritableSignal<boolean> = signal(false);

  public turnSideBarVisibility(): void {
    this.sideBarVisibility.update((value) => !value);
  }

  public get isSideBarVisible(): boolean {
    return this.sideBarVisibility();
  }
}
