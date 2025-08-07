namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;

public sealed class ParsedVehicleModel(string? model)
{
    private readonly string _model = model ?? string.Empty;

    public static implicit operator string(ParsedVehicleModel model) => model._model;

    public static implicit operator bool(ParsedVehicleModel model) =>
        !string.IsNullOrWhiteSpace(model._model);
}
