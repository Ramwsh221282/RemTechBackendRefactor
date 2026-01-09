import {Component, EventEmitter, Input, Output, signal, WritableSignal,} from '@angular/core';
import { NgClass, NgForOf, NgIf } from '@angular/common';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Toast } from 'primeng/toast';
import { ConfirmDialog } from 'primeng/confirmdialog';
import {ParserLinkResponse, ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {DefaultParserResponse} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-links-list',
  imports: [NgForOf, NgIf, Toast, ConfirmDialog, NgClass],
  templateUrl: './scraper-links-list.component.html',
  styleUrl: './scraper-links-list.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class ScraperLinksListComponent {
  constructor(private readonly _confirmationService: ConfirmationService) {
    this.scraper = signal(DefaultParserResponse());
    this.scraperLinks = signal([]);
    this.linkActivityChanged = new EventEmitter();
    this.onLinkRemoveConfirmed = new EventEmitter();
    this.onLinkEditClicked = new EventEmitter();
    this.onLinkCreateClicked = new EventEmitter();
  }

  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this.scraper.set(value);
  }

  @Input({ required: true }) set scraper_links_setter(value: ParserLinkResponse[]) {
    this.scraperLinks.set(value);
  }

  @Output() linkActivityChanged: EventEmitter<{ link: ParserLinkResponse, activity: boolean }>;
  @Output() onLinkRemoveConfirmed: EventEmitter<ParserLinkResponse>;
  @Output() onLinkEditClicked: EventEmitter<ParserLinkResponse>;
  @Output() onLinkCreateClicked: EventEmitter<boolean>;

  readonly scraperLinks: WritableSignal<ParserLinkResponse[]>;
  readonly scraper: WritableSignal<ParserResponse>;

  public linkActivitySwitch(link: ParserLinkResponse): void {
    const otherValue: boolean = !link.IsActive;
    this.linkActivityChanged.emit({ link: link, activity: otherValue })
  }

  public editClick(link: ParserLinkResponse): void {
    this.onLinkEditClicked.emit(link);
  }

  public createClick(): void {
    this.onLinkCreateClicked.emit(true);
  }

  public confirmRemove($event: Event, link: ParserLinkResponse): void {
    this._confirmationService.confirm({
      target: $event.target as EventTarget,
      message: `Удалить ссылку ${link.UrlName} ?`,
      header: 'Подтверждение',
      closable: true,
      closeOnEscape: true,
      icon: 'pi pi-exclamation-triangle',
      rejectButtonProps: {
        label: 'Отменить',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Подтвердить',
      },
      accept: () => {
        this.onLinkRemoveConfirmed.emit(link);
      },
      reject: () => {},
    });
  }
}
