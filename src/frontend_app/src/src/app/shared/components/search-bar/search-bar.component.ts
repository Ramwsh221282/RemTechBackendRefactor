import { Component, EventEmitter, Input, Output, signal, WritableSignal, } from '@angular/core';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { PanelModule } from 'primeng/panel';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-search-bar',
  imports: [
    ToolbarModule,
    ButtonModule,
    FormsModule,
    PanelModule,
    InputTextModule,
  ],
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.scss',
})
export class SearchBarComponent {
  @Input() placeHolderText = 'Поиск в тексте (в описании, заголовке или характеристиках)';
  @Output() subbmitedSearchTerm: EventEmitter<string> = new EventEmitter();
  @Output() flushedSearchTerm: EventEmitter<string> = new EventEmitter();
  public input: WritableSignal<string> = signal('');

  public submitByButton(event: MouseEvent): void {
    event.stopPropagation();
    if (this.isInputEmpty()) return;
    this.submitSearch();
  }

  public flushTextSearch($event: MouseEvent): void {
    $event.stopPropagation();
    this.input.set('');
    this.submitSearch();
  }

  public submitByEnterKey(event: KeyboardEvent): void {
    event.stopPropagation();
    if (this.isInputEmpty()) return;
    if (event.key === 'Enter') this.submitSearch();
  }

  private isInputEmpty(): boolean {
    return this.input().trim().length === 0;
  }

  private submitSearch(): void {
    const searchTerm = this.trimString(this.input());
    this.subbmitedSearchTerm.emit(searchTerm);
  }

  private trimString(input: string): string {
    return input.trim();
  }
}
