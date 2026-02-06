import { HttpParams } from '@angular/common/http';

export interface QueryParamsPipelinePart {
  add: (params: HttpParams) => HttpParams;
}
