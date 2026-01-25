using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehicleSource
{
	private VehicleSource(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<VehicleSource> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<VehicleSource>)Error.Validation("Источник техники не может быть пустым.")
			: (Result<VehicleSource>)new VehicleSource(value);
	}
}
