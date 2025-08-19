import {
  Component,
  DestroyRef,
  effect,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { ScraperJournalRecordResponse } from '../../types/ScraperJournalRecordResponse';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { StringUtils } from '../../../../../../shared/utils/string-utils';
import { DatePipe, NgForOf, NgIf } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-scraper-journal-details',
  imports: [NgForOf, NgIf, DatePipe, ReactiveFormsModule, PaginationComponent],
  templateUrl: './scraper-journal-details.component.html',
  styleUrl: './scraper-journal-details.component.scss',
})
export class ScraperJournalDetailsComponent {
  private readonly _scraperJournalRecords: WritableSignal<
    ScraperJournalRecordResponse[]
  >;
  private readonly _journalId: WritableSignal<string>;
  private readonly _page: WritableSignal<number>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _searchInput: WritableSignal<string>;
  private readonly _totalCount: WritableSignal<number>;
  public readonly _searchForm: FormGroup = new FormGroup({
    text: new FormControl(''),
  });
  constructor(service: VehicleScrapersService, activatedRoute: ActivatedRoute) {
    this._totalCount = signal(0);
    this._searchInput = signal('');
    this._scraperJournalRecords = signal([]);
    this._journalId = signal('');
    this._page = signal(1);
    effect(() => {
      activatedRoute.params
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (params): void => {
            const journalId = params['journalId'];
            this._journalId.set(journalId);
          },
        });
    });
    effect(() => {
      const journalId: string = this._journalId();
      const page: number = this._page();
      const text: string = this._searchInput();
      const requestText: string | null = StringUtils.isEmptyOrWhiteSpace(text)
        ? null
        : text;
      if (StringUtils.isEmptyOrWhiteSpace(journalId)) return;
      service
        .fetchJournalRecords(journalId, page, requestText)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (data: ScraperJournalRecordResponse[]): void => {
            this._scraperJournalRecords.set(data);
          },
        });
    });
    effect(() => {
      const journalId: string = this._journalId();
      if (StringUtils.isEmptyOrWhiteSpace(journalId)) return;
      service
        .fetchJournalRecordsCount(journalId)
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (value: number): void => {
            this._totalCount.set(value);
          },
        });
    });
  }

  public get totalCount(): number {
    return this._totalCount();
  }

  public acceptPageChange($event: number): void {
    this._page.set($event);
    window.scrollTo(0, 0);
  }

  public get currentPage(): number {
    return this._page();
  }

  public submitSearch(): void {
    const formValues = this._searchForm.value;
    const text: string = formValues.text as string;
    this._searchInput.set(text);
  }

  public resetSearch(): void {
    this._searchForm.reset();
    this._searchInput.set('');
  }

  public extractLink(text: string): string | null {
    if (!text) return null;
    const match = text.match(/https?:\/\/[^\s]+/);
    return match ? match[0] : null;
  }

  public formatText(text: string): string {
    if (!text) return '';
    return text.replace(/https?:\/\/[^\s]+/, '').trim();
  }

  public get records(): ScraperJournalRecordResponse[] {
    return this._scraperJournalRecords();
  }
}
