import {
  Component,
  DestroyRef,
  effect,
  EventEmitter,
  inject,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import {ActivatedRoute, Params} from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { VehicleScrapersService } from '../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { Scraper } from '../scrapers-management-settings-page/types/Scraper';
import { HttpErrorResponse } from '@angular/common/http';
import { DatePipe, NgClass, NgForOf, NgIf } from '@angular/common';
import { Button } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { ScraperDomainInfoComponent } from './components/scraper-domain-info/scraper-domain-info.component';
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
import { MessageService } from 'primeng/api';
import { ScraperLink } from '../scrapers-management-settings-page/types/ScraperLink';
import { ScraperEditLinkDialogComponent } from './components/scraper-edit-link-dialog/scraper-edit-link-dialog.component';
import { InstantlyEnabledParserResponse } from '../scrapers-management-settings-page/types/InstantlyEnabledParserResponse';
import { MessageServiceUtils } from '../../../../shared/utils/message-service-utils';
import { Toast } from 'primeng/toast';
import {ParsersControlApiService} from '../../../../shared/api/parsers-module/parsers-control-api.service';
import {map, Observable, switchMap, tap} from 'rxjs';
import {TypedEnvelope} from '../../../../shared/api/envelope';
import {ParserLinkResponse, ParserResponse} from '../../../../shared/api/parsers-module/parsers-responses';

@Component({
  selector: 'app-scrapers-management-concrete-scraper-page',
  imports: [
    FormsModule,
    ScraperDomainInfoComponent,
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
    NgIf,
    Toast,
  ],
  templateUrl: './scrapers-management-concrete-scraper-page.component.html',
  styleUrl: './scrapers-management-concrete-scraper-page.component.scss',
  providers: [MessageService],
})
export class ScrapersManagementConcreteScraperPageComponent {
  @Output() parserLinkRemoved: EventEmitter<ParserLinkResponse>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  readonly _isCreatingLink: WritableSignal<boolean>;
  readonly _isEditingLink: WritableSignal<boolean>;
  readonly _scraper: WritableSignal<ParserResponse | null>;
  readonly _linkToEdit: WritableSignal<ParserLinkResponse | null>;

  constructor(
    private readonly _service: ParsersControlApiService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _messageService: MessageService,
  ) {
    this._linkToEdit = signal(null);
    this._isEditingLink = signal(false);
    this._scraper = signal(null);
    this._isCreatingLink = signal(false);
    this.parserLinkRemoved = new EventEmitter<ParserLinkResponse>();
    effect((): void => {
      this.fetchParserInfo();
    });
  }

  private fetchParserInfo(): void {
    this._activatedRoute.params
      .pipe(
        takeUntilDestroyed(this._destroyRef),
        map((parameters: Params) => {
          const id: string = parameters['id'] as string;
          return id;
        }),
        switchMap((id: string): Observable<TypedEnvelope<ParserResponse>> => this._service.fetchParser(id))
      )
      .subscribe({
        next: (response: TypedEnvelope<ParserResponse>): void => {
          if (response.body) this._scraper.set(response.body);
        }
      });
  }

  public instantlyEnableClick(): void {
    // const current: Scraper = this._scraper();
    // this._service
    //   .enableInstantly(current)
    //   .pipe(takeUntilDestroyed(this._destroyRef))
    //   .subscribe({
    //     next: (response: InstantlyEnabledParserResponse): void => {
    //       current.nextRun = response.nextRun;
    //       current.lastRun = response.lastRun;
    //       current.state = response.state;
    //       this._scraper.set(current);
    //       MessageServiceUtils.showSuccess(
    //         this._messageService,
    //         `Парсер ${current.name} ${current.type} немедленно включен.`,
    //       );
    //     },
    //     error: (err: HttpErrorResponse): void => {
    //       const message: string = err.error.message as string;
    //       MessageServiceUtils.showError(this._messageService, message);
    //     },
    //   });
  }

  public get scraperLinkToEdit(): ParserLinkResponse | null {
    return this._linkToEdit();
  }

  public acceptCreatingLink(flag: boolean): void {
    this._isCreatingLink.set(flag);
  }

  public acceptLinkEditClick(flag: boolean): void {
    console.log(flag);
    this._isEditingLink.set(flag);
  }

  public get isCreatingLink(): boolean {
    return this._isCreatingLink();
  }

  public acceptLinkToEdit(link: ParserLinkResponse): void {
    this._linkToEdit.set(link);
  }

  public acceptScraperChangedState($event: Scraper): void {
    // this._scraper.set($event);
  }

  public onEditClose(): void {
    this._isEditingLink.set(false);
    this._linkToEdit.set(null);
  }

  public get isEditingLink(): boolean {
    return this._isEditingLink();
  }

  public get scraper(): ParserResponse | null {
    return this._scraper();
  }
}
