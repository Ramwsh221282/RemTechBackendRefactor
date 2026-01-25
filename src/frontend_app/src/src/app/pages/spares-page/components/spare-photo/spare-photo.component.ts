import { Component, input, InputSignal } from '@angular/core';

@Component({
	selector: 'app-spare-photo',
	imports: [],
	templateUrl: './spare-photo.component.html',
	styleUrl: './spare-photo.component.scss',
})
export class SparePhotoComponent {
	photos: InputSignal<string[]> = input(defaultSparePhotos());
}

function defaultSparePhotos(): string[] {
	return [];
}
