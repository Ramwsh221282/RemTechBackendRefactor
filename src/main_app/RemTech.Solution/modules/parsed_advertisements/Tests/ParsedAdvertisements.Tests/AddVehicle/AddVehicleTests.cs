using ParsedAdvertisements.Adapters.Storage;
using Shared.Infrastructure.Module.DependencyInjection;

namespace ParsedAdvertisements.Tests.AddVehicle;

public sealed class AddVehicleTests : IClassFixture<ParsedAdvertisementsServices>
{
    private readonly ParsedAdvertisementsServices _services;

    public AddVehicleTests(ParsedAdvertisementsServices services)
    {
        _services = services;
    }

    [Fact]
    private async Task Access_Service_Collection()
    {
        await using var scope = _services.CreateScope();
        await using var dbContext = scope.GetService<ParsedAdvertisementsDbContext>();
        // dbContext.Database.
    }
}
