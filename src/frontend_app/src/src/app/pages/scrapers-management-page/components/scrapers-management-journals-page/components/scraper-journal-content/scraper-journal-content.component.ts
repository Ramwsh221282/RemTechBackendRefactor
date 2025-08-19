import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ScraperJournalResponse } from '../../types/ScraperJournalResponse';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { StringUtils } from '../../../../../../shared/utils/string-utils';
import { DatePipe, NgClass, NgForOf, NgIf } from '@angular/common';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';
import { ConfirmationService, MessageService } from 'primeng/api';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { HttpErrorResponse } from '@angular/common/http';
import { Toast } from 'primeng/toast';
import { ConfirmDialog } from 'primeng/confirmdialog';

@Component({
  selector: 'app-scraper-journal-content',
  imports: [
    NgForOf,
    NgClass,
    NgIf,
    DatePipe,
    PaginationComponent,
    Toast,
    ConfirmDialog,
  ],
  templateUrl: './scraper-journal-content.component.html',
  styleUrl: './scraper-journal-content.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class ScraperJournalContentComponent {
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _journals: WritableSignal<ScraperJournalResponse[]>;
  private readonly _page: WritableSignal<number>;
  private readonly _scraperName: WritableSignal<string>;
  private readonly _scraperType: WritableSignal<string>;
  private readonly _totalCount: WritableSignal<number>;
  constructor(
    activatedRoute: ActivatedRoute,
    private readonly service: VehicleScrapersService,
    private readonly messageService: MessageService,
    private readonly confirmationService: ConfirmationService,
    private readonly _router: Router,
  ) {
    this._totalCount = signal(0);
    this._journals = signal([]);
    this._page = signal(1);
    this._scraperName = signal('');
    this._scraperType = signal('');
    effect((): void => {
      activatedRoute.params
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe((param) => {
          const name = param['scraperName'] as string;
          const type = param['scraperType'] as string;
          this._scraperName.set(name);
          this._scraperType.set(type);
        });
    });
    effect(() => {
      const name: string = this._scraperName();
      const type: string = this._scraperType();
      const page: number = this._page();
      if (
        StringUtils.isEmptyOrWhiteSpace(name) ||
        StringUtils.isEmptyOrWhiteSpace(type)
      )
        return;
      service
        .fetchJournals(name, type, page, null, null)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: ScraperJournalResponse[]): void => {
            console.log(data);
            this._journals.set(data);
          },
        });
    });
    effect(() => {
      const name: string = this._scraperName();
      const type: string = this._scraperType();
      if (
        StringUtils.isEmptyOrWhiteSpace(name) ||
        StringUtils.isEmptyOrWhiteSpace(type)
      )
        return;
      service
        .fetchJournalsCount(name, type)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (value: number): void => {
            this._totalCount.set(value);
          },
        });
    });
  }

  public openDetails(journal: ScraperJournalResponse): void {
    const journalId = journal.id;
    const url = this._router
      .createUrlTree(['/scrapers', 'journals', journalId])
      .toString();
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  public get journals(): ScraperJournalResponse[] {
    return this._journals();
  }

  public get totalCount(): number {
    return this._totalCount();
  }

  public get currentPage(): number {
    return this._page();
  }

  public acceptPageChange($event: number): void {
    this._page.set($event);
  }

  public confirmJournalDrop(
    $event: Event,
    journal: ScraperJournalResponse,
  ): void {
    console.log('clicked');
    $event.stopPropagation();
    this.confirmationService.confirm({
      target: $event.target as EventTarget,
      message: `Удалить журнал: ${journal.name} ${journal.type} от ${journal.createdAt}?`,
      header: 'Confirmation',
      closable: true,
      closeOnEscape: true,
      icon: 'pi pi-exclamation-triangle',
      rejectButtonProps: {
        label: 'Отмена',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Удалить',
      },
      accept: () => {
        this.dropJournal(journal);
      },
      reject: () => {},
    });
  }

  private dropJournal(journal: ScraperJournalResponse): void {
    const journalId = journal.id;
    const name: string = this._scraperName();
    const type: string = this._scraperType();
    if (
      StringUtils.isEmptyOrWhiteSpace(name) ||
      StringUtils.isEmptyOrWhiteSpace(type)
    )
      return;
    this.service
      .removeJournal(journalId, name, type)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (removedId: string): void => {
          const journals = this._journals();
          const index = journals.findIndex((j) => j.id === removedId);
          if (index >= 0) {
            const filtered = journals.filter((j) => j.id !== removedId);
            MessageServiceUtils.showSuccess(
              this.messageService,
              'Журнал был успешно удален.',
            );
            this._journals.set(filtered);
          }
        },
        error: (err: HttpErrorResponse): void => {
          const message = err.error.message as string;
          MessageServiceUtils.showError(this.messageService, message);
        },
      });
  }
}
