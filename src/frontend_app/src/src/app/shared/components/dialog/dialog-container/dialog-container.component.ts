import { Component, Input } from '@angular/core';
import { DialogModule } from 'primeng/dialog';

@Component({
  selector: 'app-dialog-container',
  imports: [DialogModule],
  templateUrl: './dialog-container.component.html',
  styleUrl: './dialog-container.component.scss',
})
export class DialogContainerComponent {
  @Input({ required: true }) visible: boolean = false;
}
