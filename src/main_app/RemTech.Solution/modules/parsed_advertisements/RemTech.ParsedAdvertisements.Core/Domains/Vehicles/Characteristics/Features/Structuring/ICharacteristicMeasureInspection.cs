using System.Diagnostics.CodeAnalysis;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public interface ICharacteristicMeasureInspection
{
    Characteristic Inspect(Characteristic ctx);
}