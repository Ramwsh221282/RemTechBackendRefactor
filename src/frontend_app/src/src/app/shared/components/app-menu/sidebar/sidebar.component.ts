import {
  Component,
  effect,
  EventEmitter,
  inject,
  input,
  Input,
  InputSignal,
  Output, signal,
  WritableSignal
} from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { NgOptimizedImage } from '@angular/common';
import { Drawer } from 'primeng/drawer';
import { RouterLink } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { PermissionsStatusService } from '../../../services/PermissionsStatus.service';

@Component({
  selector: 'app-sidebar',
  imports: [ButtonModule, NgOptimizedImage, Drawer, RouterLink],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  animations: [],
  providers: [ConfirmationService],
})
export class SidebarComponent {
  @Output() closeClicked: EventEmitter<void> = new EventEmitter<void>();

  isExpanded: InputSignal<boolean> = input(false);

  readonly permissionsService: PermissionsStatusService = inject(
    PermissionsStatusService,
  );

  public onHide(): void {
    this.closeClicked.emit();
  }

  public closeClick(): void {
    this.closeClicked.emit();
  }
}
