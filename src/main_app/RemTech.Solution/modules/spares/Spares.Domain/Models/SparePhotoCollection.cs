using RemTech.SharedKernel.Core.Enumerables;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Коллекция фото запчастей.
/// </summary>
public sealed class SparePhotoCollection
{
	/// <summary>
	/// Создаёт коллекцию фото запчастей.
	/// </summary>
	/// <param name="value">Список фото запчастей.</param>
	private SparePhotoCollection(IReadOnlyList<SparePhoto> value)
	{
		Value = value;
	}

	/// <summary>
	/// Список фото запчастей.
	/// </summary>
	public IReadOnlyList<SparePhoto> Value { get; }

	/// <summary>
	/// Создаёт коллекцию фото из списка.
	/// </summary>
	/// <param name="value">Список фото запчастей.</param>
	/// <returns>Результат создания коллекции фото.</returns>
	public static Result<SparePhotoCollection> Create(IReadOnlyList<SparePhoto> value)
	{
		return value.HasDuplicates(out _) switch
		{
			true => Error.Validation("Фото запчасти не может содержать дубликаты: "),
			false => new SparePhotoCollection(value),
		};
	}

	/// <summary>
	/// Создаёт коллекцию фото из перечисления строк.
	/// </summary>
	/// <param name="values">Перечисление строковых представлений фото.</param>
	/// <returns>Результат создания коллекции фото.</returns>
	public static Result<SparePhotoCollection> Create(IEnumerable<string> values)
	{
		List<Result<SparePhoto>> photos = [.. values.Select(SparePhoto.Create)];
		int failuresCount = photos.Count(p => p.IsFailure);
		return failuresCount switch
		{
			0 => Create(photos.ConvertAll(p => p.Value)),
			_ => Error.Validation($"Фото запчастей содержат {failuresCount} ошибок."),
		};
	}
}
