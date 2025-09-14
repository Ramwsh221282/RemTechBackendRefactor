import { QueryParamsPipelinePart } from './QueryParamsPipelinePart';
import { HttpParams } from '@angular/common/http';

export class PriceFilterPart implements QueryParamsPipelinePart {
  constructor(
    private readonly priceFrom: number | null,
    private readonly priceTo: number | null,
  ) {}

  add(params: HttpParams): HttpParams {
    if (this.priceFrom) {
      params = params.set('priceMin', this.priceFrom);
    }
    if (this.priceTo) {
      params = params.set('priceMax', this.priceTo);
    }
    return params;
  }
}
