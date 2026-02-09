import { QueryParamsPipelinePart } from './QueryParamsPipelinePart';
import { HttpParams } from '@angular/common/http';

export class BrandIdPart implements QueryParamsPipelinePart {
  constructor(private readonly _id: string | undefined) {}

  add(params: HttpParams): HttpParams {
    if (!this._id) return params;
    return params.set('brandId', this._id);
  }
}
