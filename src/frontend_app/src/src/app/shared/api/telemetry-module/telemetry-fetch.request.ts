import { HttpParams } from '@angular/common/http';

type TelemetryFetchRequestParameters = {
	page: number | null;
	pageSize: number | null;
};

function defaultParams(): TelemetryFetchRequestParameters {
	return {
		page: null,
		pageSize: null,
	};
}

export class FetchAnalyticsTelemetryRecordsQuery {
	private constructor(private readonly _params: TelemetryFetchRequestParameters) {}

	public static create(): FetchAnalyticsTelemetryRecordsQuery {
		return new FetchAnalyticsTelemetryRecordsQuery(defaultParams());
	}

	public buildHttpParams(): HttpParams {
		let params: HttpParams = new HttpParams();
		if (!!this._params.page) params = params.append('page', this._params.page.toString());
		if (!!this._params.pageSize) params = params.append('page-size', this._params.pageSize.toString());
		return params;
	}

	public withPage(page: number | null): FetchAnalyticsTelemetryRecordsQuery {
		return new FetchAnalyticsTelemetryRecordsQuery({
			...this._params,
			page: page,
		});
	}

	public withPageSize(pageSize: number | null): FetchAnalyticsTelemetryRecordsQuery {
		return new FetchAnalyticsTelemetryRecordsQuery({
			...this._params,
			pageSize: pageSize,
		});
	}
}
