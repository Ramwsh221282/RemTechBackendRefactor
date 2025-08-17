import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AppMenuService {
  private readonly sideBarVisibility = signal(false);

  public turnSideBarVisibility(): void {
    this.sideBarVisibility.update((value) => !value);
  }

  public get isSideBarVisible(): boolean {
    return this.sideBarVisibility();
  }
}
