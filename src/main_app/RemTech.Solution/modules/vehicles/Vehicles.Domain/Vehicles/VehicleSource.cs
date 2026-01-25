using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehicleSource
{
	public string Value { get; }

	private VehicleSource(string value)
	{
		Value = value;
	}

	public static Result<VehicleSource> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<VehicleSource>)Error.Validation("Источник техники не может быть пустым.")
			: (Result<VehicleSource>)new VehicleSource(value);
	}
}
