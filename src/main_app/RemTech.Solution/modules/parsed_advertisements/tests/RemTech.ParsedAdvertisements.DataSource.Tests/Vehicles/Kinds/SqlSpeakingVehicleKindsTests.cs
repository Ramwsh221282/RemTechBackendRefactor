using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds.Decorators;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Tests.Vehicles.Kinds;

public sealed class SqlSpeakingVehicleKindsTests : IClassFixture<DataSourceTestsFixture>
{
    // private readonly DataSourceTestsFixture _fixture;
    //
    // public SqlSpeakingVehicleKindsTests(DataSourceTestsFixture fixture)
    // {
    //     _fixture = fixture;
    // }
    //
    // [Fact]
    // private async Task Test_Add_Vehicle_Kind_Success()
    // {
    //     await using PgDefaultConnectionSource engine = _fixture.Engine();
    //     IVehicleKind kind = new NewVehicleKind("Бульдозеры");
    //     Status<IVehicleKind> added = await new TextSearchValidatingSqlSpeakingVehicleKinds(
    //         new TextSearchSqlSpeakingVehicleKinds(
    //             engine,
    //             new ValidatingSqlSpeakingKindsEnvelope(new SqlSpeakingVehicleKinds(engine))
    //         )
    //     ).Add(kind, CancellationToken.None);
    //     Assert.True(added.IsSuccess);
    // }
    //
    // [Fact]
    // private async Task Test_Add_Vehicle_Kind_Name_Failure()
    // {
    //     await using PgDefaultConnectionSource engine = _fixture.Engine();
    //     IVehicleKind kind = new NewVehicleKind(string.Empty);
    //     Status<IVehicleKind> added = await new TextSearchValidatingSqlSpeakingVehicleKinds(
    //         new TextSearchSqlSpeakingVehicleKinds(
    //             engine,
    //             new ValidatingSqlSpeakingKindsEnvelope(new SqlSpeakingVehicleKinds(engine))
    //         )
    //     ).Add(kind, CancellationToken.None);
    //     Assert.True(added.IsFailure);
    // }
    //
    // [Fact]
    // private async Task Test_Add_Similar_Vehicle_Kind()
    // {
    //     await using PgDefaultConnectionSource engine = _fixture.Engine();
    //     IVehicleKind kind1 = new NewVehicleKind("Мини-погрузчик");
    //     IVehicleKind kind2 = new NewVehicleKind("Фронтальный-погрузчик");
    //     IVehicleKind kind3 = new NewVehicleKind("Фронтальный");
    //     await new TextSearchValidatingSqlSpeakingVehicleKinds(
    //         new TextSearchSqlSpeakingVehicleKinds(
    //             engine,
    //             new ValidatingSqlSpeakingKindsEnvelope(new SqlSpeakingVehicleKinds(engine))
    //         )
    //     ).Add(kind1, CancellationToken.None);
    //     Status<IVehicleKind> added2 = await new TextSearchValidatingSqlSpeakingVehicleKinds(
    //         new TextSearchSqlSpeakingVehicleKinds(
    //             engine,
    //             new ValidatingSqlSpeakingKindsEnvelope(new SqlSpeakingVehicleKinds(engine))
    //         )
    //     ).Add(kind2, CancellationToken.None);
    //     Status<IVehicleKind> added3 = await new TextSearchValidatingSqlSpeakingVehicleKinds(
    //         new TextSearchSqlSpeakingVehicleKinds(
    //             engine,
    //             new ValidatingSqlSpeakingKindsEnvelope(new SqlSpeakingVehicleKinds(engine))
    //         )
    //     ).Add(kind2, CancellationToken.None);
    //     Assert.True(added3.IsSuccess);
    //
    //     string kind2Name = kind2.Identify().ReadText();
    //     string createdKind2 = added2.Value.Identify().ReadText();
    //     string createdKind3 = added3.Value.Identify().ReadText();
    //     Assert.Equal(kind2Name, createdKind2);
    //     Assert.Equal(kind2Name, createdKind3);
    // }
}
