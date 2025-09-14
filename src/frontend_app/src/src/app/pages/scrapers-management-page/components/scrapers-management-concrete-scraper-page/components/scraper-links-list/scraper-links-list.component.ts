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

@Component({
  selector: 'app-scraper-links-list',
  imports: [NgForOf, NgIf, Toast, ConfirmDialog, NgClass],
  templateUrl: './scraper-links-list.component.html',
  styleUrl: './scraper-links-list.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class ScraperLinksListComponent {
  @Output() onLinkCreateChange: EventEmitter<boolean> = new EventEmitter();
  @Output() onLinkRemoved: EventEmitter<Scraper> = new EventEmitter();
  @Output() onLinkEditChange: EventEmitter<boolean> = new EventEmitter();
  @Output() linkToEditSelect: EventEmitter<ScraperLink> = new EventEmitter();
  @Output() linkActivityChanged: EventEmitter<Scraper> = new EventEmitter();
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }
  @Input({ required: true }) set scraper_links_setter(value: ScraperLink[]) {
    this._scraperLinks.set(value);
  }

  private readonly _scraperLinks: WritableSignal<ScraperLink[]>;
  private readonly _service: VehicleScrapersService;
  private readonly _messageService: MessageService;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _scraper: WritableSignal<Scraper>;
  private readonly _confirmationService: ConfirmationService;
  constructor(
    service: VehicleScrapersService,
    messageService: MessageService,
    confirmationService: ConfirmationService,
  ) {
    this._scraper = signal(VehicleScrapersService.defaultScraper());
    this._scraperLinks = signal([]);
    this._confirmationService = confirmationService;
    this._service = service;
    this._messageService = messageService;
  }

  public get links(): ScraperLink[] {
    return this._scraperLinks();
  }

  public click(): void {
    this.onLinkCreateChange.emit(true);
  }

  public editClick(link: ScraperLink): void {
    this.onLinkEditChange.emit(true);
    this.linkToEditSelect.emit(link);
  }

  public changeLinkActivity(link: ScraperLink): void {
    const scraper: Scraper = this._scraper();
    const indexOfLink: number = scraper.links.findIndex(
      (l: ScraperLink): boolean => l.name === link.name && l.url === link.url,
    );
    const oppositeActivity: boolean = !link.activity;
    this._service
      .changeLinkActivity(link, { activity: oppositeActivity })
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (response: LinkWithChangedActivityResponse): void => {
          scraper.links[indexOfLink].activity = response.newActivity;
          this.linkActivityChanged.emit(scraper);
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Активность ссылки ${link.name} изменена на ${response.newActivity ? 'Активна' : 'Неактивна'}`,
          );
        },
        error: (err: HttpErrorResponse): void => {
          const message: string = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }

  public confirmRemove($event: Event, link: ScraperLink): void {
    this._confirmationService.confirm({
      target: $event.target as EventTarget,
      message: `Удалить ссылку ${link.name} ${link.parserName} ${link.parserType} ?`,
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
        this.removeLink(link);
      },
      reject: () => {},
    });
  }

  private removeLink(link: ScraperLink): void {
    const current: Scraper = this._scraper();
    this._service
      .removeParserLink(current, { linkName: link.name })
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (removed: RemoveParserLinkResponse): void => {
          current.links = current.links.filter(
            (link: ScraperLink) =>
              link.name !== removed.linkName && link.url !== removed.linkUrl,
          );
          this.onLinkRemoved.emit(current);
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Удалена ссылка ${removed.linkName} ${removed.parserName} ${removed.parserType}`,
          );
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
        },
      });
  }
}
