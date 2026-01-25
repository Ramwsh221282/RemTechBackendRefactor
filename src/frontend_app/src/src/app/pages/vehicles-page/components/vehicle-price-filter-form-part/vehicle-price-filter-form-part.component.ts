import { Component, Directive, EventEmitter, HostListener, OnDestroy, Output, Renderer2 } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { UserInputDebouncer } from '../../../../shared/utils/user-input-debouncer';

@Directive({
	selector: '[appFormatPrice]',
})
export class FormatPriceDirective {
	private isFormatting: boolean = false;

	constructor(private renderer: Renderer2) {}

	@HostListener('input', ['$event'])
	onInput(event: Event): void {
		const input: HTMLInputElement = event.target as HTMLInputElement;
		const rawValue: string = input.value.replace(/\D/g, ''); // Только цифры

		if (this.isFormatting) return; // Защита от рекурсии

		this.isFormatting = true;

		if (rawValue) {
			const formatted: string = Number(rawValue).toLocaleString('ru-RU'); // 1000 → "1 000"
			this.renderer.setProperty(input, 'value', formatted);
		} else {
			this.renderer.setProperty(input, 'value', '');
		}

		// Отправляем событие изменения значения
		input.dispatchEvent(new Event('change', { bubbles: true }));

		this.isFormatting = false;
	}

	@HostListener('blur', ['$event'])
	onBlur(event: Event): void {
		const input: HTMLInputElement = event.target as HTMLInputElement;
		const rawValue: string = input.value.replace(/\D/g, '');
		if (!rawValue) {
			this.renderer.setProperty(input, 'value', '');
		}
	}

	@HostListener('focus', ['$event'])
	onFocus(event: Event): void {
		const input: HTMLInputElement = event.target as HTMLInputElement;
		const rawValue: string = input.value.replace(/\D/g, '');
		if (rawValue) {
			this.renderer.setProperty(input, 'value', rawValue);
		}
	}
}

export interface PriceChangeEvent {
	minimalPrice: number | null;
	maximalPrice: number | null;
}

@Component({
	selector: 'app-vehicle-price-filter-form-part',
	imports: [FormatPriceDirective, DecimalPipe],
	templateUrl: './vehicle-price-filter-form-part.component.html',
	styleUrl: './vehicle-price-filter-form-part.component.scss',
})
export class VehiclePriceFilterFormPartComponent implements OnDestroy {
	@Output() priceChanged: EventEmitter<PriceChangeEvent> = new EventEmitter<PriceChangeEvent>();
	public priceFrom: number | null = null;
	public priceTo: number | null = null;
	private priceChangeDebounce$: UserInputDebouncer = new UserInputDebouncer(1000, this.submit.bind(this));

	onPriceFromChange(event: Event): void {
		const value: string = (event.target as HTMLInputElement).value.replace(/\D/g, '');
		this.priceFrom = value ? parseInt(value, 10) : null;
		this.priceChangeDebounce$.trigger();
	}

	onPriceToChange(event: Event): void {
		const value: string = (event.target as HTMLInputElement).value.replace(/\D/g, '');
		this.priceTo = value ? parseInt(value, 10) : null;
		this.priceChangeDebounce$.trigger();
	}

	public submit(): void {
		const $event: PriceChangeEvent = {
			maximalPrice: this.priceTo,
			minimalPrice: this.priceFrom,
		};
		this.priceChanged.emit($event);
	}

	public ngOnDestroy(): void {
		this.priceChangeDebounce$.dispose();
	}

	public onReset(): void {
		this.priceFrom = null;
		this.priceTo = null;
		this.submit();
	}

	protected readonly onsubmit: ((this: Window, ev: SubmitEvent) => any) | null = onsubmit;
}
