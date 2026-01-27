namespace Spares.Infrastructure.Queries.GetSpareTypes;

/// <summary>
/// Ответ с информацией о типе запчасти.
/// </summary>
public sealed class SpareTypeResponse
{
	/// <summary>
	/// Идентификатор типа запчасти.
	/// </summary>
	public required string Value { get; set; }
}
