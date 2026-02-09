export type CategoriesPopularity = {
  Id: string;
  Name: string;
  Count: number;
};

export type BrandsPopularity = {
  Id: string;
  Name: string;
  Count: number;
};

export type ItemStats = {
  ItemType: string;
  Count: number;
};

export type MainPageItemStatsResponse = {
  CategoriesPopularity: CategoriesPopularity[];
  BrandsPopularity: BrandsPopularity[];
  ItemStats: ItemStats[];
};

export type SpareData = {
  Id: string;
  Title: string;
};

export type VehicleDataCharacteristics = {
  Characteristic: string;
  Value: string;
};

export type VehicleData = {
  Id: string;
  Title: string;
  Photos: string[];
  Characteristics: VehicleDataCharacteristics[];
};

export type MainPageLastAddedItem = {
  Spare: SpareData | null;
  Vehicle: VehicleData | null;
};

export type MainPageLastAddedItemsResponse = {
  Items: MainPageLastAddedItem[];
};
