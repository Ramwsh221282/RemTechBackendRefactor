import { QueryParamsPipelinePart } from './QueryParamsPipelinePart';
import { HttpParams } from '@angular/common/http';

export class CategoryIdPart implements QueryParamsPipelinePart {
  constructor(private readonly id: string | undefined) {}

  add(params: HttpParams): HttpParams {
    if (!this.id) return params;
    return params.set('categoryId', this.id);
  }
}
