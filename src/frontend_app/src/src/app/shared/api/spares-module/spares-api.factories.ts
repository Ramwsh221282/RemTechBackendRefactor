import { GetSparesQueryParameters } from './spares-api.requests';
import { GetSparesQueryResponse, SpareLocationResponse, SpareResponse } from './spares-api.responses';

export function DefaultGetSparesQueryResponse(): GetSparesQueryResponse {
	return {
		AveragePrice: 0,
		TotalCount: 0,
		Spares: [],
		MaximalPrice: 0,
		MinimalPrice: 0,
	};
}

export function DefaultSpareResponse(): SpareResponse {
	return {
		Id: '',
		Url: '',
		Price: 0,
		Oem: '',
		Text: '',
		Type: '',
		IsNds: false,
		Location: '',
	};
}

export function DefaultGetSpareParameters(): GetSparesQueryParameters {
	return {
		MaximalPrice: null,
		MinimalPrice: null,
		Page: 1,
		PageSize: 20,
		RegionId: null,
		TextSearch: '',
		OrderMode: null,
		Oem: null,
		Type: null,
	};
}

export function GetSpareParametersWithOem(oem: string, params: GetSparesQueryParameters): GetSparesQueryParameters {
	return { ...params, Oem: oem };
}

export function GetSpareParametersWithRegion(regionId: string, params: GetSparesQueryParameters): GetSparesQueryParameters {
	return { ...params, RegionId: regionId };
}

export function GetSpareParametersWithMaxPrice(price: number, params: GetSparesQueryParameters): GetSparesQueryParameters {
	return { MaximalPrice: price, ...params };
}

export function GetSpareParametersWithMinPrice(price: number, params: GetSparesQueryParameters): GetSparesQueryParameters {
	return { ...params, MinimalPrice: price };
}

export function GetSpareParametersWithPage(page: number, size: number, params: GetSparesQueryParameters): GetSparesQueryParameters {
	return { ...params, Page: page, PageSize: size };
}

export function GetSpareParametersWithTextSearch(textSearch: string, params: GetSparesQueryParameters): GetSparesQueryParameters {
	return { ...params, TextSearch: textSearch };
}

export function GetSpareParametersWithOrderMode(orderMode: string, params: GetSparesQueryParameters): GetSparesQueryParameters {
	return { ...params, OrderMode: orderMode };
}

export function DefaultSpareLocationResponse(): SpareLocationResponse {
	return { Id: '', Name: '', TextSearchScore: 0, VehiclesCount: 0 };
}
