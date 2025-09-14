import { Component, EventEmitter, Output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StringUtils } from '../../../../shared/utils/string-utils';
import { Button } from 'primeng/button';

@Component({
  selector: 'app-spares-search-input',
  imports: [Button, ReactiveFormsModule],
  templateUrl: './spares-search-input.component.html',
  styleUrl: './spares-search-input.component.scss',
})
export class SparesSearchInputComponent {
  @Output() onTextSearchSubmit: EventEmitter<string | null> = new EventEmitter<
    string | null
  >();
  public readonly textSearchForm: FormGroup = new FormGroup({
    text: new FormControl(''),
  });

  public onSubmit(): void {
    this.search();
  }

  public onReset(): void {
    this.textSearchForm.reset();
    this.onTextSearchSubmit.emit(null);
  }

  public search(): void {
    console.log('submitted');
    const formValues = this.textSearchForm.value;
    const input: string = formValues.text;
    if (StringUtils.isEmptyOrWhiteSpace(input)) {
      this.onTextSearchSubmit.emit(null);
      return;
    }
    this.onTextSearchSubmit.emit(input);
  }
}
