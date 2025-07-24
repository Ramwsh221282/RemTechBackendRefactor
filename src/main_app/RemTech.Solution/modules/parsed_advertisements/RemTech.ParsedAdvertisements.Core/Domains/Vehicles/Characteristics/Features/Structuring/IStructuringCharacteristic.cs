using System.Diagnostics.CodeAnalysis;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public interface IStructuringCharacteristic
{
    bool Structure([NotNullWhen(true)] out ValuedCharacteristic? ctx);
}