using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Oems;

/// <summary>
/// OEM-номер запчасти.
/// </summary>
public sealed class SpareOem
{
	private SpareOem(string value, SpareOemId id)
	{
		Value = value;
		Id = id;
	}

	/// <summary>
	/// Значение OEM-номера.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Идентификатор OEM-номера запчасти.
	/// </summary>
	public SpareOemId Id { get; }

	/// <summary>
	/// Создаёт OEM-номер из строки.
	/// </summary>
	/// <param name="value">Строковое значение OEM-номера.</param>
	/// <returns>Результат создания OEM-номера.</returns>
	public static Result<SpareOem> Create(string value)
	{
		SpareOemId id = SpareOemId.New();
		return string.IsNullOrWhiteSpace(value)
			? Error.Validation("OEM-номер запчасти не может быть пустым.")
			: Result.Success(new SpareOem(value, id));
	}

	public static Result<SpareOem> Create(string value, Guid id)
	{
		Result<SpareOemId> spareOemIdResult = SpareOemId.FromGuid(id);
		if (spareOemIdResult.IsFailure)
		{
			return spareOemIdResult.Error;
		}

		return string.IsNullOrWhiteSpace(value)
			? Error.Validation("OEM-номер запчасти не может быть пустым.")
			: Result.Success(new SpareOem(value, spareOemIdResult.Value));
	}
}
