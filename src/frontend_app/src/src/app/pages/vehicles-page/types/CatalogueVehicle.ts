export interface CatalogueVehicle {
  id: string;
  categoryId: string;
  brandId: string;
  modelId: string;
  regionId: string;
  price: number;
  isNds: boolean;
  sourceUrl: string;
  categoryName: string;
  brandName: string;
  modelName: string;
  city: string;
  region: string;
  regionKind: string;
  photos: string[];
  description: string;
  characteristics: CatalogueVehicleCtx[];
}

export interface CatalogueVehicleCtx {
  name: string;
  value: string;
  measure: string;
}
