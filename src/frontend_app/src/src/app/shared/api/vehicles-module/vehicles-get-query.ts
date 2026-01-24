import { HttpParams } from '@angular/common/http';
import { BrandResponse } from '../brands-module/brands-api.responses';
import { CategoryResponse } from '../categories-module/categories-responses';
import { LocationResponse } from '../locations-module/locations.responses';
import { ModelResponse } from '../models-module/models-responses';

export type GetVehiclesQueryParameters = {
	brandId: string | null | undefined;
	categoryId: string | null | undefined;
	regionId: string | null | undefined;
	modelId: string | null | undefined;
	isNds: boolean | null | undefined;
	minimalPrice: number | null | undefined;
	maximalPrice: number | null | undefined;
	sortFields: string[] | null | undefined;
	sort: string | null | undefined;
	page: number | null | undefined;
	pageSize: number | null | undefined;
	textSearch: string | null | undefined;
};

export class GetVehiclesQuery {
	private constructor(private readonly _parameters: GetVehiclesQueryParameters) {}

	public toHttpParams(): HttpParams {
		let params: HttpParams = new HttpParams();

		if (this._parameters.brandId) params = params.append('brand', this._parameters.brandId);
		if (this._parameters.categoryId) params = params.append('category', this._parameters.categoryId);
		if (this._parameters.regionId) params = params.append('region', this._parameters.regionId);
		if (this._parameters.modelId) params = params.append('model', this._parameters.modelId);
		if (this._parameters.isNds) params = params.append('nds', this._parameters.isNds.toString());
		if (this._parameters.minimalPrice) params = params.append('price-min', this._parameters.minimalPrice.toString());
		if (this._parameters.maximalPrice) params = params.append('price-max', this._parameters.maximalPrice.toString());
		if (this._parameters.sort) params = params.append('sort', this._parameters.sort);
		if (this._parameters.sortFields) {
			this._parameters.sortFields.forEach((field: string) => {
				params = params.append('sort-fields', field);
			});
		}

		if (this._parameters.page) params = params.append('page', this._parameters.page.toString());
		if (this._parameters.pageSize) params = params.append('page-size', this._parameters.pageSize.toString());
		if (this._parameters.textSearch) params = params.append('text-search', this._parameters.textSearch);

		return params;
	}

	public useBrand(brand: BrandResponse | null | undefined = undefined, brandId: string | null | undefined = undefined): GetVehiclesQuery {
		if (brand) return new GetVehiclesQuery({ ...this._parameters, brandId: brand.Id });
		if (brandId) return new GetVehiclesQuery({ ...this._parameters, brandId: brandId });
		return new GetVehiclesQuery({ ...this._parameters, brandId: undefined });
	}

	public useCategory(category: CategoryResponse | null | undefined = undefined, categoryId: string | null | undefined = undefined) {
		if (category) return new GetVehiclesQuery({ ...this._parameters, categoryId: category.Id });
		if (categoryId) return new GetVehiclesQuery({ ...this._parameters, categoryId: categoryId });
		return new GetVehiclesQuery({ ...this._parameters, categoryId: undefined });
	}

	public useLocation(location: LocationResponse | null | undefined = undefined, locationId: string | null | undefined = undefined) {
		if (location) return new GetVehiclesQuery({ ...this._parameters, regionId: location.Id });
		if (locationId) return new GetVehiclesQuery({ ...this._parameters, regionId: locationId });
		return new GetVehiclesQuery({ ...this._parameters, regionId: undefined });
	}

	public useNds(isNds: boolean | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this._parameters, isNds });
	}

	public useMinimalPrice(minimalPrice: number | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this._parameters, minimalPrice });
	}

	public useMaximalPrice(maximalPrice: number | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this._parameters, maximalPrice });
	}

	public useSortFields(sortFields: string[] | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this._parameters, sortFields });
	}

	public useSort(sort: string | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this._parameters, sort });
	}

	public usePage(page: number | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this._parameters, page });
	}

	public usePageSize(pageSize: number | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this._parameters, pageSize });
	}

	public useTextSearch(textSearch: string | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this._parameters, textSearch });
	}

	public useModel(model: ModelResponse | null | undefined = undefined, modelId: string | null | undefined = undefined) {
		if (model) return new GetVehiclesQuery({ ...this._parameters, modelId: model.Id });
		if (modelId) return new GetVehiclesQuery({ ...this._parameters, modelId: modelId });
		return new GetVehiclesQuery({ ...this._parameters, modelId: undefined });
	}

	public static default(): GetVehiclesQuery {
		return new GetVehiclesQuery({
			brandId: undefined,
			categoryId: undefined,
			regionId: undefined,
			modelId: undefined,
			isNds: undefined,
			minimalPrice: undefined,
			maximalPrice: undefined,
			sortFields: undefined,
			sort: undefined,
			page: undefined,
			pageSize: undefined,
			textSearch: undefined,
		});
	}
}
