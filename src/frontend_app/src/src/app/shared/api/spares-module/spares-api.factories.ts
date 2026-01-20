import { GetSparesQueryParameters } from './spares-api.requests';
import { SpareResponse } from './spares-api.responses';

export const DefaultSpareResponse = (): SpareResponse => {
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
};

export const DefaultGetSpareParameters = (): GetSparesQueryParameters => {
  return {
    MaximalPrice: null,
    MinimalPrice: null,
    Page: 1,
    PageSize: 20,
    RegionId: null,
    TextSearch: '',
    OrderMode: null,
    Oem: null,
  };
};

export const GetSpareParametersWithOem = (
  oem: string,
  params: GetSparesQueryParameters,
): GetSparesQueryParameters => {
  return { ...params, Oem: oem };
};

export const GetSpareParametersWithRegion = (
  regionId: string,
  params: GetSparesQueryParameters,
): GetSparesQueryParameters => {
  return { ...params, RegionId: regionId };
};

export const GetSpareParametersWithMaxPrice = (
  price: number,
  params: GetSparesQueryParameters,
): GetSparesQueryParameters => {
  return { MaximalPrice: price, ...params };
};

export const GetSpareParametersWithMinPrice = (
  price: number,
  params: GetSparesQueryParameters,
): GetSparesQueryParameters => {
  return { ...params, MinimalPrice: price };
};

export const GetSpareParametersWithPage = (
  page: number,
  size: number,
  params: GetSparesQueryParameters,
): GetSparesQueryParameters => {
  return { ...params, Page: page, PageSize: size };
};

export const GetSpareParametersWithTextSearch = (
  textSearch: string,
  params: GetSparesQueryParameters,
): GetSparesQueryParameters => {
  return { ...params, TextSearch: textSearch };
};

export const GetSpareParametersWithOrderMode = (
  orderMode: string,
  params: GetSparesQueryParameters,
): GetSparesQueryParameters => {
  return { ...params, OrderMode: orderMode };
};
