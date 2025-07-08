using RemTech.Core.Shared.Extensions;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

public sealed class ParsedTransport
{
    private readonly ParsedTransportIdentity _identity;
    private readonly ParsedTransportPrice _price;
    private readonly ParsedTransportText _text;
    private readonly ParsedTransportPhotos _photos;
    private VehicleCharacteristic[] _characteristics;

    public ParsedTransport(
        ParsedTransportIdentity identity,
        ParsedTransportPrice price,
        ParsedTransportText text,
        ParsedTransportPhotos photos
    )
        : this(identity, price, text, photos, []) { }

    public ParsedTransport(
        ParsedTransportIdentity identity,
        ParsedTransportPrice price,
        ParsedTransportText text,
        ParsedTransportPhotos photos,
        VehicleCharacteristic[] characteristics
    )
    {
        _identity = identity;
        _price = price;
        _text = text;
        _photos = photos;
        _characteristics = characteristics;
    }

    public ParsedTransportIdentity Identify() => _identity;

    public VehicleOfKind Specify(ParsedVehicleKind kind)
    {
        return kind.PutKindMark(this);
    }

    public VehicleOfBrand Specify(ParsedVehicleBrand brand)
    {
        return brand.PutBrandMark(this);
    }

    public VehicleOfGeo Specify(ParsedGeoLocation geo)
    {
        return geo.PutGeoMark(this);
    }

    public Status<VehicleCharacteristic> WithCharacteristic(
        ParsedVehicleCharacteristic characteristic
    )
    {
        ParsedVehicleCharacteristicText text = characteristic.Identify().ReadText();
        VehicleCharacteristic ctx = new(this, characteristic);
        if (
            _characteristics
                .Maybe(c => c.WhatCharacteristic().Identify().ReadText().Equals(text))
                .Any()
        )
            return Error.Conflict($"У объявления уже есть такая характеристика: {(string)text}");
        _characteristics = [.. _characteristics, ctx];
        return ctx;
    }
}
