import { Component, DestroyRef, effect, EventEmitter, inject, Output, signal, WritableSignal } from '@angular/core';
import {ActivatedRoute, Params} from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';

import { FormsModule } from '@angular/forms';
import { ScraperLastRunInfoComponent } from './components/scraper-last-run-info/scraper-last-run-info.component';
import { ScraperNextRunInfoComponent } from './components/scraper-next-run-info/scraper-next-run-info.component';
import { ScraperWaitDaysSelectComponent } from './components/scraper-wait-days-select/scraper-wait-days-select.component';
import { ScraperProcessedAmountComponent } from './components/scraper-processed-amount/scraper-processed-amount.component';
import { ScraperElapsedTimeComponent } from './components/scraper-elapsed-time/scraper-elapsed-time.component';
import { ScraperStateSelectComponent } from './components/scraper-state-select/scraper-state-select.component';
import { ScraperLinksListComponent } from './components/scraper-links-list/scraper-links-list.component';
import { ScraperActivateButtonComponent } from './components/scraper-activate-button/scraper-activate-button.component';
import { ScraperDeactivateButtonComponent } from './components/scraper-deactivate-button/scraper-deactivate-button.component';
import { ScraperAddLinkDialogComponent } from './components/scraper-add-link-dialog/scraper-add-link-dialog.component';
import {ConfirmationService, MessageService} from 'primeng/api';
import { ScraperEditLinkDialogComponent } from './components/scraper-edit-link-dialog/scraper-edit-link-dialog.component';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { Toast } from 'primeng/toast';
import {ParsersControlApiService} from '../../../../shared/api/parsers-module/parsers-control-api.service';
import {catchError, EMPTY, map, Observable, switchMap, tap, throwError} from 'rxjs';
import {TypedEnvelope} from '../../../../shared/api/envelope';
import {ParserLinkResponse, ParserResponse} from '../../../../shared/api/parsers-module/parsers-responses';
import {ScraperHeaderComponent} from './components/scraper-header/scraper-header.component';
import {ScraperLastRunStartedComponent} from './components/scraper-last-run-started/scraper-last-run-started.component';
import {ConfirmDialog} from 'primeng/confirmdialog';

@Component({
  selector: 'app-scrapers-management-concrete-scraper-page',
  imports: [
    FormsModule,
    ScraperLastRunInfoComponent,
    ScraperNextRunInfoComponent,
    ScraperWaitDaysSelectComponent,
    ScraperProcessedAmountComponent,
    ScraperElapsedTimeComponent,
    ScraperStateSelectComponent,
    ScraperLinksListComponent,
    ScraperActivateButtonComponent,
    ScraperDeactivateButtonComponent,
    ScraperAddLinkDialogComponent,
    ScraperEditLinkDialogComponent,
    Toast,
    ScraperHeaderComponent,
    ScraperLastRunStartedComponent,
    ConfirmDialog
],
  templateUrl: './scrapers-management-concrete-scraper-page.component.html',
  styleUrl: './scrapers-management-concrete-scraper-page.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class ScrapersManagementConcreteScraperPageComponent {
  @Output() parserLinkRemoved: EventEmitter<ParserLinkResponse>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  readonly isCreatingLink: WritableSignal<boolean>;
  readonly scraper: WritableSignal<ParserResponse | null>;
  readonly linkToEdit: WritableSignal<ParserLinkResponse | null>;

  constructor(
    private readonly _service: ParsersControlApiService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _messageService: MessageService,
    private readonly _confirmationService: ConfirmationService,
  ) {
    this.linkToEdit = signal(null);
    this.scraper = signal(null);
    this.isCreatingLink = signal(false);
    this.parserLinkRemoved = new EventEmitter<ParserLinkResponse>();
    effect((): void => {
      this.fetchParserInfo();
    });
  }

  public onLinkEditClicked(link: ParserLinkResponse): void {
    this.linkToEdit.set(link);
  }

  public acceptCreatingLink(flag: boolean): void {
    this.isCreatingLink.set(flag);
  }

  public onEditLinkConfirmed(payload: { linkId: string, name: string | null, url: string | null }): void {
    this.linkToEdit.set(null);
    const current: ParserResponse | null = this.scraper();
    if (current) this.updateParserLink(current, payload.linkId, payload.name, payload.url);
  }

  public onParserStartClicked(parser: ParserResponse): void {
    const current: ParserResponse | null = this.scraper();
    if (current) this.handleParserStarting(current);
  }

  public onEditClose(): void {
    this.linkToEdit.set(null);
  }

  public onLinkRemoveConfirmed(link: ParserLinkResponse): void {
    const current: ParserResponse | null = this.scraper();
    if (current) this.removeParserLink(current, link);
  }

  public onWaitDaysChange(days: number): void {
    const current: ParserResponse | null = this.scraper();
    if (current) this.updateParserWaitDays(current, days);
  }

  public onLinksAddConfirmed(links: ParserLinkResponse[]): void {
    const current: ParserResponse | null = this.scraper();
    if (current) this.addLinksToParser(current, links);
  }

  public onLinkActivitySwitch(switch$: { link: ParserLinkResponse, activity: boolean }): void {
    const current: ParserResponse | null = this.scraper();
    if (current) this.updateLinkActivity(current, switch$.link, switch$.activity)
  }

  public instantlyEnableClick(): void {
    const current: ParserResponse | null = this.scraper();
    if (current) this.handleInstantlyEnable(current);
  }

  public onParserDisableClicked(parser: ParserResponse): void {
    const current: ParserResponse | null = this.scraper();
    if (current) this.handleDisablingParser(current);
  }

  private handleDisablingParser(parser: ParserResponse): void {
    const isWorking: boolean = parser.State === 'В работе';
    const id: string = parser.Id;
    if (isWorking) this.handleDisablingParserOnWorking(id);
    else this.handleDisablingOnSleeping(id);
  }

  private handleParserStarting(parser: ParserResponse): void {
    const parserId: string = parser.Id;
    this._service.enableParser(parserId)
      .pipe(
        map((response: TypedEnvelope<ParserResponse>): ParserResponse | null | undefined => response.body),
        tap((updated: ParserResponse | null | undefined): void => {
          if (updated) {
            const message = `Парсер ${updated.Domain} ${updated.ServiceType} был включен.`;
            MessageServiceUtils.showSuccess(this._messageService, message);
            this.scraper.set(updated);
          }
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe();
  }

  private handleDisablingOnSleeping(parserId: string): void {
    this._service.disableParser(parserId)
      .pipe(
        map((response: TypedEnvelope<ParserResponse>): ParserResponse | null | undefined => response.body),
        tap((updated: ParserResponse | null | undefined): void => {
          if (updated) {
            const message = `Парсер ${updated.Domain} ${updated.ServiceType} был выключен.`;
            MessageServiceUtils.showSuccess(this._messageService, message);
            this.scraper.set(updated);
          }
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe();
  }

  private handleDisablingParserOnWorking(parserId: string): void {
    this._confirmationService.confirm({
      message: 'Парсер сейчас в рабочем состоянии. Выключение парсера остановит процесс парсинга. Вы хотите продолжить ?',
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
        this._service.permantlyDisableParser(parserId)
          .pipe(
            takeUntilDestroyed(this._destroyRef),
            map((response: TypedEnvelope<ParserResponse>): ParserResponse | null | undefined => response.body),
            tap((updated: ParserResponse | null | undefined): void => {
              if (updated) {
                const message = `Парсер ${updated.Domain} ${updated.ServiceType} был немедленно выключен.`;
                MessageServiceUtils.showSuccess(this._messageService, message);
                this.scraper.set(updated);
              }
            }),
            catchError((error: HttpErrorResponse): Observable<never> => {
              const message: string = error.error.message as string;
              MessageServiceUtils.showError(this._messageService, message);
              return EMPTY;
            })
          ).subscribe();
      },
      reject: () => {},
    });
  }

  private handleInstantlyEnable(parser: ParserResponse): void {
    const parserId: string = parser.Id;
    this._service.permantlyStartParser(parserId)
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((response: TypedEnvelope<ParserResponse>): ParserResponse | null | undefined => response.body),
        tap((updated: ParserResponse | null | undefined): void => {
          if (updated) {
            const message: string = `Парсер ${updated.Domain} ${updated.ServiceType} немедленно включен.`;
            MessageServiceUtils.showSuccess(this._messageService, message);
            this.scraper.set(updated);
          }
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe();
  }

  private fetchParserInfo(): void {
    this._activatedRoute.params
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((parameters: Params) => parameters['id'] as string),
        switchMap((id: string): Observable<TypedEnvelope<ParserResponse>> => this._service.fetchParser(id)),
        tap((response: TypedEnvelope<ParserResponse>): void => {
          this.scraper.set(response.body ?? null);
        }),
        catchError((error: HttpErrorResponse): Observable<never> => throwError((): HttpErrorResponse => error))
      ).subscribe();
  }

  private addLinksToParser(parser: ParserResponse, links: ParserLinkResponse[]): void {
    const parserId: string = parser.Id;
    const linksPayload: { name: string, url: string }[] = links.map((l): { name: string, url: string } => ({ name: l.UrlName, url: l.UrlValue }));
    this._service.addLinksToParser(parserId, linksPayload)
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((response: TypedEnvelope<ParserLinkResponse[]>) => {
          const current: ParserLinkResponse[] = [...parser.Links];
          if (response.body) return { addedItems: [...response.body, ...current], added: response.body.length };
          return { addedItems: current, added: 0 };
        }),
        tap((values): void => {
          const message: string = `Добавлено ${values.added} новых ссылок.`
          MessageServiceUtils.showSuccess(this._messageService, message);
          this.scraper.update((scraper: ParserResponse | null): ParserResponse | null => scraper ? { ...scraper, Links: values.addedItems } : null);
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe()
  }

  private updateLinkActivity(parser: ParserResponse, link: ParserLinkResponse, activity: boolean): void {
    const parserId: string = parser.Id;
    const linkId: string = link.Id;
    const linkIndex: number = parser.Links.findIndex((l: ParserLinkResponse): boolean => l.Id === linkId);
    if (linkIndex < 0) return;
    this._service.changeLinkActivity(parserId, linkId, activity)
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((response: TypedEnvelope<ParserLinkResponse>): ParserLinkResponse[] => {
          const current: ParserLinkResponse[] = [...parser.Links];
          if (response.body) current[linkIndex] = response.body;
          return current;
        }),
        tap((updated: ParserLinkResponse[]): void => {
          MessageServiceUtils.showSuccess(this._messageService, `Активность ссылки ${link.UrlName} изменена на ${activity ? 'Активна' : 'Неактивна'}`);
          this.scraper.update((scraper: ParserResponse | null): ParserResponse | null => scraper ? { ...scraper, Links: updated } : null);
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe();
  }

  private updateParserWaitDays(parser: ParserResponse, days: number): void {
    this._service.changeWaitDays(parser.Id, days)
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((response: TypedEnvelope<ParserResponse>): ParserResponse | null | undefined => response.body),
        tap((updated: ParserResponse | null | undefined): void => {
          if (updated) {
            const message = `Изменено число дней ожидания на ${updated.WaitDays}`;
            MessageServiceUtils.showSuccess(this._messageService, message);
            this.scraper.set(updated);
          }
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe();
  }

  private removeParserLink(parser: ParserResponse, link: ParserLinkResponse): void {
    const parserId: string = parser.Id;
    const linkId: string = link.Id;
    this._service.removeLinkFromParser(parserId, linkId)
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((_: TypedEnvelope<ParserLinkResponse>): ParserLinkResponse[] => {
          const current: ParserLinkResponse[] = [...parser.Links];
          const index: number = current.findIndex((l: ParserLinkResponse): boolean => l.Id === linkId);
          if (index >= 0) current.splice(index, 1);
          return current;
        }),
        tap((updated: ParserLinkResponse[]): void => {
          MessageServiceUtils.showSuccess(this._messageService, `Ссылка ${link.UrlName} удалена`);
          this.scraper.update((scraper: ParserResponse | null): ParserResponse | null => scraper ? { ...scraper, Links: updated } : null);
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe();
  }

  private updateParserLink(parser: ParserResponse, linkId: string, name: string | null, url: string | null): void {
    const parserId: string = parser.Id;
    this._service.updateLink(parserId, linkId, name, url)
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((response: TypedEnvelope<ParserLinkResponse>): ParserLinkResponse | null | undefined => response.body),
        tap((updated: ParserLinkResponse | null | undefined): void => {
          if (updated) {
            const message: string = `Ссылка ${updated.UrlName} изменена`;
            MessageServiceUtils.showSuccess(this._messageService, message);
            const current: ParserResponse | null = this.scraper();
            if (current) {
              const links: ParserLinkResponse[] = [...current.Links];
              const index: number = links.findIndex((l: ParserLinkResponse): boolean => l.Id === linkId);
              if (index >= 0) links[index] = updated;
              this.scraper.update((scraper: ParserResponse | null): ParserResponse | null => scraper ? { ...scraper, Links: links } : null);
            }
          }
        }),
        catchError((error: HttpErrorResponse): Observable<never> => {
          const message: string = error.error.message as string;
          MessageServiceUtils.showError(this._messageService, message);
          return EMPTY;
        })
      ).subscribe();
  }
}
