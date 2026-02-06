import { HttpParams } from '@angular/common/http';
import { LocationResponse } from './locations.responses';
import { CategoryResponse } from '../categories-module/categories-responses';
import { BrandResponse } from '../brands-module/brands-api.responses';
import { ModelResponse } from '../models-module/models-responses';

type GetLocationsQueryParameters = {
	Amount: number | undefined | null;
	Id: string | undefined | null;
	CategoryId: string | undefined | null;
	BrandId: string | undefined | null;
	ModelId: string | undefined | null;
	CategoryName: string | undefined | null;
	BrandName: string | undefined | null;
	ModelName: string | undefined | null;
	Includes: string[] | undefined | null;
	TextSearch: string | undefined | null;
	OrderByName: boolean | undefined | null;
};

export class GetLocationsQuery {
	private constructor(private readonly params: GetLocationsQueryParameters) {}

	public useAmount(amount: number | null | undefined): GetLocationsQuery {
		return new GetLocationsQuery({
			...this.params,
			Amount: amount,
		});
	}

	public toHttpParams(): HttpParams {
		let params: HttpParams = new HttpParams();
		const includes: string[] = [];

		if (this.params.Id) {
			params = params.append('id', this.params.Id);
		}

		if (this.params.CategoryId) {
			params = params.append('category-id', this.params.CategoryId);
		}

		if (this.params.BrandId) {
			params = params.append('brand-id', this.params.BrandId);
		}

		if (this.params.ModelId) {
			params = params.append('model-id', this.params.ModelId);
		}

		if (this.params.CategoryName) {
			params = params.append('category-name', this.params.CategoryName);
		}

		if (this.params.BrandName) {
			params = params.append('brand-name', this.params.BrandName);
		}

		if (this.params.ModelName) {
			params = params.append('model-name', this.params.ModelName);
		}

		if (this.params.Amount) {
			params = params.append('amount', this.params.Amount.toString());
		}

		if (this.params.OrderByName) {
			params = params.append('order-by-name', this.params.OrderByName.toString());
		}

		if (this.params.TextSearch) {
			params = params.append('text-search', this.params.TextSearch);
			includes.push('text-search-score');
		}

		if (this.containsInclude('vehicles-count')) {
			includes.push('vehicles-count');
		}

		if (includes.length > 0) {
			for (const include of includes) {
				params = params.append('include', include);
			}
		}

		return params;
	}

	public useId(id: string | null | undefined = undefined, location: LocationResponse | null | undefined = undefined): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, Id: id ?? location?.Id ?? undefined });
	}

	public useCategoryId(
		categoryId: string | null | undefined = undefined,
		category: CategoryResponse | null | undefined = undefined,
	): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, CategoryId: categoryId ?? category?.Id ?? undefined });
	}

	public useBrandId(
		brandId: string | null | undefined = undefined,
		brand: BrandResponse | null | undefined = undefined,
	): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, BrandId: brandId ?? brand?.Id ?? undefined });
	}

	public useModelId(
		modelId: string | null | undefined = undefined,
		model: ModelResponse | null | undefined = undefined,
	): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, ModelId: modelId ?? model?.Id ?? undefined });
	}

	public useCategoryName(categoryName: string | null | undefined): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, CategoryName: categoryName ?? undefined });
	}

	public useBrandName(brandName: string | null | undefined): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, BrandName: brandName ?? undefined });
	}

	public useModelName(modelName: string | null | undefined): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, ModelName: modelName ?? undefined });
	}

	public useIncludes(includes: string[] | null | undefined): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, Includes: includes ?? undefined });
	}

	public useTextSearch(textSearch: string | null | undefined): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, TextSearch: textSearch ?? undefined });
	}

	public useOrderByName(orderByName: boolean | null | undefined): GetLocationsQuery {
		return new GetLocationsQuery({ ...this.params, OrderByName: orderByName ?? undefined });
	}

	public containsInclude(include: string): boolean {
		if (!this.params.Includes) return false;
		return this.params.Includes.includes(include);
	}

	public static default(): GetLocationsQuery {
		return new GetLocationsQuery({
			Amount: undefined,
			Id: undefined,
			CategoryId: undefined,
			BrandId: undefined,
			ModelId: undefined,
			CategoryName: undefined,
			BrandName: undefined,
			ModelName: undefined,
			Includes: undefined,
			TextSearch: undefined,
			OrderByName: undefined,
		});
	}
}
