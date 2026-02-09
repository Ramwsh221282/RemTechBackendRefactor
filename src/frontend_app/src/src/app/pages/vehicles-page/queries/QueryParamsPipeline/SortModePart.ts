import { QueryParamsPipelinePart } from './QueryParamsPipelinePart';
import { HttpParams } from '@angular/common/http';

export class SortModePart implements QueryParamsPipelinePart {
  constructor(private readonly _mode: string | undefined) {}

  add(params: HttpParams): HttpParams {
    if (!this._mode) return params;
    return params.set('sort', this._mode);
  }
}
