namespace Spares.Infrastructure.Queries.GetSpares;

public sealed class GetSparesQueryResponse
{
    public int TotalCount { get; private set; }
    public double AveragePrice { get; private set; }
    public double MinimalPrice { get; private set; }
    public double MaximalPrice { get; private set; }
    public IReadOnlyCollection<SpareResponse> Spares { get; set; } = [];

    public void SetTotalCount(int totalCound)
    {
        if (PriceIsAlreadySet(TotalCount))
            return;
        TotalCount = totalCound;
    }

    public void SetAveragePrice(double averagePrice)
    {
        if (PriceIsAlreadySet(AveragePrice))
            return;
        AveragePrice = averagePrice;
    }

    public void SetMinimalPrice(double minimalPrice)
    {
        if (PriceIsAlreadySet(MinimalPrice))
            return;
        MinimalPrice = minimalPrice;
    }

    public void SetMaximalPrice(double maximalPrice)
    {
        if (PriceIsAlreadySet(MaximalPrice))
            return;
        MaximalPrice = maximalPrice;
    }

    private static bool PriceIsAlreadySet(double price) => Math.Abs(price) != 0;
}
