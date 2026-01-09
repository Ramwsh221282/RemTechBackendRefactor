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
import { NgClass, NgForOf, NgIf } from '@angular/common';
import { ScraperLink } from '../../../scrapers-management-settings-page/types/ScraperLink';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Toast } from 'primeng/toast';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RemoveParserLinkResponse } from '../../../scrapers-management-settings-page/types/RemoveParserLinkResponse';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { LinkWithChangedActivityResponse } from '../../../scrapers-management-settings-page/types/LinkWithChangedActivityResponse';
import {ParsersControlApiService} from '../../../../../../shared/api/parsers-module/parsers-control-api.service';
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
  constructor(
    private readonly _service: ParsersControlApiService,
    private readonly _messageService: MessageService,
    private readonly _confirmationService: ConfirmationService,
  ) {
    this.scraper = signal(DefaultParserResponse());
    this.scraperLinks = signal([]);
    this.linkActivityChanged = new EventEmitter();
    this.onLinkRemoveConfirmed = new EventEmitter();
    this.linkToEditSelect = new EventEmitter();
    this.onLinkCreateChange = new EventEmitter();
    this.onLinkEditChange = new EventEmitter();
  }

  @Input({ required: true }) set scraper_setter(value: ParserResponse) {
    this.scraper.set(value);
  }

  @Input({ required: true }) set scraper_links_setter(value: ParserLinkResponse[]) {
    this.scraperLinks.set(value);
  }

  @Output() linkActivityChanged: EventEmitter<{ link: ParserLinkResponse, activity: boolean }>;
  @Output() onLinkRemoveConfirmed: EventEmitter<ParserLinkResponse>;
  @Output() linkToEditSelect: EventEmitter<ParserLinkResponse>;
  @Output() onLinkCreateChange: EventEmitter<boolean>;
  @Output() onLinkEditChange: EventEmitter<boolean>;

  readonly scraperLinks: WritableSignal<ParserLinkResponse[]>;
  readonly scraper: WritableSignal<ParserResponse>;

  public click(): void {
    this.onLinkCreateChange.emit(true);
  }

  public editClick(link: ParserLinkResponse): void {
    this.onLinkEditChange.emit(true);
    this.linkToEditSelect.emit(link);
  }

  public linkActivitySwitch(link: ParserLinkResponse): void {
    const otherValue: boolean = !link.IsActive;
    this.linkActivityChanged.emit({ link: link, activity: otherValue })
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
