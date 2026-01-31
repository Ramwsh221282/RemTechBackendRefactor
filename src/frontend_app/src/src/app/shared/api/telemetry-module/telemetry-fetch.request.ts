import { HttpParams } from '@angular/common/http';

type QueryParameters = {
	page: number | null;
	pageSize: number | null;
	permissions: string[] | null;
};

function defaultParams(): QueryParameters {
	return {
		page: null,
		pageSize: null,
		permissions: null,
	};
}

export class ActionRecordsQuery {
	private constructor(private readonly _params: QueryParameters) {}

	public static create(): ActionRecordsQuery {
		return new ActionRecordsQuery(defaultParams());
	}

	public buildHttpParams(): HttpParams {
		let params: HttpParams = new HttpParams();
		if (!!this._params.page) params = params.append('page', this._params.page.toString());
		if (!!this._params.pageSize) params = params.append('page-size', this._params.pageSize.toString());
		if (!!this._params.permissions && this._params.permissions.length > 0) {
			for (const permission of this._params.permissions) {
				params = params.append('permissions', permission);
			}
		}
		return params;
	}

	public withPermissions(permissions: string[] | null): ActionRecordsQuery {
		if (!!permissions) return new ActionRecordsQuery({ ...this._params, permissions: permissions });
		return new ActionRecordsQuery({ ...this._params, permissions: length === 0 ? null : permissions });
	}

	public withPage(page: number | null): ActionRecordsQuery {
		return new ActionRecordsQuery({
			...this._params,
			page: page,
		});
	}

	public withPageSize(pageSize: number | null): ActionRecordsQuery {
		return new ActionRecordsQuery({
			...this._params,
			pageSize: pageSize,
		});
	}
}
