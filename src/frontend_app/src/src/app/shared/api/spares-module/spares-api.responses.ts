import { LocationResponse } from '../locations-module/locations.responses';

export type SpareResponse = {
	Id: string;
	Url: string;
	Price: number;
	Oem: string;
	Text: string;
	Type: string;
	IsNds: boolean;
	Location: string;
  Photos: string[]
};

export type GetSparesQueryResponse = {
	TotalCount: number;
	AveragePrice: number;
	MinimalPrice: number;
	MaximalPrice: number;
	Spares: SpareResponse[];
};

export type SpareTypeResponse = {
	Value: SpareTypeName;
};

export type SpareTypeName = string;

export type SpareLocationResponse = LocationResponse;
