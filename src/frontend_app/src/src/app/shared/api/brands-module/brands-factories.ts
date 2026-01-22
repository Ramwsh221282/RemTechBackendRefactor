import { BrandResponse } from './brands-api.responses';

export const DefaultBrand = (): BrandResponse => {
  return {
    Id: '',
    Name: '',
    VehiclesCount: null,
    TextSearchScore: null,
    TotalCount: null,
  };
};
