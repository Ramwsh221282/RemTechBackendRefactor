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
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { StringUtils } from '../../../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Button } from 'primeng/button';
import { Toast } from 'primeng/toast';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CreateNewParserLinkResponse } from '../../../scrapers-management-settings-page/types/CreateNewParserLinkResponse';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-scraper-add-link-dialog',
  imports: [Dialog, ReactiveFormsModule, InputText, Button, Toast],
  templateUrl: './scraper-add-link-dialog.component.html',
  styleUrl: './scraper-add-link-dialog.component.scss',
  providers: [MessageService],
})
export class ScraperAddLinkDialogComponent {
  @Output() onLinkAdd: EventEmitter<Scraper> = new EventEmitter<Scraper>();
  @Output() visibilityChange: EventEmitter<boolean> =
    new EventEmitter<boolean>();
  @Input({ required: true }) set visbility_setter(value: boolean) {
    this.visibility = value;
  }
  addLinkForm: FormGroup = new FormGroup({
    name: new FormControl(''),
    url: new FormControl(''),
  });
  @Input({ required: true }) set scraper_setter(value: Scraper) {
    this._scraper.set(value);
  }

  public visibility: boolean;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _service: VehicleScrapersService;
  private readonly _messageService: MessageService;
  private readonly _scraper: WritableSignal<Scraper>;
  constructor(service: VehicleScrapersService, messageService: MessageService) {
    this._scraper = signal(VehicleScrapersService.defaultScraper());
    this.visibility = false;
    this._messageService = messageService;
    this._service = service;
  }

  public onSubmit(): void {
    const formValues = this.addLinkForm.value;
    const name: string = formValues.name as string;
    const url: string = formValues.url as string;
    if (StringUtils.isEmptyOrWhiteSpace(name)) {
      MessageServiceUtils.showError(
        this._messageService,
        'Название ссылки было пустым.',
      );
      return;
    }
    if (StringUtils.isEmptyOrWhiteSpace(url)) {
      MessageServiceUtils.showError(
        this._messageService,
        'URL ссылки был пустым.',
      );
      return;
    }
    this.addLink(name, url);
  }

  private addLink(linkName: string, linkUrl: string): void {
    const scraper: Scraper = this._scraper();
    this._service
      .addParserLink(scraper, { name: linkName, url: linkUrl })
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (data: CreateNewParserLinkResponse): void => {
          scraper.links.push({
            name: data.name,
            url: data.url,
            activity: data.activity,
            hours: data.elapsedHours,
            minutes: data.elapsedMinutes,
            parserName: data.parserName,
            parserType: data.parserType,
            processed: data.processedAmount,
            seconds: data.elapsedSeconds,
            totalSeconds: data.totalElapsedSeconds,
          });
          this.onLinkAdd.emit(scraper);
          this.addLinkForm.reset();
          MessageServiceUtils.showSuccess(
            this._messageService,
            `Добавлена ссылка ${data.name} ${data.parserName} ${data.parserType}`,
          );
        },
        error: (err: HttpErrorResponse): void => {
          const error: string = err.error.message;
          MessageServiceUtils.showError(this._messageService, error);
        },
      });
  }

  public onHide(): void {
    this.visibilityChange.emit(false);
  }
}
