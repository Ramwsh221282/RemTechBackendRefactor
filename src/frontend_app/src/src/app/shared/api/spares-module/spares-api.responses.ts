export type SpareResponse = {
  Id: string;
  Url: string;
  Price: number;
  Oem: string;
  Text: string;
  Type: string;
  IsNds: boolean;
  Location: string;
};

export type GetSparesQueryResponse = {
  TotalCount: number;
  AveragePrice: number;
  MinimalPrice: number;
  MaximalPrice: number;
  Spares: SpareResponse[];
};
