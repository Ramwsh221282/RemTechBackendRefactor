using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.DbSearch.VehicleKinds;
using Parsing.Vehicles.Grpc.Recognition;
using PuppeteerSharp;
using RemTech.Postgres.Adapter.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class DbOrParsedVehicleKindSource(
    PgConnectionSource pgConnection,
    IPage page,
    CommunicationChannel channel
) : IParsedVehicleKindSource
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
        ParsedVehicleKind fromDb = await new VarietVehicleKindDbSearch()
            .With(new TsQueryVehicleKindSearch(pgConnection))
            .With(new PgTgrmVehicleKindSearch(pgConnection))
            .Search(kind);
        return fromDb;
    }

    private async Task<ParsedVehicleKind> TryGetFromParsing()
    {
        VariantVehicleKind kind = new VariantVehicleKind()
            .With(new FromCharacteristicsKindSource(page))
            .With(new VehicleKindFromBreadcrumbsSource(page));
        return await kind.Read();
    }
}
