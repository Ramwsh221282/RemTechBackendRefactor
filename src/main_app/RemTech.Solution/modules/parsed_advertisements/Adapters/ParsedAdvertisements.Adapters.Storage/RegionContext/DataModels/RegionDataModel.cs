namespace ParsedAdvertisements.Adapters.Storage.RegionContext.DataModels;

public sealed class RegionDataModel
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Kind { get; set; }
}