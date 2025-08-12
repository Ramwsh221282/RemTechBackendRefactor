using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class DbOrParsedVehicleKindSource(IPage page) : IParsedVehicleKindSource
{
    public async Task<ParsedVehicleKind> Read()
    {
        VariantVehicleKind kind = new VariantVehicleKind()
            .With(new FromCharacteristicsKindSource(page))
            .With(new VehicleKindFromBreadcrumbsSource(page));
        return await kind.Read();
    }
}
