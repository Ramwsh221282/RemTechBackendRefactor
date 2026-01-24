import { HttpParams } from '@angular/common/http';
import { CategoryResponse } from '../categories-module/categories-responses';
import { BrandResponse } from './brands-api.responses';

type GetBrandsQueryParameters = {
	id: string | null | undefined;
	name: string | null | undefined;
	categoryId: string | null | undefined;
	categoryName: string | null | undefined;
	modelId: string | null | undefined;
	modelName: string | null | undefined;
	page: number | null | undefined;
	pageSize: number | null | undefined;
	textSearch: string | null | undefined;
	useVehiclesCount: boolean;
	useBrandsCount: boolean;
};

export class GetBrandsQuery {
	private constructor(private readonly parameters: GetBrandsQueryParameters) {}
	public static default(): GetBrandsQuery {
		return new GetBrandsQuery({
			id: undefined,
			categoryId: undefined,
			categoryName: undefined,
			modelId: undefined,
			modelName: undefined,
			name: undefined,
			page: undefined,
			pageSize: undefined,
			textSearch: undefined,
			useBrandsCount: false,
			useVehiclesCount: false,
		});
	}

	public toHttpParams(): HttpParams {
		let params: HttpParams = new HttpParams();
		const includes: string[] = [];
		if (this.parameters.id) params = params.append('id', this.parameters.id);
		if (this.parameters.name) params = params.append('name', this.parameters.name);
		if (this.parameters.categoryId) params = params.append('categoryId', this.parameters.categoryId);
		if (this.parameters.categoryName) params = params.append('categoryName', this.parameters.categoryName);
		if (this.parameters.modelId) params = params.append('modelId', this.parameters.modelId);
		if (this.parameters.modelName) params = params.append('modelName', this.parameters.modelName);
		if (this.parameters.page) params = params.append('page', this.parameters.page.toString());
		if (this.parameters.pageSize) params = params.append('pageSize', this.parameters.pageSize.toString());
		if (this.parameters.useVehiclesCount) params = params.append('include', 'vehicles-count');
		if (this.parameters.useBrandsCount) params = params.append('include', 'brands-count');
		if (this.parameters.textSearch) {
			params = params.append('text-search', this.parameters.textSearch);
			params = params.append('include', 'text-search-score');
		}
		if (includes) {
			for (const include of includes) {
				params = params.append('include', include);
			}
		}
		return params;
	}

	public useModelId(id: string | null | undefined = undefined, model: BrandResponse | null | undefined = undefined): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			modelId: id ?? model?.Id ?? undefined,
		});
	}

	public useVehiclesCount(use: boolean): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			useVehiclesCount: use,
		});
	}

	public useBrandsCount(use: boolean): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			useBrandsCount: use,
		});
	}

	public useTextSearch(text: string | null | undefined = undefined): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			textSearch: text,
		});
	}

	public usePageSize(size: number | null | undefined = undefined): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			pageSize: size,
		});
	}

	public usePage(page: number | null | undefined = undefined): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			page: page,
		});
	}

	public useModelName(name: string | null | undefined = undefined, model: BrandResponse | null | undefined = undefined): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			modelName: name ?? model?.Name ?? undefined,
		});
	}

	public useCategoryName(
		name: string | null | undefined = undefined,
		category: CategoryResponse | null | undefined = undefined,
	): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			categoryName: name ?? category?.Name ?? undefined,
		});
	}

	public useCategoryId(
		id: string | null | undefined = undefined,
		category: CategoryResponse | null | undefined = undefined,
	): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			categoryId: id ?? category?.Id ?? undefined,
		});
	}

	public useBrandId(id: string | null | undefined = undefined, brand: BrandResponse | null | undefined = undefined): GetBrandsQuery {
		return new GetBrandsQuery({
			...this.parameters,
			id: id ?? brand?.Id ?? undefined,
		});
	}
}
