using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.GeoLocations;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Tests.Vehicles.Locations;

public sealed class GeoLocationsTests : IClassFixture<DataSourceTestsFixture>
{
    private readonly DataSourceTestsFixture _fixture;

    public GeoLocationsTests(DataSourceTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Add_Location_Success()
    {
        string city = "Москва";
        string kind = "Область";
        GeoLocationEnvelope location = new NewGeoLocation(city, kind);
        await using IAsyncGeoLocations locations = _fixture.Locations();
        Status<IGeoLocation> added = await locations.Add(location);
        Assert.True(added.IsSuccess);
    }

    [Fact]
    private async Task Add_Location_Name_Failure()
    {
        string city = string.Empty;
        string kind = string.Empty;
        GeoLocationEnvelope location = new NewGeoLocation(city, kind);
        await using IAsyncGeoLocations locations = _fixture.Locations();
        Status<IGeoLocation> added = await locations.Add(location);
        Assert.False(added.IsSuccess);
    }

    [Fact]
    private async Task Add_Similar_Location_Success()
    {
        string city1 = "Красноярск";
        string kind1 = "Край";
        string city2 = "Московская область";
        string kind2 = "Область";
        string city3 = "Калининград";
        string kind3 = "Область";
        string expectedRegion1 = "Красноярский";
        string expectedRegion2 = "Московская";
        string expectedRegion3 = "Калининградская";
        GeoLocationEnvelope location1 = new NewGeoLocation(city1, kind1);
        GeoLocationEnvelope location2 = new NewGeoLocation(city2, kind2);
        GeoLocationEnvelope location3 = new NewGeoLocation(city3, kind3);
        await using IAsyncGeoLocations locations = _fixture.Locations();
        Status<IGeoLocation> added1 = await locations.Add(location1);
        Status<IGeoLocation> added2 = await locations.Add(location2);
        Status<IGeoLocation> added3 = await locations.Add(location3);
        Assert.True(added1.IsSuccess);
        Assert.True(added2.IsSuccess);
        Assert.True(added3.IsSuccess);
        string created1Name = added1.Value.Identify().ReadText();
        string created2Name = added2.Value.Identify().ReadText();
        string created3Name = added3.Value.Identify().ReadText();
        Assert.Equal(expectedRegion1, created1Name);
        Assert.Equal(expectedRegion2, created2Name);
        Assert.Equal(expectedRegion3, created3Name);
    }
}
