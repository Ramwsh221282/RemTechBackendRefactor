using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Tests.Vehicles.Kinds;

public sealed class PgVehicleKindsTests : IClassFixture<DataSourceTestsFixture>
{
    private readonly DataSourceTestsFixture _fixture;

    public PgVehicleKindsTests(DataSourceTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Test_Add_Vehicle_Kind_Success()
    {
        await using IAsyncVehicleKinds kinds = _fixture.VehicleKinds();
        IVehicleKind kind = new NewVehicleKind("Бульдозеры");
        Status<IVehicleKind> added = await kinds.Add(kind);
        Assert.True(added.IsSuccess);
    }

    [Fact]
    private async Task Test_Add_Vehicle_Kind_Name_Failure()
    {
        await using IAsyncVehicleKinds kinds = _fixture.VehicleKinds();
        IVehicleKind kind = new NewVehicleKind(string.Empty);
        Status<IVehicleKind> added = await kinds.Add(kind);
        Assert.True(added.IsFailure);
    }

    [Fact]
    private async Task Test_Add_Similar_Vehicle_Kind()
    {
        await using IAsyncVehicleKinds kinds = _fixture.VehicleKinds();
        IVehicleKind kind1 = new NewVehicleKind("Мини-погрузчик");
        IVehicleKind kind2 = new NewVehicleKind("Фронтальный-погрузчик");
        IVehicleKind kind3 = new NewVehicleKind("Фронтальный");
        IVehicleKind kind4 = new NewVehicleKind("Фронтальный погрузчик LuGong LM938, 2025");
        await kinds.Add(kind1, CancellationToken.None);
        Status<IVehicleKind> added2 = await kinds.Add(kind2, CancellationToken.None);
        Status<IVehicleKind> added3 = await kinds.Add(kind3, CancellationToken.None);
        Status<IVehicleKind> added4 = await kinds.Add(kind4, CancellationToken.None);
        Assert.True(added3.IsSuccess);
        Assert.True(added4.IsSuccess);
        string kind2Name = kind2.Identify().ReadText();
        string createdKind2 = added2.Value.Identify().ReadText();
        string createdKind3 = added3.Value.Identify().ReadText();
        string createdKind4 = added4.Value.Identify().ReadText();
        Assert.Equal(kind2Name, createdKind2);
        Assert.Equal(kind2Name, createdKind3);
        Assert.Equal(kind2Name, createdKind4);
    }
}
