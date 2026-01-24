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
	private constructor(public readonly parameters: GetVehiclesQueryParameters) {}

	public toHttpParams(): HttpParams {
		let params: HttpParams = new HttpParams();

		if (this.parameters.brandId) params = params.append('brand', this.parameters.brandId);
		if (this.parameters.categoryId) params = params.append('category', this.parameters.categoryId);
		if (this.parameters.regionId) params = params.append('region', this.parameters.regionId);
		if (this.parameters.modelId) params = params.append('model', this.parameters.modelId);
		if (this.parameters.isNds) params = params.append('nds', this.parameters.isNds.toString());
		if (this.parameters.minimalPrice) params = params.append('price-min', this.parameters.minimalPrice.toString());
		if (this.parameters.maximalPrice) params = params.append('price-max', this.parameters.maximalPrice.toString());
		if (this.parameters.sort) params = params.append('sort', this.parameters.sort);
		if (this.parameters.sortFields) {
			this.parameters.sortFields.forEach((field: string) => {
				params = params.append('sort-fields', field);
			});
		}

		if (this.parameters.page) params = params.append('page', this.parameters.page.toString());
		if (this.parameters.pageSize) params = params.append('page-size', this.parameters.pageSize.toString());
		if (this.parameters.textSearch) params = params.append('text-search', this.parameters.textSearch);

		return params;
	}

	public useBrand(brand: BrandResponse | null | undefined = undefined, brandId: string | null | undefined = undefined): GetVehiclesQuery {
		if (brand) return new GetVehiclesQuery({ ...this.parameters, brandId: brand.Id });
		if (brandId) return new GetVehiclesQuery({ ...this.parameters, brandId: brandId });
		return new GetVehiclesQuery({ ...this.parameters, brandId: undefined });
	}

	public useCategory(category: CategoryResponse | null | undefined = undefined, categoryId: string | null | undefined = undefined) {
		if (category) return new GetVehiclesQuery({ ...this.parameters, categoryId: category.Id });
		if (categoryId) return new GetVehiclesQuery({ ...this.parameters, categoryId: categoryId });
		return new GetVehiclesQuery({ ...this.parameters, categoryId: undefined });
	}

	public useLocation(location: LocationResponse | null | undefined = undefined, locationId: string | null | undefined = undefined) {
		if (location) return new GetVehiclesQuery({ ...this.parameters, regionId: location.Id });
		if (locationId) return new GetVehiclesQuery({ ...this.parameters, regionId: locationId });
		return new GetVehiclesQuery({ ...this.parameters, regionId: undefined });
	}

	public useNds(isNds: boolean | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this.parameters, isNds });
	}

	public useMinimalPrice(minimalPrice: number | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this.parameters, minimalPrice });
	}

	public useMaximalPrice(maximalPrice: number | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this.parameters, maximalPrice });
	}

	public useSortFields(sortFields: string[] | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this.parameters, sortFields });
	}

	public useSort(sort: string | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this.parameters, sort });
	}

	public usePage(page: number | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this.parameters, page });
	}

	public usePageSize(pageSize: number | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this.parameters, pageSize });
	}

	public useTextSearch(textSearch: string | null | undefined): GetVehiclesQuery {
		return new GetVehiclesQuery({ ...this.parameters, textSearch });
	}

	public useModel(model: ModelResponse | null | undefined = undefined, modelId: string | null | undefined = undefined) {
		if (model) return new GetVehiclesQuery({ ...this.parameters, modelId: model.Id });
		if (modelId) return new GetVehiclesQuery({ ...this.parameters, modelId: modelId });
		return new GetVehiclesQuery({ ...this.parameters, modelId: undefined });
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
