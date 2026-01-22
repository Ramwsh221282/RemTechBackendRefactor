import { GetLocationsQueryParameters } from './locations.requests';

export const DefaultLocationQuery: () => GetLocationsQueryParameters =
  (): GetLocationsQueryParameters => {
    return {
      BrandId: undefined,
      BrandName: undefined,
      CategoryId: undefined,
      CategoryName: undefined,
      Id: undefined,
      Includes: undefined,
      ModelId: undefined,
      ModelName: undefined,
      Page: undefined,
      TextSearch: undefined,
    };
  };

export const LocationsQueryWithTextSearch: (
  input: string | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  input: string | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, TextSearch: input };
};

export const LocationQueryWithBrandId: (
  brandId: string | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  brandId: string | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, BrandId: brandId };
};

export const LocationQueryWithCategoryId: (
  categoryId: string | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  categoryId: string | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, CategoryId: categoryId };
};

export const LocationQueryWithModelId: (
  modelId: string | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  modelId: string | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, ModelId: modelId };
};

export const LocationsQueryWithBrandName: (
  brandName: string | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  brandName: string | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, BrandName: brandName };
};

export const LocationsQueryWithCategoryName: (
  categoryName: string | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  categoryName: string | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, CategoryName: categoryName };
};

export const LocationsQueryWithModelName: (
  modelName: string | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  modelName: string | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, ModelName: modelName };
};

export const LocationsQueryWithPage: (
  page: number | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  page: number | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, Page: page };
};

export const LocationsQueryWithId: (
  id: string | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  id: string | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, Id: id };
};

export const LocationsQueryWithIncludes: (
  includes: string[] | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  includes: string[] | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  return { ...query, Includes: includes };
};

export const LocationsQueryWithSingleInclude: (
  include: string | null | undefined,
  query: GetLocationsQueryParameters,
) => GetLocationsQueryParameters = (
  include: string | null | undefined,
  query: GetLocationsQueryParameters,
): GetLocationsQueryParameters => {
  if (!include) return query;

  if (!query.Includes) {
    const includes: string[] = [include];
    return LocationsQueryWithIncludes(includes, query);
  }

  const includes: string[] = [...query.Includes, include];
  return LocationsQueryWithIncludes(includes, query);
};
