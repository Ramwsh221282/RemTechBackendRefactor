import {
  Component,
  DestroyRef,
  effect,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { NgIf, NgOptimizedImage } from '@angular/common';
import { Drawer } from 'primeng/drawer';
import { RouterLink } from '@angular/router';
import { UsersService } from '../../../../pages/sign-in-page/services/UsersService';
import { CookieService } from 'ngx-cookie-service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { defer, Observable } from 'rxjs';
import { TokensService } from '../../../services/TokensService';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { UserInfoService } from '../../../services/UserInfoService';
import { UserInfo } from '../../../../pages/sign-in-page/types/UserInfo';
import { StringUtils } from '../../../utils/string-utils';
import {PermissionsStatusService} from '../../../services/PermissionsStatus.service';

@Component({
  selector: 'app-sidebar',
  imports: [
    ButtonModule,
    NgOptimizedImage,
    Drawer,
    RouterLink,
    NgIf,
    ConfirmDialog,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  animations: [],
  providers: [ConfirmationService],
})
export class SidebarComponent {
  @Output() closeClicked: EventEmitter<void> = new EventEmitter();
  @Input({ required: true }) isExpanded: boolean = false;
  readonly permissionsService: PermissionsStatusService = inject(PermissionsStatusService);

  public closeClick(): void {
    this.closeClicked.emit();
  }
}
