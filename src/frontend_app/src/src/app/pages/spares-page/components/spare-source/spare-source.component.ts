import { Component, input, InputSignal } from '@angular/core';

@Component({
	selector: 'app-spare-source',
	imports: [],
	templateUrl: './spare-source.component.html',
	styleUrl: './spare-source.component.scss',
})
export class SpareSourceComponent {
	source: InputSignal<string> = input('');
}
