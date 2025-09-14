import {
  computed,
  effect,
  Injectable,
  signal,
  WritableSignal,
} from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { UserInfo } from '../../pages/sign-in-page/types/UserInfo';
import { StringUtils } from '../utils/string-utils';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TokensService {
  public readonly tokenId: WritableSignal<string>;
  public readonly isAdminSignal: WritableSignal<boolean>;

  public readonly hasTokenSignal = computed(() => {
    const token = this.tokenId();
    return !StringUtils.isEmptyOrWhiteSpace(token);
  });

  constructor(private readonly cookies: CookieService) {
    this.tokenId = signal('');
    this.hasTokenSignal = signal(false);
    this.isAdminSignal = signal(false);
  }

  public addToken(httpHeaders: HttpHeaders): HttpHeaders {
    this.updateToken();
    return httpHeaders.set('RemTechAccessTokenId', this.tokenId());
  }

  public setAdmin(): void {
    this.isAdminSignal.set(true);
  }

  public setNotAdmin(): void {
    this.isAdminSignal.set(false);
  }

  public hasToken(): boolean {
    return this.hasTokenSignal();
  }

  private updateToken(): void {
    const tokenId: string | undefined = this.cookies.get(
      'RemTechAccessTokenId',
    );
    if (tokenId) {
      this.tokenId.set(tokenId);
    }
  }

  public tokenAsObservable(): Observable<string> {
    return new Observable((observer) => {
      const checkToken = () => {
        const tokenId = this.cookies.get('RemTechAccessTokenId');
        if (tokenId) {
          this.tokenId.set(tokenId);
          observer.next(tokenId);
          observer.complete();
        } else {
          setTimeout(checkToken, 100);
        }
      };
      checkToken();
    });
  }
}
