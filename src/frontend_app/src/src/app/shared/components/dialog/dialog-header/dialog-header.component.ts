import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-dialog-header',
  imports: [],
  templateUrl: './dialog-header.component.html',
  styleUrl: './dialog-header.component.scss',
})
export class DialogHeaderComponent {
  @Input({ required: true }) header: string = 'header text';
}
