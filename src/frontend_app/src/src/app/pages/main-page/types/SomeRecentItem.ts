export interface SomeRecentItem {
  id: string;
  region: string;
  regionKind: string;
  city: string;
  price: number;
  IsNds: boolean;
  sourceUrl: string;
  photos: string[];
  description: string;
}

export interface RecentSpare extends SomeRecentItem {}

export interface RecentVehicle extends SomeRecentItem {
  categoryId: string;
  brandId: string;
  modelId: string;
  regionId: string;
  categoryName: string;
  brandName: string;
  modelName: string;
}
