using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Categories;

public readonly record struct CategoryId
{
	public CategoryId()
	{
		Id = Guid.NewGuid();
	}

	private CategoryId(Guid id)
	{
		Id = id;
	}

	public Guid Id { get; }

	public static Result<CategoryId> Create(Guid id)
	{
		return id == Guid.Empty
			? Error.Validation("Идентификатор категории не может быть пустым.")
			: new CategoryId(id);
	}
}
