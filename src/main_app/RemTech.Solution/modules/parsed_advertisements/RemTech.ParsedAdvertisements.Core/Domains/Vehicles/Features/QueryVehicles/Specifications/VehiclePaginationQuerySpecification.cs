namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

public sealed record VehiclePaginationQuerySpecification : IQueryVehiclesSpecification
{
    private readonly int _currentPage;

    public VehiclePaginationQuerySpecification(int currentPage)
    {
        _currentPage = currentPage;
    }

    public void ApplyTo(VehiclesSqlQuery query)
    {
        query.AcceptPagination(_currentPage);
    }
}