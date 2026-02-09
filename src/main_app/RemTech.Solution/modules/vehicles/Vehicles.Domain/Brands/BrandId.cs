using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands;

/// <summary>
/// Идентификатор бренда.
/// </summary>
public readonly record struct BrandId
{
	/// <summary>
	/// Создаёт новый идентификатор бренда.
	/// </summary>
	public BrandId()
	{
		Id = Guid.NewGuid();
	}

	/// <summary>
	/// Создаёт идентификатор бренда из заданного значения.
	/// </summary>
	/// <param name="id">Значение идентификатора бренда.</param>
	private BrandId(Guid id)
	{
		Id = id;
	}

	/// <summary>
	/// Значение идентификатора бренда.
	/// </summary>
	public Guid Id { get; }

	/// <summary>
	/// Создаёт идентификатор бренда из заданного значения.
	/// </summary>
	/// <param name="id">Значение идентификатора бренда.</param>
	/// <returns>Результат создания идентификатора бренда.</returns>
	public static Result<BrandId> Create(Guid id)
	{
		return id == Guid.Empty ? Error.Validation("Идентификатор бренда не может быть пустым.") : new BrandId(id);
	}
}
