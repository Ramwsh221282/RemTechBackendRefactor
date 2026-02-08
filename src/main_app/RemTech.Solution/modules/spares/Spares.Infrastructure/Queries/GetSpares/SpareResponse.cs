namespace Spares.Infrastructure.Queries.GetSpares;

/// <summary>
/// Ответ с информацией о запчасти.
/// </summary>
public sealed class SpareResponse
{
	/// <summary>
	/// Идентификатор запчасти.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// URL запчасти.
	/// </summary>
	public required string Url { get; set; }

	/// <summary>
	/// Цена запчасти.
	/// </summary>
	public required long Price { get; set; }

	/// <summary>
	/// OEM запчасти.
	/// </summary>
	public required string Oem { get; set; }

	/// <summary>
	/// Описание запчасти.
	/// </summary>
	public required string Text { get; set; }

	/// <summary>
	/// Тип запчасти.
	/// </summary>
	public required string Type { get; set; }

	/// <summary>
	/// Фотографии запчасти.
	/// </summary>
	public required bool IsNds { get; set; }

	/// <summary>
	///     Адрес запчасти.
	/// </summary>
	public required string Location { get; set; }

    public IReadOnlyList<string> Photos { get; set; } = [];
}

public sealed record SparePhotoResponse(string Value);
