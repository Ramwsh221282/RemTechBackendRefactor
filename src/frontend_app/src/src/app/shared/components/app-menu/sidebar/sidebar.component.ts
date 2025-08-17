import {
  Component,
  DestroyRef,
  effect,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { NgIf, NgOptimizedImage } from '@angular/common';
import { Drawer } from 'primeng/drawer';
import { RouterLink } from '@angular/router';
import { UsersService } from '../../../../pages/sign-in-page/services/UsersService';
import { CookieService } from 'ngx-cookie-service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { defer, Observable } from 'rxjs';
import { TokensService } from '../../../services/TokensService';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialog } from 'primeng/confirmdialog';

@Component({
  selector: 'app-sidebar',
  imports: [
    ButtonModule,
    NgOptimizedImage,
    Drawer,
    RouterLink,
    NgIf,
    ConfirmDialog,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  animations: [],
  providers: [ConfirmationService],
})
export class SidebarComponent implements OnInit {
  @Output() closeClicked: EventEmitter<void> = new EventEmitter();
  @Input({ required: true }) isExpanded: boolean = false;

  private readonly _destroyRef: DestroyRef = inject(DestroyRef);
  constructor(
    public readonly usersService: UsersService,
    public readonly tokensService: TokensService,
    private readonly cookieService: CookieService,
    private readonly confirmationService: ConfirmationService,
  ) {
    effect(() => {
      defer(() => this.waitForToken())
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (): void => {
            tokensService.setAdmin();
            tokensService.hasTokenSignal.set(true);
          },
          error: (): void => {
            tokensService.hasTokenSignal.set(false);
            tokensService.setNotAdmin();
          },
        });
    });

    effect(() => {
      defer(() => this.verifyToken())
        .pipe(takeUntilDestroyed(this._destroyRef))
        .subscribe({
          next: (): void => {
            tokensService.hasTokenSignal.set(true);
          },
          error: (): void => {
            tokensService.hasTokenSignal.set(false);
            tokensService.setNotAdmin();
          },
        });
    });
  }

  ngOnInit(): void {
    const tokenId = this.cookieService.get('RemTechAccessTokenId');
  }

  private waitForToken(): Observable<any> {
    return new Observable((observer) => {
      const checkToken = () => {
        const tokenId = this.cookieService.get('RemTechAccessTokenId');
        if (tokenId) {
          this.usersService.verifyAdminAccess(tokenId).subscribe({
            next: (result) => observer.next(result),
            error: (error) => observer.error(error),
          });
        } else {
          setTimeout(checkToken, 100);
        }
      };
      checkToken();
    });
  }

  private verifyToken(): Observable<any> {
    return new Observable((observer) => {
      const checkToken = () => {
        const tokenId = this.cookieService.get('RemTechAccessTokenId');
        if (tokenId) {
          this.usersService.verify(tokenId).subscribe({
            next: (result) => observer.next(result),
            error: (error) => observer.error(error),
          });
        } else {
          setTimeout(checkToken, 100);
        }
      };
      checkToken();
    });
  }

  public closeClick(): void {
    this.closeClicked.emit();
  }

  public confirmSignOut(event: Event) {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: 'Покинуть сессию?',
      header: 'Подтверждение',
      closable: true,
      closeOnEscape: true,
      icon: 'pi pi-exclamation-triangle',
      rejectButtonProps: {
        label: 'Нет',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Да',
      },
      accept: () => {
        this.invokeSignOut();
      },
      reject: () => {},
    });
  }

  private invokeSignOut(): void {
    this.usersService.signOut().subscribe({
      next: () => {
        this.cookieService.delete('RemTechAccessToken');
        this.cookieService.delete('RemTechAccessTokenId');
        this.tokensService.hasTokenSignal.set(false);
        this.tokensService.setNotAdmin();
      },
    });
  }
}
