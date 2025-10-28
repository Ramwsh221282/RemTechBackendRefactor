namespace ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;

public sealed class VehicleDataModel
{
    public required string VehicleId { get; set; }
    public required Guid BrandId { get; set; }
    public required Guid CategoryId { get; set; }
    public required Guid ModelId { get; set; }
    public required Guid LocationId { get; set; }
    public required double Price { get; set; }
    public required bool IsNds { get; set; }
    public required string Url { get; set; }
    public required string Domain { get; set; }
    public required string LocationPath { get; set; }
    public required string Photos { get; set; }
    public required IEnumerable<VehicleCharacteristicDataModel> Characteristics { get; set; }
}