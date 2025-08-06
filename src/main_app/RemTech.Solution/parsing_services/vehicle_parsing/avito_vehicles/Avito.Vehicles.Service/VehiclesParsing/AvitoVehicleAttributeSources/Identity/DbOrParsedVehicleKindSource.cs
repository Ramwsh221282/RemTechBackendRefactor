using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Grpc.Recognition;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class DbOrParsedVehicleKindSource(IPage page, CommunicationChannel channel)
    : IParsedVehicleKindSource
{
    public async Task<ParsedVehicleKind> Read()
    {
        try
        {
            return await TryGetFromParsing();
        }
        catch
        {
            return await TryGetSimilarFromDb();
        }
    }

    private async Task<ParsedVehicleKind> TryGetSimilarFromDb()
    {
        VariantVehicleKind variant = new VariantVehicleKind()
            .With(new GrpcVehicleKindFromTitle(channel, page))
            .With(new GrpcVehicleKindFromDescription(channel, page));
        ParsedVehicleKind kind = await variant.Read();
        return kind;
    }

    private async Task<ParsedVehicleKind> TryGetFromParsing()
    {
        VariantVehicleKind kind = new VariantVehicleKind()
            .With(new FromCharacteristicsKindSource(page))
            .With(new VehicleKindFromBreadcrumbsSource(page));
        return await kind.Read();
    }
}
