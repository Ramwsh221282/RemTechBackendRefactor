using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Categories;

/// <summary>
/// Идентификатор категории транспортного средства.
/// </summary>
public readonly record struct CategoryId
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="CategoryId"/> с новым уникальным идентификатором.
	/// </summary>
	public CategoryId()
	{
		Id = Guid.NewGuid();
	}

	private CategoryId(Guid id)
	{
		Id = id;
	}

	/// <summary>
	/// Идентификатор категории транспортного средства.
	/// </summary>
	public Guid Id { get; }

	/// <summary>
	/// Создаёт идентификатор категории транспортного средства.
	/// </summary>
	/// <param name="id">Уникальный идентификатор категории.</param>
	/// <returns>Результат создания идентификатора категории.</returns>
	public static Result<CategoryId> Create(Guid id)
	{
		return id == Guid.Empty
			? Error.Validation("Идентификатор категории не может быть пустым.")
			: new CategoryId(id);
	}
}
