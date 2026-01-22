import { GetLocationsQueryParameters } from './locations.requests';

export const LocationsQueryIncludes: (
  include: string,
  query: GetLocationsQueryParameters,
) => boolean = (include: string, query: GetLocationsQueryParameters) => {
  if (!query.Includes) return false;
  return query.Includes.includes(include);
};
