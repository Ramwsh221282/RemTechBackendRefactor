using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;

public sealed class ParsedVehicleModel
{
    private readonly NotEmptyString _model;

    public ParsedVehicleModel(NotEmptyString model) =>
        _model = model;

    public ParsedVehicleModel(string? model) : this(new  NotEmptyString(model)) 
    { }

    public static implicit operator NotEmptyString(ParsedVehicleModel model) => model._model;
    public static implicit operator string(ParsedVehicleModel model) => model._model;
    public static implicit operator bool(ParsedVehicleModel model) => model._model;
}