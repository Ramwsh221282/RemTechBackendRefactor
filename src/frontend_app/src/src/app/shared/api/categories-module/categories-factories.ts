import { CategoryResponse } from './categories-responses';

export const DefaultCategory = (): CategoryResponse => {
  return { Id: '', Name: '', VehiclesCount: null };
};
