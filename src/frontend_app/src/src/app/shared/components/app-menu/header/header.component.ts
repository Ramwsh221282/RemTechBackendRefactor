import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { AppMenuService } from '../app-menu.service';
import { NgIf, NgOptimizedImage } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [ButtonModule, NgOptimizedImage],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private readonly _appMenuService: AppMenuService;

  constructor(appMenuService: AppMenuService) {
    this._appMenuService = appMenuService;
  }

  public onMenuButtonClick = (event: MouseEvent) => {
    event.stopPropagation();
    this._appMenuService.turnSideBarVisibility();
  };

  public get isAuthorized(): boolean {
    return true;
  }
}
