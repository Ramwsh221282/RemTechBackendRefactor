namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

internal sealed record VehiclePaginationQuerySpecification : IQueryVehiclesSpecification
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
