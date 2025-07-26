using System.Text.Json;
using RemTech.Core.Shared.Exceptions;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.PresentInfos;

public sealed class VehiclePresentCharacteristicInfo
{
    private readonly Guid _ctxId;
    private readonly string _ctxName;
    private readonly string _ctxValue;
    private readonly string _ctxMeasure;

    public VehiclePresentCharacteristicInfo()
    {
        _ctxId = Guid.Empty;
        _ctxName = string.Empty;
        _ctxValue = string.Empty;
        _ctxMeasure = string.Empty;
    }

    private VehiclePresentCharacteristicInfo(Guid id, string name, string value, string measure)
    {
        _ctxId = id;
        _ctxName = name;
        _ctxValue = value;
        _ctxMeasure = measure;
    }

    public VehiclePresentCharacteristicInfo RiddenBy(JsonElement json)
    {
        Guid id = json.GetProperty("ctx_id").GetGuid();
        if (id == Guid.Empty)
            throw new OperationException("Идентификатор характеристики объявления при чтении был пустым.");
        string? name = json.GetProperty("ctx_name").GetString();
        if (string.IsNullOrEmpty(name))
            throw new OperationException("Название характеристики объявления при чтении был пустое.");
        string? value = json.GetProperty("ctx_value").GetString();
        if (string.IsNullOrEmpty(value))
            throw new OperationException("Значение характеристики объявления при чтении было пустое.");
        string? measure = json.GetProperty("ctx_measure").GetString();
        if (string.IsNullOrEmpty(measure))
            throw new OperationException("Единица измерения характеристики объявления при чтении была пустой.");
        return new VehiclePresentCharacteristicInfo(id, name, value, measure);
    }
}