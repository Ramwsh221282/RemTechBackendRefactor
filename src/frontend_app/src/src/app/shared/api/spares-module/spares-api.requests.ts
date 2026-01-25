import { HttpParams } from '@angular/common/http';
import { StringUtils } from '../../utils/string-utils';

export type GetSparesQueryParameters = {
	RegionId?: string | null | undefined;
	MinimalPrice?: number | null | undefined;
	MaximalPrice?: number | null | undefined;
	TextSearch?: string | null | undefined;
	Page?: number | null | undefined;
	PageSize?: number | null | undefined;
	OrderMode?: string | null | undefined;
	Oem?: string | null | undefined;
};

type GetSpareLocationsQueryParameters = {
	TextSearch: string | null | undefined;
	Amount: number | null | undefined;
};

export class GetSpareLocationsQuery {
	private constructor(readonly params: GetSpareLocationsQueryParameters) {}

	public useTextSearch(textSearch: string | null | undefined): GetSpareLocationsQuery {
		if (!textSearch) return new GetSpareLocationsQuery({ ...this.params, TextSearch: undefined });
		if (StringUtils.isEmptyOrWhiteSpace(textSearch)) return new GetSpareLocationsQuery({ ...this.params, TextSearch: undefined });
		return new GetSpareLocationsQuery({ ...this.params, TextSearch: textSearch.trim() });
	}

	public useAmount(amount: number | null | undefined): GetSpareLocationsQuery {
		if (!amount) return new GetSpareLocationsQuery({ ...this.params, Amount: undefined });
		if (amount <= 0) return new GetSpareLocationsQuery({ ...this.params, Amount: undefined });
		return new GetSpareLocationsQuery({ ...this.params, Amount: amount });
	}

	public static create(): GetSpareLocationsQuery {
		return new GetSpareLocationsQuery({ TextSearch: null, Amount: null });
	}
}

export function ConvertSpareLocationsQueryToHttpParams(query: GetSpareLocationsQuery): HttpParams {
	let queryParams: GetSpareLocationsQueryParameters = query.params;
	let params: HttpParams = new HttpParams();
	if (queryParams.TextSearch) params = params.append('text-search', queryParams.TextSearch);
	if (queryParams.Amount) params = params.append('amount', queryParams.Amount.toString());
	return params;
}

export function ConvertSparesQueryToHttpParams(query: GetSparesQueryParameters): HttpParams {
	let params: HttpParams = new HttpParams();
	if (query.RegionId) params = params.append('region-id', query.RegionId);
	if (query.MinimalPrice) params = params.append('price-min', query.MinimalPrice.toString());
	if (query.MaximalPrice) params = params.append('price-max', query.MaximalPrice.toString());
	if (query.TextSearch) params = params.append('text-search', query.TextSearch);
	if (query.Page) params = params.append('page', query.Page.toString());
	if (query.PageSize) params = params.append('page-size', query.PageSize.toString());
	if (query.OrderMode) params = params.append('sort-mode', query.OrderMode);
	if (query.Oem) params = params.append('oem', query.Oem);
	return params;
}
