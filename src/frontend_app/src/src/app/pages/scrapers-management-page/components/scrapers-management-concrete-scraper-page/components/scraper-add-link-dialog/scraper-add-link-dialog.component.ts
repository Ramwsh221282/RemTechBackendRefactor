import {
  Component,
  DestroyRef,
  EventEmitter,
  inject,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { StringUtils } from '../../../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Button } from 'primeng/button';
import { Toast } from 'primeng/toast';
import {
  ParserLinkResponse,
  ParserResponse,
} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {
  DefaultParserResponse,
  NewParserLink,
} from '../../../../../../shared/api/parsers-module/parsers-factory';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-scraper-add-link-dialog',
  imports: [Dialog, ReactiveFormsModule, InputText, Button, Toast, TableModule],
  templateUrl: './scraper-add-link-dialog.component.html',
  styleUrl: './scraper-add-link-dialog.component.scss',
  providers: [MessageService],
})
export class ScraperAddLinkDialogComponent {
  constructor(private readonly _messageService: MessageService) {
    this.scraper = signal(DefaultParserResponse());
    this.oldScraperLinks = signal([]);
    this.newScraperLinks = signal([]);
    this.visibility = false;
    this.linksAdded = new EventEmitter<ParserLinkResponse[]>();
    this.visibilityChange = new EventEmitter<boolean>();
  }

  visibility: boolean;
  readonly oldScraperLinks: WritableSignal<ParserLinkResponse[]>;
  readonly newScraperLinks: WritableSignal<ParserLinkResponse[]>;
  readonly scraper: WritableSignal<ParserResponse>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  addLinkForm: FormGroup = new FormGroup({
    name: new FormControl(''),
    url: new FormControl(''),
  });

  @Output() linksAdded: EventEmitter<ParserLinkResponse[]>;
  @Output() visibilityChange: EventEmitter<boolean>;
  @Input({ required: true }) set visbility_setter(value: boolean) {
    this.visibility = value;
  }
  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this.scraper.set(value);
    this.oldScraperLinks.set(value.Links);
  }

  get linksList(): ParserLinkResponse[] {
    return [...this.newScraperLinks(), ...this.oldScraperLinks()];
  }

  public submitAdd(): void {
    const name: string = this.readNameFromForm();
    const url: string = this.readUrlFromForm();
    if (this.linkHasEmptyName(name)) return;
    if (this.linkHasEmptyUrl(url)) return;
    if (this.linkIsDuplicated(name, url)) return;
    this.appendLinkToList(name, url);
    this.resetFormInputs();
  }

  public confirmAdd(): void {
    if (this.newLinksAreEmpty()) return;
    const links: ParserLinkResponse[] = this.newScraperLinks();
    this.newScraperLinks.set([]);
    this.linksAdded.emit(links);
  }

  public onHide(): void {
    this.visibilityChange.emit(false);
  }

  private appendLinkToList(name: string, url: string): void {
    const link: ParserLinkResponse = NewParserLink(name, url);
    this.newScraperLinks.update(
      (links: ParserLinkResponse[]): ParserLinkResponse[] => [...links, link],
    );
  }

  private newLinksAreEmpty(): boolean {
    const links: ParserLinkResponse[] = this.newScraperLinks();
    const empty: boolean = links.length === 0;
    if (empty)
      MessageServiceUtils.showError(
        this._messageService,
        `Нет добавленных новых ссылок.`,
      );
    return empty;
  }

  private linkHasEmptyUrl(url: string): boolean {
    const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(url);
    if (isEmpty) {
      MessageServiceUtils.showError(
        this._messageService,
        `Название ссылки было пустым.`,
      );
      return true;
    }
    return false;
  }

  private linkHasEmptyName(name: string): boolean {
    const isEmpty: boolean = StringUtils.isEmptyOrWhiteSpace(name);
    if (isEmpty) {
      MessageServiceUtils.showError(
        this._messageService,
        `URL ссылки был пустым.`,
      );
      return true;
    }
    return false;
  }

  private linkIsDuplicated(name: string, url: string): boolean {
    const links: ParserLinkResponse[] = this.linksList;
    if (this.isDuplicatedByName(name, links)) {
      MessageServiceUtils.showError(
        this._messageService,
        `Ссылка с названием ${name} уже существует.`,
      );
      return true;
    }

    if (this.isDuplicatedByUrl(url, links)) {
      MessageServiceUtils.showError(
        this._messageService,
        `Ссылка с URL ${url} уже существует.`,
      );
      return true;
    }

    return false;
  }

  private readNameFromForm(): string {
    const formValues = this.addLinkForm.value;
    return formValues.name as string;
  }

  private readUrlFromForm(): string {
    const formValues = this.addLinkForm.value;
    return formValues.url as string;
  }

  private resetFormInputs(): void {
    this.addLinkForm.reset();
  }

  private isDuplicatedByName(
    name: string,
    links: ParserLinkResponse[],
  ): boolean {
    return links.some((link) => link.UrlName === name);
  }

  private isDuplicatedByUrl(url: string, links: ParserLinkResponse[]): boolean {
    return links.some((link) => link.UrlValue === url);
  }
}
