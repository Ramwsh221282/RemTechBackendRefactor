import { GetVehiclesQueryParameters } from './vehicles-api.requests';

export const DefaultGetVehiclesQueryParameters =
  (): GetVehiclesQueryParameters => {
    return {
      BrandId: '',
      CategoryId: '',
      RegionId: '',
      ModelId: '',
      IsNds: null,
      MinimalPrice: null,
      MaximalPrice: null,
      SortFields: null,
      Sort: null,
      Page: 1,
      PageSize: 20,
      TextSearch: '',
      Characteristics: null,
    };
  };

export const GetVehiclesQueryParametersWithBrand = (
  id: string,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return {
    ...params,
    BrandId: id,
  };
};

export const GetVehiclesQueryParametersWithCategory = (
  id: string,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return {
    ...params,
    CategoryId: id,
  };
};

export const GetVehiclesQueryParametersWithModel = (
  id: string,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return {
    ...params,
    ModelId: id,
  };
};

export const GetVehiclesQueryParametersWithRegion = (
  id: string,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return {
    ...params,
    RegionId: id,
  };
};

export const GetVehiclesQueryParametersWithCharacteristic = (
  key: string,
  value: string,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  if (!params.Characteristics)
    return GetVehiclesQueryParametersWithCharacteristics(
      { [key]: value },
      params,
    );
  if (params.Characteristics[key] === value) return params;
  return GetVehiclesQueryParametersWithCharacteristics(
    {
      ...params.Characteristics,
      [key]: value,
    },
    params,
  );
};

export const GetVehiclesQueryParametersWithCharacteristics = (
  characteristics: { [key: string]: string },
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return {
    ...params,
    Characteristics: characteristics,
  };
};

export const GetVehiclesQueryParametersWithNds = (
  isNds: boolean,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return {
    ...params,
    IsNds: isNds,
  };
};

export const GetVehiclesQueryParametersWithoutCharacteristic = (
  key: string,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  if (!params.Characteristics) return params;
  if (!params.Characteristics[key]) return params;
  const updatedCharacteristics: { [key: string]: string } = {
    ...params.Characteristics,
  };
  delete updatedCharacteristics[key];
  return {
    ...params,
    Characteristics: updatedCharacteristics,
  };
};

export const GetVehiclesQueryParametersWithMinPrice = (
  price: number,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return { ...params, MinimalPrice: price };
};

export const GetVehiclesQueryParametersWithMaxPrice = (
  price: number,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return { ...params, MaximalPrice: price };
};

export const GetVehiclesQueryParametersWithSortField = (
  field: string,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  if (!params.SortFields)
    return GetVehiclesQueryParametersWithSortFields([field], params);
  if (params.SortFields.includes(field)) return params;
  return GetVehiclesQueryParametersWithSortFields(
    [...params.SortFields, field],
    params,
  );
};

export const GetVehiclesQueryParametersWithPage = (
  page: number,
  size: number,
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return { ...params, Page: page, PageSize: size };
};

export const GetVehiclesQueryParametersWithSortOrder = (
  order: 'asc' | 'desc',
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return { ...params, Sort: order };
};

export const GetVehiclesQueryParametersWithTextSearch = (
  text: string,
): GetVehiclesQueryParameters => {
  return { ...DefaultGetVehiclesQueryParameters(), TextSearch: text };
};

export const GetVehiclesQueryParametersWithSortFields = (
  fields: string[],
  params: GetVehiclesQueryParameters,
): GetVehiclesQueryParameters => {
  return { ...params, SortFields: fields };
};
