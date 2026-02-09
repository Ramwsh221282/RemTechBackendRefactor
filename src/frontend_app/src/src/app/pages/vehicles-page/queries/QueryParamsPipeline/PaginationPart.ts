import { QueryParamsPipelinePart } from './QueryParamsPipelinePart';
import { HttpParams } from '@angular/common/http';

export class PaginationPart implements QueryParamsPipelinePart {
  constructor(private readonly _page: number) {}

  public get page(): number {
    return this._page;
  }

  add(params: HttpParams): HttpParams {
    return params.set('page', this._page);
  }
}
