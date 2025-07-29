namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public sealed class VehiclePriceSortQuerySpecification(string? mode) : IQueryVehiclesSpecification
{
    private readonly string _mode = string.IsNullOrWhiteSpace(mode) ? string.Empty : mode;

    public void ApplyTo(IVehiclesSqlQuery query)
    {
        if (_mode == "ASC")
            query.AcceptAscending("v.price");
        if (_mode == "DESC")
            query.AcceptDescending(" ORDER BY v.price DESC");
    }
}
