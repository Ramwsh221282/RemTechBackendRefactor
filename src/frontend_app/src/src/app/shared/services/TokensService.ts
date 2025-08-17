import { Injectable, signal, WritableSignal } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class TokensService {
  public readonly tokenId: WritableSignal<string>;
  public readonly isAdminSignal: WritableSignal<boolean>;
  public readonly hasTokenSignal: WritableSignal<boolean>;
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
    this.hasTokenSignal.set(false);
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
      this.hasTokenSignal.set(true);
    }
  }
}
