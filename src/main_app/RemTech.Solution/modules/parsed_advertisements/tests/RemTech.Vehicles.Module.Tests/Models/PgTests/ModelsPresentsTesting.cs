using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds;
using RemTech.Vehicles.Module.Features.QueryVehicleModels;
using RemTech.Vehicles.Module.Tests.Fixtures;
using VehicleBrandPresentation = RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types.VehicleBrandPresentation;
using VehicleKindPresentation = RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types.VehicleKindPresentation;
using VehicleModelPresentation = RemTech.Vehicles.Module.Features.QueryVehicleModels.Types.VehicleModelPresentation;

namespace RemTech.Vehicles.Module.Tests.Models.PgTests;

public sealed class ModelsPresentsTesting(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task Read_Model_Presents_Success()
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await using NpgsqlConnection connection = await source.Connect();
        IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
            VehicleKindsPresentationSource.VehicleKindsCommand,
            VehicleKindsPresentationSource.VehicleKindsReader,
            VehicleKindsPresentationSource.VehicleKindsReading,
            CancellationToken.None
        );
        VehicleKindPresentation firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresentation> brands = await connection.Provide(
            kindId,
            VehicleBrandPresentationSource.CreateCommand,
            VehicleBrandPresentationSource.CreateReader,
            VehicleBrandPresentationSource.ProcessWithReader
        );
        Assert.NotEmpty(brands);
        VehicleBrandPresentation firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresentation> models = await connection.Provide(
            kindId,
            brandId,
            VehicleModelPresentationSource.VehicleModelsCommandSource,
            VehicleModelPresentationSource.VehicleModelsReaderSource,
            VehicleModelPresentationSource.VehicleModelsReadingSource
        );
        Assert.NotEmpty(models);
    }
}
