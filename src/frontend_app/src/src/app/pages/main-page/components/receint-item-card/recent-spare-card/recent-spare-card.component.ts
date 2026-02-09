import { Component, Input, signal, WritableSignal } from '@angular/core';
import { SpareResponse } from '../../../../../shared/api/spares-module/spares-api.responses';
import { DefaultSpareResponse } from '../../../../../shared/api/spares-module/spares-api.factories';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-recent-spare-card',
  templateUrl: './recent-spare-card.component.html',
  styleUrls: ['./recent-spare-card.component.css'],
  imports: [DecimalPipe],
})
export class RecentSpareCardComponent {
  constructor() {
    this.spare = signal(DefaultSpareResponse());
  }

  readonly spare: WritableSignal<SpareResponse>;
  @Input({ required: true }) set spare_setter(value: SpareResponse) {
    this.spare.set(value);
  }
}
