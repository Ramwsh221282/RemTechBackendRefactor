using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands;

public readonly record struct BrandId
{
	public BrandId()
	{
		Id = Guid.NewGuid();
	}

	private BrandId(Guid id)
	{
		Id = id;
	}

	public Guid Id { get; }

	public static Result<BrandId> Create(Guid id)
	{
		return id == Guid.Empty ? Error.Validation("Идентификатор бренда не может быть пустым.") : new BrandId(id);
	}
}
