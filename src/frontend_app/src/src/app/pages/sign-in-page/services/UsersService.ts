import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { apiUrl } from '../../../shared/api/api-endpoint';
import { Observable } from 'rxjs';
import { StringUtils } from '../../../shared/utils/string-utils';
import { TokensService } from '../../../shared/services/TokensService';
import { UserInfo } from '../types/UserInfo';
import { ReadUserResponse } from '../../users-management-page/types/ReadUserResponse';
import { ReadRoleResponse } from '../../users-management-page/types/ReadRoleResponse';
import { UpdateUserProfileResult } from '../types/UpdateUserProfileResult';
import { UpdateUserRequest } from '../types/UpdateUserRequest';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;

  constructor(
    httpClient: HttpClient,
    private readonly tokensService: TokensService,
  ) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/users`;
  }

  public readRoles(): Observable<ReadRoleResponse[]> {
    const requestUrl: string = `${this._apiUrl}/roles`;
    return this._httpClient.get<ReadRoleResponse[]>(requestUrl);
  }

  public updateUserProfile(
    request: UpdateUserRequest,
  ): Observable<UpdateUserProfileResult> {
    const requestUrl: string = `${this._apiUrl}/profile`;
    return this._httpClient.put<UpdateUserProfileResult>(requestUrl, request);
  }

  public readUsersCount(
    nameFilter?: string | null,
    emailFilter?: string | null,
    roleFilter?: string | null,
  ): Observable<number> {
    const requestUrl: string = `${this._apiUrl}/count`;
    let params: HttpParams = new HttpParams();
    if (nameFilter) params = params.set('nameFilter', nameFilter);
    if (emailFilter) params = params.set('emailFilter', emailFilter);
    if (roleFilter) params = params.set('roleFilter', roleFilter);
    return this._httpClient.get<number>(requestUrl, {
      params: params,
    });
  }

  public removeUserProfile(id: string): Observable<any> {
    const requestUrl: string = `${this._apiUrl}/profile`;
    const params: HttpParams = new HttpParams().set('userId', id);
    return this._httpClient.delete<any>(requestUrl, { params: params });
  }

  public createUserByAdmin(
    email: string,
    name: string,
    role: string,
  ): Observable<ReadUserResponse> {
    const requestUrl: string = `${this._apiUrl}/user-by-admin`;
    const body = { email, name, role };
    return this._httpClient.post<ReadUserResponse>(requestUrl, body);
  }

  public readUsers(
    page: number,
    nameFilter?: string | null,
    emailFilter?: string | null,
    roleFilter?: string | null,
  ): Observable<ReadUserResponse[]> {
    const request: string = `${this._apiUrl}/list`;
    let params: HttpParams = new HttpParams().set('page', page);
    if (nameFilter) params = params.set('nameFilter', nameFilter);
    if (emailFilter) params = params.set('emailFilter', emailFilter);
    if (roleFilter) params = params.set('roleFilter', roleFilter);
    return this._httpClient.get<ReadUserResponse[]>(request, {
      params: params,
    });
  }

  public changeUserPassword(
    token: string,
    password: string,
    newPassword: string,
  ): Observable<any> {
    const requestUrl: string = `${this._apiUrl}/password`;
    const headers = new HttpHeaders()
      .set('RemTechAccessTokenId', token)
      .set('Password', password)
      .set('NewPassword', newPassword);
    return this._httpClient.patch<any>(requestUrl, null, { headers: headers });
  }

  public createChangeEmailTicket(
    token: string,
    userId: string,
    password: string,
    newEmail: string,
  ): Observable<any> {
    const requestUrl: string = `${this._apiUrl}/email`;
    const headers: HttpHeaders = new HttpHeaders()
      .set('RemTechAccessTokenId', token)
      .set('Id', userId)
      .set('Password', password)
      .set('NewEmail', newEmail);
    return this._httpClient.patch(requestUrl, null, { headers: headers });
  }

  public createEmailConfirmationTicket(
    password: string,
    tokenId: string,
  ): Observable<string> {
    const requestUrl = `${this._apiUrl}/email-confirmation`;
    const headers = new HttpHeaders()
      .set('RemTechAccessTokenId', tokenId)
      .set('Password', password);
    return this._httpClient.patch<string>(requestUrl, null, {
      headers: headers,
    });
  }

  public confirmEmail(id: string): Observable<any> {
    const requestUrl = `${this._apiUrl}/confirm-email`;
    const params: HttpParams = new HttpParams().set('id', id);
    return this._httpClient.get<any>(requestUrl, { params: params });
  }

  public fetchUserInfo(tokenId: string): Observable<UserInfo> {
    const requestUrl = `${this._apiUrl}/info`;
    const headers: HttpHeaders = new HttpHeaders().set(
      'RemTechAccessTokenId',
      tokenId,
    );
    return this._httpClient.get<UserInfo>(requestUrl, { headers: headers });
  }

  public register(
    email: string,
    password: string,
    name: string,
  ): Observable<any> {
    const requestUrl = `${this._apiUrl}/sign-up`;
    const body = { email: email, password: password, name: name };
    return this._httpClient.post(requestUrl, body);
  }

  public signOut(): Observable<any> {
    const requestUrl = `${this._apiUrl}/sign-out`;
    return this._httpClient.get(requestUrl);
  }

  public verify(token: string): Observable<any> {
    const requestUrl = `${this._apiUrl}/verify`;
    const headers = new HttpHeaders().set('RemTechAccessTokenId', token);
    return this._httpClient.get<any>(requestUrl, { headers: headers });
  }

  public checkRoot(): Observable<boolean> {
    const requestUrl = `${this._apiUrl}/root-get`;
    return this._httpClient.get<boolean>(requestUrl);
  }

  public refreshSession(): Observable<any> {
    const requestUrl = `${this._apiUrl}/refresh-session`;
    let headers: HttpHeaders = new HttpHeaders();
    headers = this.tokensService.addToken(headers);
    return this._httpClient.get<any>(requestUrl, { headers: headers });
  }

  public upRoot(
    email: string,
    password: string,
    name: string,
  ): Observable<any> {
    const requestUrl = `${this._apiUrl}/root-up`;
    const body = { email: email, password: password, name: name };
    return this._httpClient.post(requestUrl, body);
  }

  public verifyAdminAccess(tokenId: string): Observable<any> {
    const requestUrl = `${this._apiUrl}/verify-admin`;
    const headers: HttpHeaders = new HttpHeaders().set(
      'RemTechAccessTokenId',
      tokenId,
    );
    return this._httpClient.get<any>(requestUrl, { headers: headers });
  }

  public authenticate(
    password: string,
    email?: string | null,
    name?: string | null,
  ): Observable<any> {
    const requestUrl: string = `${this._apiUrl}/sign-in`;
    const headers: HttpHeaders = new HttpHeaders().set('password', password);
    let params: HttpParams = new HttpParams();
    if (email && !StringUtils.isEmptyOrWhiteSpace(email))
      params = params.set('email', email);
    if (name && !StringUtils.isEmptyOrWhiteSpace(name))
      params = params.set('name', name);
    return this._httpClient.post<Observable<any>>(requestUrl, null, {
      headers: headers,
      params: params,
    });
  }
}
