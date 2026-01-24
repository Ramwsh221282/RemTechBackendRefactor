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
		if (ValueIsAlreadySet(TotalCount))
			return;
		TotalCount = totalCount;
	}

	public void SetAveragePrice(double averagePrice)
	{
		if (ValueIsAlreadySet(AveragePrice))
			return;
		AveragePrice = averagePrice;
	}

	public void SetMinimalPrice(double minimalPrice)
	{
		if (ValueIsAlreadySet(MinimalPrice))
			return;
		MinimalPrice = minimalPrice;
	}

	public void SetMaximalPrice(double maximalPrice)
	{
		if (ValueIsAlreadySet(MaximalPrice))
			return;
		MaximalPrice = maximalPrice;
	}

	private static bool ValueIsAlreadySet(double value) => value != 0;
}
