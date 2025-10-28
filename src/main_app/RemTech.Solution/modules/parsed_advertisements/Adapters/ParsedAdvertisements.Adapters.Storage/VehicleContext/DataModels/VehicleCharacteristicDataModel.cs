namespace ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;

public sealed class VehicleCharacteristicDataModel
{
    public required string VehicleId { get; set; }
    public required Guid CharacteristicId { get; set; }
    public required string CharacteristicName { get; set; }
    public required string CharacteristicValue { get; set; }
}