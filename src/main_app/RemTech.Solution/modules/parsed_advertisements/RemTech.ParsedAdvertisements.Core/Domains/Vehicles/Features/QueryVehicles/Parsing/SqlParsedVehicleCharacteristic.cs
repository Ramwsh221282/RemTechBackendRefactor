using System.Text.Json;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleCharacteristic(JsonElement json)
{
    public VehicleCharacteristic Read()
    {
        Guid id = json.GetProperty("ctx_id").GetGuid();
        if (id == Guid.Empty)
            throw new ApplicationException("Идентификатор характеристики объявления при чтении был пустым.");
        string? name = json.GetProperty("ctx_name").GetString();
        if (string.IsNullOrEmpty(name))
            throw new ApplicationException("Название характеристики объявления при чтении был пустое.");
        string? value = json.GetProperty("ctx_value").GetString();
        if (string.IsNullOrEmpty(value))
            throw new ApplicationException("Значение характеристики объявления при чтении было пустое.");
        string? measure = json.GetProperty("ctx_measure").GetString();
        if (string.IsNullOrEmpty(measure))
            throw new ApplicationException("Единица измерения характеристики объявления при чтении была пустой.");
        CharacteristicIdentity identity = new(new CharacteristicId(id), new CharacteristicText(name));
        VehicleCharacteristicValue vehicleCtxValue = new VehicleCharacteristicValue(value);
        VehicleCharacteristic characteristic = new(new Characteristic(identity, new CharacteristicMeasure(measure)), vehicleCtxValue);
        return characteristic;
    }
}