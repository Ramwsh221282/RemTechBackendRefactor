import { HttpParams } from '@angular/common/http';
import { ModelResponse } from './models-responses';

type GetModelsQueryParameters = {
	id: string | null | undefined;
	name: string | null | undefined;
	brandId: string | null | undefined;
	brandName: string | null | undefined;
	categoryId: string | null | undefined;
	categoryName: string | null | undefined;
};

export class GetModelsQuery {
	private constructor(public parameters: GetModelsQueryParameters) {}

	public toHttpParams(): HttpParams {
		let params: HttpParams = new HttpParams();
		if (this.parameters.id) params = params.append('id', this.parameters.id);
		if (this.parameters.name) params = params.append('name', this.parameters.name);
		if (this.parameters.brandId) params = params.append('brandId', this.parameters.brandId);
		if (this.parameters.brandName) params = params.append('brandName', this.parameters.brandName);
		if (this.parameters.categoryId) params = params.append('categoryId', this.parameters.categoryId);
		if (this.parameters.categoryName) params = params.append('categoryName', this.parameters.categoryName);
		return params;
	}

	public useBrandId(brandId: string | null | undefined = undefined, brand: ModelResponse | null | undefined = undefined): GetModelsQuery {
		return new GetModelsQuery({
			...this.parameters,
			brandId: brandId ?? brand?.Id ?? undefined,
		});
	}

	public useCategoryId(
		categoryId: string | null | undefined = undefined,
		category: ModelResponse | null | undefined = undefined,
	): GetModelsQuery {
		return new GetModelsQuery({ ...this.parameters, categoryId: categoryId ?? category?.Id ?? undefined });
	}

	public useId(modelId: string | null | undefined = undefined, model: ModelResponse | null | undefined = undefined): GetModelsQuery {
		const params: GetModelsQueryParameters = { ...this.parameters, id: modelId ?? model?.Id ?? undefined };
		return new GetModelsQuery(params);
	}

	public useBrandName(
		brandName: string | null | undefined = undefined,
		brand: ModelResponse | null | undefined = undefined,
	): GetModelsQuery {
		return new GetModelsQuery({
			...this.parameters,
			brandName: brandName ?? brand?.Name ?? undefined,
		});
	}

	public useCategoryName(
		categoryName: string | null | undefined = undefined,
		category: ModelResponse | null | undefined = undefined,
	): GetModelsQuery {
		return new GetModelsQuery({ ...this.parameters, categoryName: categoryName ?? category?.Name ?? undefined });
	}

	public useName(name: string | null | undefined = undefined, model: ModelResponse | null | undefined = undefined): GetModelsQuery {
		return new GetModelsQuery({
			...this.parameters,
			name: name ?? model?.Name ?? undefined,
		});
	}

	public static default(): GetModelsQuery {
		return new GetModelsQuery({
			id: undefined,
			name: undefined,
			brandId: undefined,
			categoryId: undefined,
			brandName: undefined,
			categoryName: undefined,
		});
	}
}
