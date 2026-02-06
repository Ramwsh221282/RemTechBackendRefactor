import { VehicleResponse } from './vehicles-api.responses';

export const DefaultVehicleResponse = (): VehicleResponse => {
	return {
		BrandId: '',
		BrandName: '',
		CategoryId: '',
		CategoryName: '',
		Characteristics: [],
		IsNds: false,
		ModelId: '',
		ModelName: '',
		Photos: [],
		Price: 0,
		RegionId: '',
		RegionName: '',
		ReleaseYear: null,
		Source: '',
		Text: '',
		VehicleId: '',
	};
};
