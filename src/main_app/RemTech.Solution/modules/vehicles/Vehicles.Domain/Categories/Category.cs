using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Categories;

public sealed class Category(CategoryId id, CategoryName name) : IPersistable<Category>
{
	public CategoryId Id { get; } = id;
	public CategoryName Name { get; } = name;

	public Task<Result<Category>> SaveBy(IPersister persister, CancellationToken ct = default) =>
		persister.Save(this, ct);
}
