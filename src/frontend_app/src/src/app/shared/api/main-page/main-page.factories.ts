import {
  CategoriesPopularity,
  ItemStats,
  MainPageItemStatsResponse,
  MainPageLastAddedItemsResponse,
  SpareData,
  VehicleData,
  VehicleDataCharacteristics,
} from './main-page-responses';

export const DefaultMainPageItemStatsResponse =
  (): MainPageItemStatsResponse => {
    return { BrandsPopularity: [], CategoriesPopularity: [], ItemStats: [] };
  };

export const DefaultMainPageLastAddedItemsResponse =
  (): MainPageLastAddedItemsResponse => {
    return { Items: [] };
  };

export const DefaultCategoriesPopularity = (): CategoriesPopularity => {
  return { Name: '', Count: 0 };
};

export const DefaultBrandsPopularity = (): CategoriesPopularity => {
  return { Name: '', Count: 0 };
};

export const DefaultItemStats = (): ItemStats => {
  return { ItemType: '', Count: 0 };
};

export const DefaultSpareData = (): SpareData => {
  return { Id: '', Title: '' };
};

export const DefaultVehicleDataCharacteristics =
  (): VehicleDataCharacteristics => {
    return { Characteristic: '', Value: '' };
  };

export const DefaultVehicleData = (): VehicleData => {
  return { Id: '', Title: '', Photos: [], Characteristics: [] };
};
