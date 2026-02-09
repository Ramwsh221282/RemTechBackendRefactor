import { Component, input, InputSignal } from '@angular/core';
import { DecimalPipe, NgClass } from '@angular/common';

@Component({
	selector: 'app-spare-details',
	imports: [DecimalPipe, NgClass],
	templateUrl: './spare-details.component.html',
	styleUrl: './spare-details.component.scss',
})
export class SpareDetailsComponent {
	location: InputSignal<string> = input('');
	price: InputSignal<number> = input(0);
	isNds: InputSignal<boolean> = input(false);
	oem: InputSignal<string> = input('');
}
