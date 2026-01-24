import { HttpParams } from '@angular/common/http';
import { BrandResponse } from '../brands-module/brands-api.responses';
import { ModelResponse } from '../models-module/models-responses';
import { CategoryResponse } from './categories-responses';

type GetCategoriesQueryParameters = {
	id: string | null | undefined;
	name: string | null | undefined;
	brandId: string | null | undefined;
	brandName: string | null | undefined;
	modelId: string | null | undefined;
	modelName: string | null | undefined;
	page: number | null | undefined;
	pageSize: number | null | undefined;
	withVehiclesCount: boolean;
	withTotalCategoriesCount: boolean;
	textSearch: string | null | undefined;
	sortFields: string[];
	sortDirection: 'ASC' | 'DESC';
};

export class GetCategoriesQuery {
	private constructor(private readonly parameters: GetCategoriesQueryParameters) {}

	public toHttpParams(): HttpParams {
		const includes: string[] = [];
		let params: HttpParams = new HttpParams();
		if (this.parameters.id) params = params.append('id', this.parameters.id);
		if (this.parameters.name) params = params.append('name', this.parameters.name);
		if (this.parameters.brandId) params = params.append('brandId', this.parameters.brandId);
		if (this.parameters.brandName) params = params.append('brandName', this.parameters.brandName);
		if (this.parameters.modelId) params = params.append('modelId', this.parameters.modelId);
		if (this.parameters.modelName) params = params.append('modelName', this.parameters.modelName);
		if (this.parameters.page) params = params.append('page', this.parameters.page.toString());
		if (this.parameters.pageSize) params = params.append('pageSize', this.parameters.pageSize.toString());
		if (this.parameters.textSearch) {
			params = params.append('text-search', this.parameters.textSearch);
			includes.push('text-search-score');
		}
		if (this.parameters.withVehiclesCount) includes.push('vehicles-count');
		if (this.parameters.withTotalCategoriesCount) includes.push('total-categories-count');
		if (this.parameters.sortDirection) params = params.append('sort-mode', this.parameters.sortDirection);
		if (this.parameters.sortFields && this.parameters.sortFields.length > 0) {
			for (const field of this.parameters.sortFields) {
				params = params.append('sort-fields', field);
			}
		}
		if (includes.length > 0) {
			for (const include of includes) {
				params = params.append('include', include);
			}
		}
		return params;
	}

	public useCategoryId(
		id: string | null | undefined = undefined,
		category: CategoryResponse | null | undefined = undefined,
	): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			id: id ?? category?.Id ?? undefined,
		});
	}

	public useOrderByName(use: boolean): GetCategoriesQuery {
		if (this.parameters.sortFields.includes('name') === use) return new GetCategoriesQuery({ ...this.parameters });
		return new GetCategoriesQuery({
			...this.parameters,
			sortFields: ['name', ...this.parameters.sortFields],
		});
	}

	public useOrderByVehiclesCount(use: boolean): GetCategoriesQuery {
		if (this.parameters.sortFields.includes('name') === use) return new GetCategoriesQuery({ ...this.parameters });
		return new GetCategoriesQuery({
			...this.useVehiclesCount(true).parameters,
			sortFields: ['vehicles-count', ...this.parameters.sortFields],
		});
	}

	public useOrderByDirection(direction: 'ASC' | 'DESC'): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			sortDirection: direction,
		});
	}

	public useCategoryName(
		name: string | null | undefined = undefined,
		category: CategoryResponse | null | undefined = undefined,
	): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			name: name ?? category?.Name ?? undefined,
		});
	}

	public useBrandId(id: string | null | undefined = undefined, brand: BrandResponse | null | undefined = undefined): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			brandId: id ?? brand?.Id ?? undefined,
		});
	}

	public useBrandName(
		name: string | null | undefined = undefined,
		brand: BrandResponse | null | undefined = undefined,
	): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			brandName: name ?? brand?.Name ?? undefined,
		});
	}

	public useModelId(id: string | null | undefined = undefined, model: ModelResponse | null | undefined = undefined): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			modelId: id ?? model?.Id ?? undefined,
		});
	}

	public useModelName(
		name: string | null | undefined = undefined,
		model: ModelResponse | null | undefined = undefined,
	): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			modelName: name ?? model?.Name ?? undefined,
		});
	}

	public useVehiclesCount(use: boolean): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			withVehiclesCount: use,
		});
	}

	public useTotalCategoriesCount(use: boolean): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			withTotalCategoriesCount: use,
		});
	}

	public useTextSearch(text: string | null | undefined = undefined): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			textSearch: text,
		});
	}

	public usePageSize(size: number | null | undefined = undefined): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			pageSize: size,
		});
	}

	public usePage(page: number | null | undefined = undefined): GetCategoriesQuery {
		return new GetCategoriesQuery({
			...this.parameters,
			page: page,
		});
	}

	public static default(): GetCategoriesQuery {
		return new GetCategoriesQuery({
			brandId: undefined,
			brandName: undefined,
			id: undefined,
			modelId: undefined,
			modelName: undefined,
			name: undefined,
			page: undefined,
			pageSize: undefined,
			textSearch: undefined,
			withVehiclesCount: false,
			withTotalCategoriesCount: false,
			sortFields: [],
			sortDirection: 'ASC',
		});
	}
}
