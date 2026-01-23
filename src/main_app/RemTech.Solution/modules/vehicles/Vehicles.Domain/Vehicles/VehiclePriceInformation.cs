using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehiclePriceInformation
{
	public long Value { get; }
	public bool IsNds { get; }

	private VehiclePriceInformation(long value, bool isNds)
	{
		Value = value;
		IsNds = isNds;
	}

	public static Result<VehiclePriceInformation> Create(long value, bool isNds)
	{
		if (value <= 0)
			return Error.Validation("Цена техники не может быть меньше или равной нулю.");
		return new VehiclePriceInformation(value, isNds);
	}
}
