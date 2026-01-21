import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { NgIf, NgOptimizedImage } from '@angular/common';
import { Drawer } from 'primeng/drawer';
import { RouterLink } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { PermissionsStatusService } from '../../../services/PermissionsStatus.service';

@Component({
  selector: 'app-sidebar',
  imports: [ButtonModule, NgOptimizedImage, Drawer, RouterLink, NgIf],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  animations: [],
  providers: [ConfirmationService],
})
export class SidebarComponent {
  @Output() closeClicked: EventEmitter<void> = new EventEmitter();
  @Input({ required: true }) isExpanded: boolean = false;
  readonly permissionsService: PermissionsStatusService = inject(
    PermissionsStatusService,
  );

  public closeClick(): void {
    this.closeClicked.emit();
  }
}
