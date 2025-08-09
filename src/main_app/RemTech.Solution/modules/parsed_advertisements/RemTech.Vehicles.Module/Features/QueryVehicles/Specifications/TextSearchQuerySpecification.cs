namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

public sealed class TextSearchQuerySpecification : IQueryVehiclesSpecification
{
    private readonly string _textSearch;

    public TextSearchQuerySpecification(string textSearch)
    {
        _textSearch = textSearch;
    }

    public void ApplyTo(IVehiclesSqlQuery query)
    {
        query.AcceptTextSearch(_textSearch);
    }
}
