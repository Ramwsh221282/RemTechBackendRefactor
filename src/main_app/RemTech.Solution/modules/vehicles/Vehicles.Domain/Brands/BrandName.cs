using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands;

public sealed record BrandName
{
	private const int MaxLength = 128;

	private BrandName(string name)
	{
		Name = name;
	}

	public string Name { get; }

	public static Result<BrandName> Create(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Error.Validation("Название бренда не может быть пустым.");
		return name.Length > MaxLength
			? Error.Validation($"Название бренда превышает {MaxLength} символов.")
			: new BrandName(name);
	}
}
