import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { MessageService } from 'primeng/api';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Button } from 'primeng/button';
import { StringUtils } from '../../../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { Toast } from 'primeng/toast';
import { ParserLinkResponse, ParserResponse } from '../../../../../../shared/api/parsers-module/parsers-responses';
import { DefaultParserLinkResponse, DefaultParserResponse } from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-edit-link-dialog',
  imports: [Dialog, ReactiveFormsModule, InputText, Button, FormsModule, Toast],
  templateUrl: './scraper-edit-link-dialog.component.html',
  styleUrl: './scraper-edit-link-dialog.component.scss',
})
export class ScraperEditLinkDialogComponent {
  constructor(private readonly _messageService: MessageService) {
    this.scraper = signal(DefaultParserResponse());
    this.scraperLink = signal(DefaultParserLinkResponse());
    this.onSave = new EventEmitter();
    this.onDialogClose = new EventEmitter();
  }

  @Output() onSave: EventEmitter<{ linkId: string, name: string | null, url: string | null }>;
  @Output() onDialogClose: EventEmitter<boolean>;
  @Input({ required: true }) set scraper_setter(scraper: ParserResponse) {
    this.scraper.set(scraper);
  }
  @Input({ required: true }) set scraper_link_setter(scraperLink: ParserLinkResponse) {
    this.scraperLink.set(scraperLink);
  }
  @Input({ required: true }) set visibility_setter(visibility: boolean) {
    this.visibility = visibility;
  }

  visibility: boolean = false;
  readonly scraper: WritableSignal<ParserResponse>;
  readonly scraperLink: WritableSignal<ParserLinkResponse>;
  editScraperLinkForm: FormGroup = new FormGroup({
    name: new FormControl(''),
    url: new FormControl(''),
  });

  public onSubmit(): void {
    const name: string = this.readNameFromForm();
    const url: string = this.readUrlFromForm();
    this.editScraperLink(name, url);
  }

  private editScraperLink(name: string, url: string): void {
    if (this.allPropertiesEmpty(name, url)) return;
    const editedName: string | null = StringUtils.isEmptyOrWhiteSpace(name) ? null : name;
    const editedUrl: string | null = StringUtils.isEmptyOrWhiteSpace(url) ? null : url;
    const id: string = this.scraperLink().Id;
    this.onSave.emit({ linkId: id, name: editedName, url: editedUrl });
    this.resetForm();
    this.onHide();
  }

  get header(): string {
    return `Редактирование ссылки ${this.scraperLink().UrlName}`;
  }

  public onHide(): void {
    this.onDialogClose.emit(false);
  }

  private resetForm(): void {
    this.editScraperLinkForm.reset();
  }

  private allPropertiesEmpty(name: string, url: string): boolean {
    const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(name) && StringUtils.isEmptyOrWhiteSpace(url);
    if (isEmpty) {
      MessageServiceUtils.showError(
        this._messageService,
        `Оба поля были пустыми.`
      )
      return true;
    }
    return false;
  }

  private readNameFromForm(): string {
    const formValues = this.editScraperLinkForm.value;
    return formValues.name as string;
  }

  private readUrlFromForm(): string {
    const formValues = this.editScraperLinkForm.value;
    return formValues.url as string;
  }
}
