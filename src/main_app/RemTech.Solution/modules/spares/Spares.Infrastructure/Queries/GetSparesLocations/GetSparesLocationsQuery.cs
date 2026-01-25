using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Infrastructure.Queries.GetSparesLocations;

// TODO: amount is not used. Use it in query handler as LIMIT for sql.
public sealed class GetSparesLocationsQuery : IQuery
{
	private GetSparesLocationsQuery() { }

	public string? TextSearch { get; private init; }
	public int? Amount { get; private init; }

	public GetSparesLocationsQuery WithTextSearch(string? text) => Copy(this, textSearch: text);

	public GetSparesLocationsQuery WithAmount(int? amount) => Copy(this, amount: amount);

	public static GetSparesLocationsQuery Create() => new();

	public override string ToString() => JsonSerializer.Serialize(this);

	private static GetSparesLocationsQuery Copy(
		GetSparesLocationsQuery origin,
		string? textSearch = null,
		int? amount = null
	) =>
		new()
		{
			TextSearch = string.IsNullOrWhiteSpace(textSearch) ? origin.TextSearch : textSearch,
			Amount = amount ?? origin.Amount,
		};
}
