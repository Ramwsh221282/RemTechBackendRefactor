namespace Spares.Infrastructure.Queries.GetSpares;

public sealed class GetSparesQueryResponse
{
    public int TotalCount { get; private set; }
    public double AveragePrice { get; private set; }
    public double MinimalPrice { get; private set; }
    public double MaximalPrice { get; private set; }
    public List<SpareResponse> Spares { get; } = [];

    public void SetTotalCount(int totalCound)
    {
        if (TotalCount != 0) return;
        TotalCount = totalCound;
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