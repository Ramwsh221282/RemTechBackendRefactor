import { Component, input, InputSignal } from '@angular/core';

@Component({
	selector: 'app-spare-title',
	imports: [],
	templateUrl: './spare-title.component.html',
	styleUrl: './spare-title.component.scss',
})
export class SpareTitleComponent {
	title: InputSignal<string> = input('');
	description: InputSignal<string> = input('');
}
