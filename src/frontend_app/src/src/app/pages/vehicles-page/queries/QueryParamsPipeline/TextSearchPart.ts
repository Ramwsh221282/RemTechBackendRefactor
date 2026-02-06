import { QueryParamsPipelinePart } from './QueryParamsPipelinePart';
import { HttpParams } from '@angular/common/http';

export class TextSearchPart implements QueryParamsPipelinePart {
  constructor(private readonly _text: string | undefined) {}

  add(params: HttpParams): HttpParams {
    if (!this._text) return params;
    return params.set('text', this._text);
  }
}
