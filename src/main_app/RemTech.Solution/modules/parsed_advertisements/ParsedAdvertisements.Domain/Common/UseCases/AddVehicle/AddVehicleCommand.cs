using RemTech.Core.Shared.Cqrs;

namespace ParsedAdvertisements.Domain.Common.UseCases.AddVehicle;

public sealed record AddVehicleCommand(
    string VehicleId,
    string CategoryName,
    string BrandName,
    string ModelName,
    bool IsNds,
    double Price,
    string Region,
    string SourceUrl,
    string SourceDomain,
    Guid SourceId,
    IEnumerable<string> Locations,
    IEnumerable<string> Photos,
    IEnumerable<VehicleCharacteristicDto> Characteristics) : ICommand;