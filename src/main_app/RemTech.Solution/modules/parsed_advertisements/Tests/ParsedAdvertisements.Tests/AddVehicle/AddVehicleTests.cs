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
    }
}