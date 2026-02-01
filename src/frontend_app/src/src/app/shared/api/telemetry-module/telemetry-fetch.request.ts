import { HttpParams } from '@angular/common/http';
import { SortEvent } from 'primeng/api';
import { StringUtils } from '../../utils/string-utils';

type QueryParameters = {
	page: number | null;
	pageSize: number | null;
	permissions: string[] | null;
	sort: SortClause[] | null;
	login: string | null;
	email: string | null;
	status: string | null;
};

export interface SortClause extends SortEvent {}

function defaultParams(): QueryParameters {
	return {
		page: null,
		pageSize: null,
		permissions: null,
		sort: null,
		login: null,
		email: null,
		status: null,
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

		if (!!this._params.login && !StringUtils.isEmptyOrWhiteSpace(this._params.login))
			params = params.append('login', this._params.login);

		if (!!this._params.email && !StringUtils.isEmptyOrWhiteSpace(this._params.email))
			params = params.append('email', this._params.email);

		if (!!this._params.status && !StringUtils.isEmptyOrWhiteSpace(this._params.status))
			params = params.append('status', this._params.status);

		if (!!this._params.permissions && this._params.permissions.length > 0) {
			for (const permission of this._params.permissions) {
				params = params.append('permissions', permission);
			}
		}

		if (!!this._params.sort && this._params.sort.length > 0) {
			for (const sortClause of this._params.sort) {
				params = params.append('sort', `${sortClause.field}:${sortClause.mode}`);
			}
		}

		return params;
	}

	public addSort(sortClause: SortClause): ActionRecordsQuery {
		const currentSorts: SortClause[] = this._params.sort ? this._params.sort : [];
		const index: number = currentSorts.findIndex((s) => s.field === sortClause.field);
		if (index !== -1) {
			currentSorts[index] = sortClause;
		} else {
			currentSorts.push(sortClause);
		}
		return new ActionRecordsQuery({ ...this._params, sort: currentSorts.filter((s: SortClause) => s.mode !== 'NONE') });
	}

	public withStatus(status: string | null): ActionRecordsQuery {
		return new ActionRecordsQuery({ ...this._params, status: status });
	}

	public withLogin(login: string | null): ActionRecordsQuery {
		return new ActionRecordsQuery({ ...this._params, login: login });
	}

	public withEmail(email: string | null): ActionRecordsQuery {
		return new ActionRecordsQuery({ ...this._params, email: email });
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
