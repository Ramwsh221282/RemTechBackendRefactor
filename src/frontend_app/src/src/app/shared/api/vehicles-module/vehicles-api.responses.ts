export type VehicleCharacteristicsResponse = {
  Name: string;
  Value: string;
};

export type VehicleResponse = {
  VehicleId: string;
  BrandId: string;
  BrandName: string;
  CategoryId: string;
  CategoryName: string;
  ModelId: string;
  ModelName: string;
  RegionId: string;
  RegionName: string;
  Source: string;
  Price: number;
  IsNds: boolean;
  Text: string;
  ReleaseYear?: number | null;
  Photos: string[];
  Characteristics: VehicleCharacteristicsResponse[];
};

export type GetVehiclesQueryResponse = {
  TotalCount: number;
  AveragePrice: number;
  MinimalPrice: number;
  MaximalPrice: number;
  Vehicles: VehicleResponse[];
};
