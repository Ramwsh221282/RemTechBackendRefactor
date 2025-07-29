using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.Shared;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public sealed record VehiclePaginationQuerySpecification : IQueryVehiclesSpecification
{
    private readonly int _currentPage;

    public VehiclePaginationQuerySpecification(int currentPage)
    {
        _currentPage = currentPage;
    }

    public void ApplyTo(IVehiclesSqlQuery query)
    {
        query.AcceptPagination(_currentPage);
    }
}
