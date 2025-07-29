namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public interface IQueryVehiclesSpecification
{
    void ApplyTo(IVehiclesSqlQuery query);
}
