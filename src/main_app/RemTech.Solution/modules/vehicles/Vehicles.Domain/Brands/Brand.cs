using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Brands;

public class Brand(BrandId id, BrandName name) : IPersistable<Brand>
{
	public BrandId Id { get; } = id;
	public BrandName Name { get; } = name;

	public Task<Result<Brand>> SaveBy(IPersister persister, CancellationToken ct = default) => persister.Save(this, ct);
}
