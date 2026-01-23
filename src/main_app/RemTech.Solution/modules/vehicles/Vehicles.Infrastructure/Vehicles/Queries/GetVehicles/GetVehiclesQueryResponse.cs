namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

public sealed class GetVehiclesQueryResponse
{
    public int TotalCount { get; private set; }
    public double AveragePrice { get; private set; }
    public double MinimalPrice { get; private set; }
    public double MaximalPrice { get; private set; }
    public IReadOnlyCollection<VehicleResponse> Vehicles { get; set; } = [];

    public void SetTotalCount(int totalCount)
    {
        if (TotalCount != 0)
            return;
        TotalCount = totalCount;
    }

    public void SetAveragePrice(double averagePrice)
    {
        if (PriceValueNotProvided(averagePrice))
            return;
        AveragePrice = averagePrice;
    }

    public void SetMinimalPrice(double minimalPrice)
    {
        if (PriceValueNotProvided(minimalPrice))
            return;
        MinimalPrice = minimalPrice;
    }

    public void SetMaximalPrice(double maximalPrice)
    {
        if (PriceValueNotProvided(maximalPrice))
            return;
        MaximalPrice = maximalPrice;
    }

    private static bool PriceValueNotProvided(double priceValue) => Math.Abs(priceValue) <= 0;
}
