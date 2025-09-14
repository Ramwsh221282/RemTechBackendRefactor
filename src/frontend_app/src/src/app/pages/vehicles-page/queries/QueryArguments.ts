import {
  VehiclesCatalogueQuery,
  VehiclesCatalogueQueryWithCharacteristics,
  VehiclesCatalogueQueryWithLocation,
  VehiclesCatalogueQueryWithPrice,
  VehiclesCatalogueQueryWithSortOrder,
} from './VehiclesCatalogueQuery';

export class VehiclesCatalogueQuerySortOrder {
  private readonly _order: string | null;

  constructor(order: string | null) {
    this._order = order;
  }

  public addTo(query: VehiclesCatalogueQuery): VehiclesCatalogueQuery {
    return new VehiclesCatalogueQueryWithSortOrder(query, this);
  }

  public print(): object | null {
    return this._order === null ? null : { order: this._order };
  }
}

export class VehiclesCatalogueQueryLocationId {
  private readonly _locationId: string | null;

  constructor(locationId: string | null) {
    this._locationId = locationId;
  }

  public addTo(query: VehiclesCatalogueQuery): VehiclesCatalogueQuery {
    return new VehiclesCatalogueQueryWithLocation(query, this);
  }

  public print(): object | null {
    return this._locationId === null ? null : { id: this._locationId };
  }
}

export class VehiclesCatalogueTextSearchQuery {
  private readonly _searchTerm: string | null;
  constructor(searchTerm: string | null) {
    this._searchTerm = searchTerm;
  }

  public print(): object | null {
    return this._searchTerm === null ? null : { textSearch: this._searchTerm };
  }
}

export class VehiclesCatalogueQueryPriceSpecification {
  private readonly _isNds: boolean | null;
  private readonly _startFrom: number | null;
  private readonly _endAt: number | null;

  constructor(
    isNds: boolean | null,
    startFrom: number | null,
    endAt: number | null,
  ) {
    this._isNds = isNds;
    this._startFrom = startFrom;
    this._endAt = endAt;
  }

  public withoutPriceFrom(): VehiclesCatalogueQueryPriceSpecification {
    return new VehiclesCatalogueQueryPriceSpecification(
      this._isNds,
      null,
      this._endAt,
    );
  }

  public withoutPriceTo(): VehiclesCatalogueQueryPriceSpecification {
    return new VehiclesCatalogueQueryPriceSpecification(
      this._isNds,
      this._startFrom,
      null,
    );
  }

  public reset(): VehiclesCatalogueQueryPriceSpecification {
    return new VehiclesCatalogueQueryPriceSpecification(null, null, null);
  }

  public withStartFrom(
    value: number,
  ): VehiclesCatalogueQueryPriceSpecification {
    return new VehiclesCatalogueQueryPriceSpecification(
      this._isNds,
      value,
      this._endAt,
    );
  }

  public withEndAt(value: number): VehiclesCatalogueQueryPriceSpecification {
    return new VehiclesCatalogueQueryPriceSpecification(
      this._isNds,
      this._startFrom,
      value,
    );
  }

  public withNdsSpecification(
    isNds: boolean,
  ): VehiclesCatalogueQueryPriceSpecification {
    return new VehiclesCatalogueQueryPriceSpecification(
      isNds,
      this._startFrom,
      this._endAt,
    );
  }

  public addTo(query: VehiclesCatalogueQuery): VehiclesCatalogueQuery {
    return new VehiclesCatalogueQueryWithPrice(query, this);
  }

  public print(): object | null {
    if (!this._isNds && !this._startFrom && !this._endAt) return null;
    return {
      isNds: this._isNds === null ? null : this._isNds,
      priceFrom: this._startFrom === null ? null : this._startFrom,
      priceTo: this._endAt === null ? null : this._endAt,
    };
  }
}

export class VehicleModelQueryArgument {
  private readonly _modelId: string | null;
  constructor(modelId: string | null) {
    this._modelId = modelId;
  }
  public print(): object | null {
    return this._modelId === null ? null : { id: this._modelId };
  }
}

export class VehiclesCatalogueQueryCharacteristicsList {
  private readonly _characteristics: VehiclesCatalogueQueryCharacteristic[];

  constructor(characteristics: VehiclesCatalogueQueryCharacteristic[]) {
    this._characteristics = characteristics;
  }

  public addTo(query: VehiclesCatalogueQuery): VehiclesCatalogueQuery {
    return new VehiclesCatalogueQueryWithCharacteristics(query, this);
  }

  public removeById(id: string): VehiclesCatalogueQueryCharacteristicsList {
    const withoutItem: VehiclesCatalogueQueryCharacteristic[] =
      this._characteristics.filter(
        (c: VehiclesCatalogueQueryCharacteristic): boolean => c.id !== id,
      );
    return new VehiclesCatalogueQueryCharacteristicsList(withoutItem);
  }

  public accept(
    characteristic: VehiclesCatalogueQueryCharacteristic,
  ): VehiclesCatalogueQueryCharacteristicsList {
    const id: string = characteristic.id;
    const indexOfExisting: number = this._characteristics.findIndex(
      (item: VehiclesCatalogueQueryCharacteristic): boolean => item.id === id,
    );
    if (indexOfExisting > -1) {
      this._characteristics[indexOfExisting] = characteristic;
      return this;
    }

    return new VehiclesCatalogueQueryCharacteristicsList([
      ...this._characteristics,
      characteristic,
    ]);
  }

  public print(): object | null {
    if (this._characteristics.length === 0) return null;
    return {
      arguments: this._characteristics.map(
        (c: VehiclesCatalogueQueryCharacteristic) => c.print(),
      ),
    };
  }
}

export class VehiclesCatalogueQueryCharacteristic {
  private readonly _id: string;
  private readonly _value: string;
  private readonly _name: string;

  constructor(id: string, name: string, value: string) {
    this._id = id;
    this._value = value;
    this._name = name;
  }

  public get id(): string {
    return this._id;
  }

  public addTo(
    list: VehiclesCatalogueQueryCharacteristicsList,
  ): VehiclesCatalogueQueryCharacteristicsList {
    return list.accept(this);
  }

  public print(): object {
    return { id: this._id, name: this._name, value: this._value };
  }
}
