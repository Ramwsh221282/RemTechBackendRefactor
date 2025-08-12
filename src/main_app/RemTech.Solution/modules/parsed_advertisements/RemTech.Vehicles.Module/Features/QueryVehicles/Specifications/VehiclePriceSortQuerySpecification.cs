namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

internal sealed class VehiclePriceSortQuerySpecification(string? mode) : IQueryVehiclesSpecification
{
    private readonly string _mode = string.IsNullOrWhiteSpace(mode) ? string.Empty : mode;

    public void ApplyTo(IVehiclesSqlQuery query)
    {
        if (_mode == "ASC")
            query.AcceptAscending("v.price");
        if (_mode == "DESC")
            query.AcceptDescending("v.price");
    }
}
