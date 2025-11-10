using ParsedAdvertisements.Core.VehicleContext.ValueObjects;

namespace ParsedAdvertisements.Core.VehicleContext;

public sealed record Vehicle(VehicleMetadata Metadata, VehicleParameters Parameters, VehicleSourceSpecification Source);