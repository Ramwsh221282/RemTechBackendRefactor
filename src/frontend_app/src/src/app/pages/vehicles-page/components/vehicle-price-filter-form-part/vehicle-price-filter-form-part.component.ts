import {
  Component,
  Directive,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  Output,
  Renderer2,
} from '@angular/core';
import { InputNumber } from 'primeng/inputnumber';
import { DecimalPipe } from '@angular/common';

@Directive({
  selector: '[appFormatPrice]',
})
export class FormatPriceDirective {
  private isFormatting = false;

  constructor(
    private el: ElementRef,
    private renderer: Renderer2,
  ) {}

  @HostListener('input', ['$event'])
  onInput(event: Event) {
    const input = event.target as HTMLInputElement;
    const rawValue = input.value.replace(/\D/g, ''); // Только цифры

    if (this.isFormatting) return; // Защита от рекурсии

    this.isFormatting = true;

    if (rawValue) {
      const formatted = Number(rawValue).toLocaleString('ru-RU'); // 1000 → "1 000"
      this.renderer.setProperty(input, 'value', formatted);
    } else {
      this.renderer.setProperty(input, 'value', '');
    }

    // Отправляем событие изменения значения
    input.dispatchEvent(new Event('change', { bubbles: true }));

    this.isFormatting = false;
  }

  @HostListener('blur', ['$event'])
  onBlur(event: Event) {
    const input = event.target as HTMLInputElement;
    const rawValue = input.value.replace(/\D/g, '');
    if (!rawValue) {
      this.renderer.setProperty(input, 'value', '');
    }
  }

  @HostListener('focus', ['$event'])
  onFocus(event: Event) {
    const input = event.target as HTMLInputElement;
    const rawValue = input.value.replace(/\D/g, '');
    if (rawValue) {
      this.renderer.setProperty(input, 'value', rawValue);
    }
  }
}

export interface PriceSubmitEvent {
  priceFrom: number | null;
  priceTo: number | null;
}

@Component({
  selector: 'app-vehicle-price-filter-form-part',
  imports: [FormatPriceDirective, DecimalPipe],
  templateUrl: './vehicle-price-filter-form-part.component.html',
  styleUrl: './vehicle-price-filter-form-part.component.scss',
})
export class VehiclePriceFilterFormPartComponent {
  @Output() onPriceChangeSubmit: EventEmitter<PriceSubmitEvent> =
    new EventEmitter<PriceSubmitEvent>();
  public priceFrom: number | null = null;
  public priceTo: number | null = null;

  onPriceFromChange(event: Event) {
    const value = (event.target as HTMLInputElement).value.replace(/\D/g, '');
    this.priceFrom = value ? parseInt(value, 10) : null;
  }

  onPriceToChange(event: Event) {
    const value = (event.target as HTMLInputElement).value.replace(/\D/g, '');
    this.priceTo = value ? parseInt(value, 10) : null;
  }

  public submit(): void {
    const $event: PriceSubmitEvent = {
      priceTo: this.priceTo,
      priceFrom: this.priceFrom,
    };
    this.onPriceChangeSubmit.emit($event);
  }

  public onReset(): void {
    this.priceFrom = null;
    this.priceTo = null;
    this.submit();
  }

  protected readonly onsubmit = onsubmit;
}
