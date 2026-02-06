import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { AppMenuService } from '../app-menu.service';
import { NgOptimizedImage } from '@angular/common';
import { RouterLink } from '@angular/router';
import {AuthenticationStatusService} from '../../../services/AuthenticationStatusService';

@Component({
  selector: 'app-header',
  imports: [ButtonModule, NgOptimizedImage, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private readonly _appMenuService: AppMenuService;

  constructor(
    appMenuService: AppMenuService,
    private readonly _authStatusService: AuthenticationStatusService) {
    this._appMenuService = appMenuService;
  }

  get isAuthorized(): boolean {
    return this._authStatusService.isAuthenticated();
  }

  public onMenuButtonClick = (event: MouseEvent) => {
    event.stopPropagation();
    this._appMenuService.turnSideBarVisibility();
  };
}
