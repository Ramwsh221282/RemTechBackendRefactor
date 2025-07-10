using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports;

public sealed class ValidatingVehicleKinds(IVehicleKinds origin) : IVehicleKinds
{
    public Status<VehicleKindEnvelope> Add(string? name)
    {
        NotEmptyString kindName = new(name);
        return !kindName
            ? new ValidationError<VehicleKindEnvelope>(
                $"Некорректное название типа техники: {name}"
            )
            : origin.Add(name);
    }

    public MaybeBag<VehicleKindEnvelope> GetByName(string? name) =>
        !new NotEmptyString(name) ? new MaybeBag<VehicleKindEnvelope>() : origin.GetByName(name);
}