import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { apiUrl } from '../../../shared/api/api-endpoint';
import { QueryPipeLineParamsFactory } from '../queries/QueryParamsPipeline/QueryPipeLineParamsFactory';
import { Observable } from 'rxjs';
import { CatalogueVehicle } from '../types/CatalogueVehicle';
import { CatalogueBrand } from '../types/CatalogueBrand';
import { CatalogueCategory } from '../types/CatalogueCategory';
import { CatalogueModel } from '../types/CatalogueModel';
import { CatalogueRegion } from '../types/CatalogueRegion';

@Injectable({
  providedIn: 'root',
})
export class CatalogueVehiclesService {
  private readonly _httpClient: HttpClient;
  private readonly _apiUrl: string;
  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
    this._apiUrl = `${apiUrl}/vehicles`;
  }

  public fetch(
    paramsFactory: QueryPipeLineParamsFactory,
  ): Observable<CatalogueVehicle[]> {
    const params: HttpParams = paramsFactory.makeParams();
    return this._httpClient.get<CatalogueVehicle[]>(this._apiUrl, {
      params: params,
    });
  }

  public fetchCategoryBrands(categoryId: string): Observable<CatalogueBrand[]> {
    const requestUrl: string = `${this._apiUrl}/category-brands`;
    const params: HttpParams = new HttpParams().set('categoryId', categoryId);
    return this._httpClient.get<CatalogueBrand[]>(requestUrl, {
      params: params,
    });
  }

  public fetchModelsCategoryBrands(
    categoryId: string,
    brandId: string,
  ): Observable<CatalogueModel[]> {
    const requestUrl: string = `${this._apiUrl}/category-brand-models`;
    const params: HttpParams = new HttpParams()
      .set('categoryId', categoryId)
      .set('brandId', brandId);
    return this._httpClient.get<CatalogueModel[]>(requestUrl, {
      params: params,
    });
  }

  public fetchConcrete(id: string): Observable<CatalogueVehicle> {
    const requestUrl: string = `${this._apiUrl}/item`;
    const params: HttpParams = new HttpParams().set('vehicleId', id);
    return this._httpClient.get<CatalogueVehicle>(requestUrl, {
      params: params,
    });
  }

  public fetchSimilar(
    id: string,
    text: string,
  ): Observable<CatalogueVehicle[]> {
    const requestUrl: string = `${this._apiUrl}/similar`;
    const params: HttpParams = new HttpParams()
      .set('text', text)
      .set('vehicleId', id);
    return this._httpClient.get<CatalogueVehicle[]>(requestUrl, {
      params: params,
    });
  }

  public count(paramsFactory: QueryPipeLineParamsFactory): Observable<number> {
    const params: HttpParams = paramsFactory.makeParams();
    const requestUrl = `${this._apiUrl}/count`;
    return this._httpClient.get<number>(requestUrl, { params: params });
  }

  public fetchBrands(): Observable<CatalogueBrand[]> {
    const requestUrl = `${this._apiUrl}/brands`;
    return this._httpClient.get<CatalogueBrand[]>(requestUrl);
  }

  public fetchCategories(): Observable<CatalogueCategory[]> {
    const requestUrl = `${this._apiUrl}/categories`;
    return this._httpClient.get<CatalogueCategory[]>(requestUrl);
  }

  public fetchModels(): Observable<CatalogueModel[]> {
    const requestUrl = `${this._apiUrl}/models`;
    return this._httpClient.get<CatalogueModel[]>(requestUrl);
  }

  public fetchRegions(): Observable<CatalogueRegion[]> {
    const requestUrl = `${this._apiUrl}/regions`;
    return this._httpClient.get<CatalogueRegion[]>(requestUrl);
  }

  public static default(): CatalogueVehicle {
    return {
      region: '',
      isNds: false,
      price: 0,
      brandId: '',
      brandName: '',
      categoryId: '',
      categoryName: '',
      city: '',
      characteristics: [],
      description: '',
      id: '',
      modelId: '',
      modelName: '',
      photos: [],
      regionId: '',
      regionKind: '',
      sourceUrl: '',
    };
  }
}
