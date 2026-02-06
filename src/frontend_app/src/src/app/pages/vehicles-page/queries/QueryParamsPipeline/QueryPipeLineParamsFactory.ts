import { QueryParamsPipelinePart } from './QueryParamsPipelinePart';
import { HttpParams } from '@angular/common/http';

export class QueryPipeLineParamsFactory {
  private readonly _pipeLines: QueryParamsPipelinePart[];
  constructor(pipeLines: QueryParamsPipelinePart[]) {
    this._pipeLines = pipeLines;
  }

  public with(pipe: QueryParamsPipelinePart): QueryPipeLineParamsFactory {
    const nextPipeLines: QueryParamsPipelinePart[] = [pipe, ...this._pipeLines];
    return new QueryPipeLineParamsFactory(nextPipeLines);
  }

  public reset(): QueryPipeLineParamsFactory {
    return new QueryPipeLineParamsFactory([]);
  }

  public makeParams(): HttpParams {
    let params: HttpParams = new HttpParams();
    for (const pipe of this._pipeLines) {
      params = pipe.add(params);
    }
    return params;
  }
}
