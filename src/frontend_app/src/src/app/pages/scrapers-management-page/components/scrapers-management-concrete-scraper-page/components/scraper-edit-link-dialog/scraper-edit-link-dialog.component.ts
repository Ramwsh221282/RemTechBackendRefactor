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
import { Scraper } from '../../../scrapers-management-settings-page/types/Scraper';
import { ScraperLink } from '../../../scrapers-management-settings-page/types/ScraperLink';
import { MessageService } from 'primeng/api';
import { VehicleScrapersService } from '../../../scrapers-management-settings-page/services/vehicle-scrapers.service';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Button } from 'primeng/button';
import { StringUtils } from '../../../../../../shared/utils/string-utils';
import { MessageServiceUtils } from '../../../../../../shared/utils/message-service-utils';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UpdateParserLinkEndpointResponse } from '../../../scrapers-management-settings-page/types/UpdateParserLinkEndpointResponse';
import { HttpErrorResponse } from '@angular/common/http';
import { Toast } from 'primeng/toast';
import {ParserLinkResponse, ParserResponse} from '../../../../../../shared/api/parsers-module/parsers-responses';
import {ParsersControlApiService} from '../../../../../../shared/api/parsers-module/parsers-control-api.service';
import {
  DefaultParserLinkResponse,
  DefaultParserResponse
} from '../../../../../../shared/api/parsers-module/parsers-factory';

@Component({
  selector: 'app-scraper-edit-link-dialog',
  imports: [Dialog, ReactiveFormsModule, InputText, Button, FormsModule, Toast],
  templateUrl: './scraper-edit-link-dialog.component.html',
  styleUrl: './scraper-edit-link-dialog.component.scss',
})
export class ScraperEditLinkDialogComponent {
  @Output() onSave: EventEmitter<Scraper> = new EventEmitter<Scraper>();
  @Output() onDialogClose: EventEmitter<boolean> = new EventEmitter<boolean>();

  @Input({ required: true }) set scraper_setter(scraper: ParserResponse) {
    this.scraper.set(scraper);
  }

  @Input({ required: true }) set scraper_link_setter(scraperLink: ParserLinkResponse) {
    this.scraperLink.set(scraperLink);
    // this.editScraperLinkForm.patchValue({
    //   name: scraperLink.name,
    //   url: scraperLink.url,
    // });
  }

  @Input({ required: true }) set visibility_setter(visibility: boolean) {
    this.visibility = visibility;
  }

  public editScraperLinkForm: FormGroup = new FormGroup({
    name: new FormControl(''),
    url: new FormControl(''),
  });

  public visibility: boolean = false;
  readonly scraper: WritableSignal<ParserResponse>;
  readonly scraperLink: WritableSignal<ParserLinkResponse>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);

  constructor(
    private readonly _messageService: MessageService,
    private readonly _service: ParsersControlApiService) {
    this.scraper = signal(DefaultParserResponse());
    this.scraperLink = signal(DefaultParserLinkResponse());
  }

  public onSubmit(): void {
    // const formValues = this.editScraperLinkForm.value;
    // const name: string = formValues.name as string;
    // const url: string = formValues.url as string;
    // if (StringUtils.isEmptyOrWhiteSpace(name)) {
    //   MessageServiceUtils.showError(
    //     this._messageService,
    //     `Название ссылки парсера пустое.`,
    //   );
    //   return;
    // }
    // if (StringUtils.isEmptyOrWhiteSpace(url)) {
    //   MessageServiceUtils.showError(
    //     this._messageService,
    //     `URL ссылки парсера пустое.`,
    //   );
    //   return;
    // }
    // this.editScraperLink(name, url);
  }

  private editScraperLink(name: string, url: string): void {
    // const current: Scraper = this._scraper();
    // const currentLink: ScraperLink = this._scraperLink();
    // const indexOfCurrentLink: number = current.links.findIndex(
    //   (link) => link.name === currentLink.name && link.url === currentLink.url,
    // );
    // this._service
    //   .updateScraperLink(currentLink, { linkName: name, linkUrl: url })
    //   .pipe(takeUntilDestroyed(this._destroyRef))
    //   .subscribe({
    //     next: (response: UpdateParserLinkEndpointResponse): void => {
    //       current.links[indexOfCurrentLink].name = response.linkName;
    //       current.links[indexOfCurrentLink].url = response.linkUrl;
    //       MessageServiceUtils.showSuccess(
    //         this._messageService,
    //         `Ссылка изменена ${response.linkName} ${response.linkUrl}`,
    //       );
    //       this.onSave.emit(current);
    //     },
    //     error: (err: HttpErrorResponse): void => {
    //       const message: string = err.error.message as string;
    //       MessageServiceUtils.showError(this._messageService, message);
    //     },
    //   });
  }

  public onHide(): void {
    this.onDialogClose.emit(false);
  }
}
