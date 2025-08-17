import { Component, EventEmitter, Output } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { StringUtils } from '../../../../shared/utils/string-utils';

@Component({
  selector: 'app-vehicles-text-search',
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './vehicles-text-search.component.html',
  styleUrl: './vehicles-text-search.component.scss',
})
export class VehiclesTextSearchComponent {
  @Output() textSearchSubmit: EventEmitter<string | undefined> =
    new EventEmitter();
  public readonly textSearchForm: FormGroup = new FormGroup({
    text: new FormControl(''),
  });

  public onSubmit(): void {
    const formValues = this.textSearchForm.value;
    const text: string = formValues.text as string;
    if (StringUtils.isEmptyOrWhiteSpace(text)) {
      this.submitTextSearch('');
      return;
    }
    this.submitTextSearch(text);
  }

  public onReset(): void {
    this.textSearchForm.reset();
    this.submitTextSearch('');
  }

  private submitTextSearch(text: string): void {
    if (text === '') {
      this.textSearchSubmit.emit(undefined);
    }
    this.textSearchSubmit.emit(text);
  }
}
