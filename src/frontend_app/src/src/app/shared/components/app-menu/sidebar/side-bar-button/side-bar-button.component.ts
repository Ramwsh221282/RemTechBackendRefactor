import { NgClass, NgIf } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { AnimationsFactory } from '../../../../animations/animations-factory';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-side-bar-button',
  imports: [ButtonModule, NgClass, NgIf, RouterLink],
  templateUrl: './side-bar-button.component.html',
  styleUrl: './side-bar-button.component.scss',
})
export class SideBarButtonComponent {
  @Input({ required: true }) label: string = '';
  @Input({ required: true }) icon: string = '';
  @Input({ required: true }) navigationRoute: string = '';
}
