import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-add-user-button',
  imports: [],
  templateUrl: './add-user-button.component.html',
  styleUrl: './add-user-button.component.scss',
})
export class AddUserButtonComponent {
  @Output() userAddClicked: EventEmitter<void> = new EventEmitter();

  public handleAddUserClick(): void {
    this.userAddClicked.emit();
  }
}
