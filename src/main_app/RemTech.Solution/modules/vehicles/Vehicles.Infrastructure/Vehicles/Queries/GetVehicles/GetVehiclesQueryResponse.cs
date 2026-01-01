namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

public sealed class GetVehiclesQueryResponse
{
    public int TotalCount { get; private set; } = 0;
    public double AveragePrice { get; private set; } = 0;
    public double MinimalPrice { get; private set; } = 0;
    public double MaximalPrice { get; private set; } = 0;
    
    public List<VehicleResponse> Vehicles { get; } = [];

    public void SetTotalCount(int totalCount)
    {
        if (TotalCount != 0) return;
        TotalCount = totalCount;
    }
    
    public void SetAveragePrice(double averagePrice)
    {
        if (AveragePrice != 0) return;
        AveragePrice = averagePrice;
    }
    
    public void SetMinimalPrice(double minimalPrice)
    {
        if (MinimalPrice != 0) return;
        MinimalPrice = minimalPrice;
    }
    
    public void SetMaximalPrice(double maximalPrice)
    {
        if (MaximalPrice != 0) return;
        MaximalPrice = maximalPrice;
    }
}