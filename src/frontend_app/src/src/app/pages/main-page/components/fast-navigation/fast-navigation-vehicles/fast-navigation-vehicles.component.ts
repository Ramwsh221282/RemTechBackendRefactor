import { Component, computed, effect, EventEmitter, inject, Input, Output, signal, Signal, WritableSignal } from '@angular/core';
import { Select, SelectModule } from 'primeng/select';
import { tap, catchError, Observable, EMPTY } from 'rxjs';
import { BrandResponse } from '../../../../../shared/api/brands-module/brands-api.responses';
import { BrandsApiService } from '../../../../../shared/api/brands-module/brands-api.service';
import { CategoriesApiService } from '../../../../../shared/api/categories-module/categories-api.service';
import { CategoryResponse } from '../../../../../shared/api/categories-module/categories-responses';
import { ModelsApiService } from '../../../../../shared/api/models-module/models-api.service';
import { ModelResponse } from '../../../../../shared/api/models-module/models-responses';
import { VehicleResponse, GetVehiclesQueryResponse } from '../../../../../shared/api/vehicles-module/vehicles-api.responses';
import { VehiclesApiService } from '../../../../../shared/api/vehicles-module/vehicles-api.service';
import { FormsModule } from '@angular/forms';
import { GetBrandsQuery } from '../../../../../shared/api/brands-module/brands-get-query';
import { GetCategoriesQuery } from '../../../../../shared/api/categories-module/categories-get-query';
import { GetModelsQuery } from '../../../../../shared/api/models-module/models-get-query';
import { GetVehiclesQuery } from '../../../../../shared/api/vehicles-module/vehicles-get-query';

type FastNavigationVehicleProps = {
	page?: number | undefined;
	pageSize?: number | undefined;
	isWatching: boolean;
	category?: CategoryResponse | undefined;
	brand?: BrandResponse | undefined;
	model?: ModelResponse | undefined;
	textSearch?: string | undefined;
};

const defaultProps: () => FastNavigationVehicleProps = (): FastNavigationVehicleProps => {
	return {
		page: undefined,
		pageSize: undefined,
		isWatching: false,
		category: undefined,
		brand: undefined,
		model: undefined,
		textSearch: undefined,
	};
};

@Component({
	selector: 'app-fast-navigation-vehicles',
	imports: [Select, SelectModule, FormsModule],
	templateUrl: './fast-navigation-vehicles.component.html',
	styleUrl: './fast-navigation-vehicles.component.css',
})
export class FastNavigationVehiclesComponent {
	private readonly _categoriesService: CategoriesApiService = inject(CategoriesApiService);
	private readonly _brandsService: BrandsApiService = inject(BrandsApiService);
	private readonly _modelsService: ModelsApiService = inject(ModelsApiService);
	private readonly _vehiclesService: VehiclesApiService = inject(VehiclesApiService);
	@Input({ required: true }) set vehicles_fetch_props(value: {
		page?: number | undefined;
		pageSize?: number | undefined;
		isWatching: boolean;
		category?: CategoryResponse | undefined;
		brand?: BrandResponse | undefined;
		model?: ModelResponse | undefined;
		textSearch?: string | undefined;
	}) {
		this.vehiclesFetchProps.update((state: FastNavigationVehicleProps): FastNavigationVehicleProps => {
			return {
				...state,
				brand: value.brand,
				model: value.model,
				textSearch: value.textSearch,
				category: value.category,
				isWatching: value.isWatching,
				page: value.page,
				pageSize: value.pageSize,
			};
		});
	}
	@Output() brandSelected: EventEmitter<BrandResponse | undefined> = new EventEmitter<BrandResponse | undefined>();
	@Output() categorySelected: EventEmitter<CategoryResponse | undefined> = new EventEmitter<CategoryResponse | undefined>();
	@Output() modelSelected: EventEmitter<ModelResponse | undefined> = new EventEmitter<ModelResponse | undefined>();
	@Output() vehiclesFetched: EventEmitter<{
		vehicles: VehicleResponse[];
		totalCount: number;
	}> = new EventEmitter<{
		vehicles: VehicleResponse[];
		totalCount: number;
	}>();

	readonly vehiclesFetchProps: WritableSignal<FastNavigationVehicleProps> = signal(defaultProps());
	readonly itemTypes: string[] = ['Техника', 'Запчасти'];
	readonly categories: WritableSignal<CategoryResponse[]> = signal([]);
	readonly brands: WritableSignal<BrandResponse[]> = signal([]);
	readonly models: WritableSignal<ModelResponse[]> = signal([]);

	readonly brandsQuery: Signal<GetBrandsQuery> = computed((): GetBrandsQuery => {
		const props: FastNavigationVehicleProps = this.vehiclesFetchProps();
		return GetBrandsQuery.default()
			.useCategoryId(props.category?.Id)
			.useCategoryName(props.category?.Name)
			.useModelId(props.model?.Id)
			.useModelName(props.model?.Name);
	});

	readonly modelsQuery: Signal<GetModelsQuery> = computed((): GetModelsQuery => {
		const props: FastNavigationVehicleProps = this.vehiclesFetchProps();
		return GetModelsQuery.default()
			.useBrandId(props.brand?.Id)
			.useBrandName(props.brand?.Name)
			.useCategoryId(props.category?.Id)
			.useCategoryName(props.category?.Name);
	});

	readonly vehiclesQuery: Signal<GetVehiclesQuery> = computed(() => {
		const props: {
			page?: number | undefined;
			pageSize?: number | undefined;
			isWatching: boolean;
			category?: CategoryResponse | undefined;
			brand?: BrandResponse | undefined;
			model?: ModelResponse | undefined;
		} = this.vehiclesFetchProps();
		return GetVehiclesQuery.default()
			.usePage(props.page)
			.usePageSize(props.pageSize)
			.useCategory(props.category)
			.useBrand(props.brand)
			.useModel(props.model);
	});

	readonly readSelectedItemType: Signal<string> = computed((): string => {
		const vehicleProps = this.vehiclesFetchProps();
		return vehicleProps.isWatching ? 'Техника' : 'Запчасти';
	});

	readonly userIsWatchingVehicles: Signal<boolean> = computed((): boolean => {
		return this.vehiclesFetchProps().isWatching;
	});

	readonly userSelectedCategory: Signal<boolean> = computed((): boolean => {
		return !!this.vehiclesFetchProps().category;
	});

	readonly userSelectedBrand: Signal<boolean> = computed((): boolean => {
		return !!this.vehiclesFetchProps().brand;
	});

	readonly userSelectedModel: Signal<boolean> = computed((): boolean => {
		return !!this.vehiclesFetchProps().model;
	});

	readonly fetchSelectionDataOnPropsChange = effect((): void => {
		const props: FastNavigationVehicleProps = this.vehiclesFetchProps();
		this.fetchCategoriesOnItemTypeChange(props);
		this.fetchBrandsOnCategoryChange(props);
		this.fetchModelsOnBrandChange(props);
		this.fetchVehiclesOnQueryChange(props);
	});

	public navigateVehicles(): void {
		// const brand: CatalogueBrand | undefined = this.selectedBrand();
		// const model: CatalogueModel | undefined = this.selectedModel();
		// const type: CatalogueCategory | undefined = this.selectedType();
		// if (brand && model && type) {
		//   this._router.navigate(['vehicles'], {
		//     queryParams: {
		//       brandId: brand.id,
		//       categoryId: type.id,
		//       modelId: model.id,
		//       page: 1,
		//     },
		//   });
		// }
	}

	private fetchBrandsOnCategoryChange(props: FastNavigationVehicleProps): void {
		if (props.category) {
			const query: GetBrandsQuery = this.brandsQuery();
			this._brandsService
				.fetchBrands(query)
				.pipe(
					tap((brands: BrandResponse[]): void => this.brands.set(brands)),
					catchError((): Observable<never> => EMPTY),
				)
				.subscribe();
		}
	}

	private fetchCategoriesOnItemTypeChange(props: FastNavigationVehicleProps): void {
		if (!props.isWatching) return;
		this._categoriesService
			.fetchCategories(GetCategoriesQuery.default())
			.pipe(
				tap((categories: CategoryResponse[]): void => this.categories.set(categories)),
				catchError((): Observable<never> => EMPTY),
			)
			.subscribe();
	}

	private fetchModelsOnBrandChange(props: FastNavigationVehicleProps): void {
		if (!props.brand) return;
		const query: GetModelsQuery = this.modelsQuery();
		this._modelsService
			.fetchModels(query)
			.pipe(
				tap((models: ModelResponse[]): void => this.models.set(models)),
				catchError((): Observable<never> => EMPTY),
			)
			.subscribe();
	}

	private fetchVehiclesOnQueryChange(props: FastNavigationVehicleProps): void {
		if (!props.isWatching) return;
		if (!props.category || !props.brand || !props.model) return;
		const query: GetVehiclesQuery = this.vehiclesQuery();
		this._vehiclesService
			.fetchVehicles(query)
			.pipe(
				tap((response: GetVehiclesQueryResponse): void => {
					this.vehiclesFetched.emit({
						vehicles: response.Vehicles,
						totalCount: response.TotalCount,
					});
				}),
				catchError((): Observable<never> => EMPTY),
			)
			.subscribe();
	}
}
