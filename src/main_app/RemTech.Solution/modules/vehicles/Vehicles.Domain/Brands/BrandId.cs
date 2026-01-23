using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands;

public readonly record struct BrandId
{
	public Guid Id { get; }

	public BrandId()
	{
		Id = Guid.NewGuid();
	}

	private BrandId(Guid id)
	{
		Id = id;
	}

	public static Result<BrandId> Create(Guid id)
	{
		if (id == Guid.Empty)
			return Error.Validation("Идентификатор бренда не может быть пустым.");
		return new BrandId(id);
	}
}
