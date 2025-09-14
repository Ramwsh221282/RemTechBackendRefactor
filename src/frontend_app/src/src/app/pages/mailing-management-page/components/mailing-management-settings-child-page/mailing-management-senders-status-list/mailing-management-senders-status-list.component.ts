import {
  Component,
  DestroyRef,
  effect,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { MailingSender } from '../../../models/MailingSender';
import { MailingManagementService } from '../../../services/MailingManagementService';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-mailing-management-senders-status-list',
  imports: [NgClass],
  templateUrl: './mailing-management-senders-status-list.component.html',
  styleUrl: './mailing-management-senders-status-list.component.scss',
})
export class MailingManagementSendersStatusListComponent implements OnInit {
  @Output() onFetched: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() onSenderPingClicked: EventEmitter<MailingSender> =
    new EventEmitter<MailingSender>();
  private readonly _senders: WritableSignal<MailingSender[]>;
  private readonly _shouldFetch: WritableSignal<boolean>;
  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  private readonly _service: MailingManagementService;

  constructor(service: MailingManagementService) {
    this._senders = signal([]);
    this._service = service;
    this._shouldFetch = signal(false);
    effect((): void => {
      if (this._shouldFetch()) this.ngOnInit();
    });
  }

  @Input({ required: true }) set should_fetch_setter(value: boolean) {
    this._shouldFetch.set(value);
  }

  public boolHasSenderWith(postFix: string): boolean {
    const sender: MailingSender | undefined = this.findSenderByPostfix(postFix);
    return !!sender;
  }

  public pingSenderByPostfix(postfix: string): void {
    const sender: MailingSender | undefined = this.findSenderByPostfix(postfix);
    if (sender) this.onSenderPingClicked.emit(sender);
  }

  private findSenderByPostfix(postFix: string): MailingSender | undefined {
    const senders: MailingSender[] = this._senders();
    return senders.find((s: MailingSender): boolean =>
      s.email.toLowerCase().includes(postFix.toLowerCase()),
    );
  }

  ngOnInit(): void {
    this._service
      .read()
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (items: MailingSender[]): void => {
          this._senders.set(items);
          this.onFetched.emit(false);
        },
      });
  }
}
