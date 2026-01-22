export type GetLocationsQueryParameters = {
  Page: number | undefined | null;
  Id: string | undefined | null;
  CategoryId: string | undefined | null;
  BrandId: string | undefined | null;
  ModelId: string | undefined | null;
  CategoryName: string | undefined | null;
  BrandName: string | undefined | null;
  ModelName: string | undefined | null;
  Includes: string[] | undefined | null;
  TextSearch: string | undefined | null;
};
