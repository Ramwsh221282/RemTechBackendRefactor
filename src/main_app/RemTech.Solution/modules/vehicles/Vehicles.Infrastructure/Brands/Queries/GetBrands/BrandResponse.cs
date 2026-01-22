namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

public sealed class BrandResponse
{
    public BrandResponse(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
    public float? TextSearchScore { get; set; }
    public int? VehiclesCount { get; set; }
    public int? TotalCount { get; set; }
}
