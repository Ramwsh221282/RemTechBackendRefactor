export type GetVehiclesQueryParameters = {
  BrandId?: string | null;
  CategoryId?: string | null;
  RegionId?: string | null;
  ModelId?: string | null;
  IsNds?: boolean | null;
  MinimalPrice?: number | null;
  MaximalPrice?: number | null;
  SortFields?: string[] | null;
  Sort?: string | null;
  Page?: number | null;
  PageSize?: number | null;
  TextSearch?: string | null;
  Characteristics?: { [key: string]: string } | null;
};
