export type CategoryResponse = {
  Name: string;
  Id: string;
  VehiclesCount?: number | null;
  TextSearchScore?: number | null;
};
